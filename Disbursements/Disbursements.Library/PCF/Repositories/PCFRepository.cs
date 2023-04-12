using AccountingLegacy;
using AccountingLegacy.Core.Library;
using Core.Library.Models;
using Disbursements.Library.PCF.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SAPbobsCOM;
using Disbursements.Library.PCF.Helpers;
using Disbursements.Library.PCF.Models;
using System.Reflection.PortableExecutable;
using System.Numerics;
using System.Security.Principal;
using MoreLinq;
using AccountingLegacy.Disbursements.Library.PCF.ViewModels;
using AccountingLegacy.Disbursements.Library.PCF.Models;

namespace Disbursements.Library.PCF.Repositories
{
    internal class PCFRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public PCFRepository(string empCode = "")
        {
            server = new SERVER(PcfBuilder.PCFSERVER());
            this.empCode = empCode;
        }

        public int PostJrnlEntry(JrnlEntryView data)
        {
            try
            {

                var docEntry = UpdateData(data);
                var jrnlEntry = GetTemplate(docEntry);

                using (var sap = new SAPBusinessOne())
                {
                    var entry = sap.JournalEntries;
                    entry.ReferenceDate = jrnlEntry.Header.DocDate;
                    entry.Memo = jrnlEntry.Header.Memo.Trim();
                    entry.Reference = jrnlEntry.Header.Ref1.Trim();
                    entry.Reference2 = jrnlEntry.Header.Ref2.Trim();
                    entry.UserFields.Fields.Item("U_FTDocNo").Value = docEntry.ToString();
                    if (jrnlEntry.Header.Ref3 is not null) entry.Reference3 = jrnlEntry.Header.Ref3.Trim();

                    foreach (var item in jrnlEntry.Details)
                    {
                        entry.Lines.AccountCode = item.Account;
                        entry.Lines.Debit = Convert.ToDouble(item.Debit);
                        entry.Lines.Credit = Convert.ToDouble(item.Credit);
                        entry.Lines.LineMemo = item.LineMemo;
                        entry.Lines.ShortName = item.ShortName;
                        entry.Lines.Reference1 = item.Ref1;
                        entry.Lines.Reference2 = item.Ref2;
                        entry.Lines.Add();
                    }
                    var returnValue = entry.Add();

                    if (returnValue == 0)
                    {
                        var transId = Convert.ToInt32(sap.Company.GetNewObjectKey());

                        using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                        {
                            var storedProc = "spPCFPosting";
                            var parameters = new
                            {
                                mode = "POST_JE",
                                transId = transId,
                                pcfOP = jrnlEntry.Header.PCFOP,
                                pcfDoc = jrnlEntry.Header.Ref2.Trim(),
                                empCode = empCode,
                                docEntry = docEntry
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }
                        using (IDbConnection cn = new SqlConnection(server.EMS_HPCOMMON))
                        {
                            var storedProc = PcfBuilder.spPcfLegacy1051();
                            var parameters = new
                            {
                                mode = PcfBuilder.spModeJEUpdateTables(),
                                transId = transId,
                                pcfOP = jrnlEntry.Header.PCFOP,
                                pcfDoc = jrnlEntry.Header.Ref2.Trim(),

                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        }
                        sap.Commit();

                        using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                        {
                            var storedProc = "spJrnlEntryLogs";
                            var parameters = new
                            {
                                mode = "PCFPostJE",
                                empID = empCode,
                                transId = transId,
                                module = "PCF JE POSTING"

                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        }

                        return transId;
                    }
                    else
                    {
                        var err = sap.Company.GetLastErrorDescription();
                        sap.Rollback();
                        throw new ApplicationException(err);
                    }

                }
            }
            catch (Exception ex)
            {

                LogError(new PCFErrorLogs
                {
                    Module = "PCF POST JE",
                    ErrorMsg = ex.GetBaseException().Message,
                    DocEntry = data.Header.Docentry,
                    Remarks = "",
                    PostedBy = empCode
                });

                throw new ApplicationException(ex.Message);
            }
        }

        private int UpdateData(JrnlEntryView jrnlEntry)
        {
            //var output = new JrnlEntryView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                List<JournalEntryHeaderView> headerTable = new List<JournalEntryHeaderView>();

                headerTable.Add(jrnlEntry.Header);

                return cn.ExecuteScalar<int>(
                    "spPCFPosting",
                    new
                    {
                        mode = "UPDATE_JE",
                        header = headerTable.ToDataTable(),
                        details = jrnlEntry.Details.ToDataTable(),
                        //docEntry = jrnlEntry.Header.Docentry
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);

            }
        }

        private JrnlEntryView GetTemplate(int docEntry)
        {

            var output = new JrnlEntryView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {

                using (var multi = cn.QueryMultiple
                (
                    "spPCFPosting",
                    new
                    {
                        mode = "GET_JE_TEMPLATE",
                        docEntry = docEntry,
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Header = multi.Read<JournalEntryHeaderView>().Single();
                    output.Details = multi.Read<JournalEntrDetailView>();
                    return output;
                }
            }


        }

        private void LogError(PCFErrorLogs log)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                cn.Execute(
                    "spPCFJEError",
                    new
                    {
                        mode = "INSERT",
                        module = log.Module,
                        message = log.ErrorMsg,
                        empCode = log.PostedBy
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public int PostPayment(PCFOP data)
        {
            var model = GetPaymentTemplate(data);

            using (var sap = new SAPBusinessOne("172.30.1.167"))
            {
                var opEntryPcfovpm = PostPayment_EMS(data);
                try
                {
                    var pay = sap.VendorPayments;
                    //GET DOCENTRY FROM LOOKUP TABLE IN EMS SERVER (pcfovpm)
                

                    sap.BeginTran();
                    //POST OP
                    pay.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;

                    pay.CardCode = string.Empty;
                    pay.CardName = model.Header.Payee;
                    pay.DocDate = model.Header.DocDate;
                    pay.DueDate = model.Header.DocDate;
                    pay.TaxDate = model.Header.DocDate;
                    pay.Remarks = model.Header.Remarks;
                    pay.JournalRemarks = model.Header.Payee;
                    pay.Address = model.Header.Address;
                    pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                    pay.DocCurrency = "PHP";
                    pay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = string.IsNullOrEmpty(data.Header.ChkNum) ? "" : data.Header.ChkNum.Trim();
                    pay.UserFields.Fields.Item("U_BranchCode").Value = data.Header.BranchCode.Trim();
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = model.Header.U_HPDVoucherNo!.Trim();
                    pay.Reference2 = opEntryPcfovpm.ToString();
                    pay.UserFields.Fields.Item("U_CardCode").Value = "PCF-" + opEntryPcfovpm.ToString();

                    if (model.Accounts is not null && model.Accounts.Count() > 0)
                    {
                        foreach (var item in model.Accounts)
                        {
                            pay.AccountPayments.AccountCode = item.AcctCode;
                            pay.AccountPayments.SumPaid = (double)item.SumPaid;
                            pay.AccountPayments.Decription = item.Description;
                            pay.AccountPayments.UserFields.Fields.Item("U_DocLine").Value = item.U_Docline.ToString();
                            pay.AccountPayments.Add();
                        }
                    }

                    if (model.Checks is not null && model.Checks.Count() > 0)
                    {
                        foreach (var item in model.Checks)
                        {
                            pay.Checks.Branch = item.WhsCode;
                            pay.Checks.AccounttNum = item.AccounttNum;
                            pay.Checks.DueDate = item.DueDate;
                            pay.Checks.CountryCode = "PH";
                            pay.Checks.BankCode = item.BankCode;
                            pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            pay.Checks.CheckAccount = item.CheckAccount;
                            pay.Checks.CheckSum = (double)item.CheckSum;
                            pay.Checks.Add();
                        }
                    }

                    var returnValue = pay.Add();

                    if (returnValue == 0)
                    {
                        var _opNumber = Convert.ToInt32(sap.Company.GetNewObjectKey());

                   
                        List<PCFOPHeader> headerTable = new List<PCFOPHeader>();
                        headerTable.Add(data.Header);

                        //POPULATE LOOKUP TABLES IN SAP SERVER (OVPM AND PCF)
                        using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                        {
                            var storedProc = "spPCFPosting";
                            var parameters = new
                            {
                                mode = "POST_OP",
                                opDetail = data.Detail,
                                opNumber = _opNumber,
                                postBy = empCode,
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                            cn.Execute(storedProc, new
                            {
                                mode = "POST_PCF_OP",
                                opHeader = headerTable.ToDataTable(),
                                opEntryPcfovpm = opEntryPcfovpm
                            }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }

                    
                            sap.Commit();

                            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                            {
                                var storedProc = "spSapOPLogs";
                                var parameters = new
                                {
                                    mode = "PCFSAPOP",
                                    empID = empCode,
                                    opNumber = _opNumber,
                                    module = "PCF OP POSTING"

                                };

                                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                            }
                            return _opNumber;

                    }
                    else
                    {
                        var err = sap.Company.GetLastErrorDescription();
                        DeletePcfOVPM(opEntryPcfovpm);
                        sap.Rollback();
                        throw new ApplicationException(err);
                    }
                }
                catch (Exception ex)
                {
                    DeletePcfOVPM(opEntryPcfovpm);
                    sap.Rollback();

                    LogError(new PCFErrorLogs
                    {
                        Module = "PCF POST OP",
                        ErrorMsg = ex.GetBaseException().Message,
                        PostedBy = empCode
                    });

                    throw new ApplicationException(ex.GetBaseException().Message);
                }
            }


            //return 0;
        }

        public void TagPcfPayment(PCFOP data, int opEntryPcfovpm)
        {
            try
            {
                //OLD UPDATE
           

                using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                {
                    var storedProc = "spPCFPosting";
                    var parameters = new
                    {
                        mode = "POST_TAG_PCFPAYMENT",
                        opNumber = data.Header.OPNum,
                        bank = data.Header.Bank
                    };

                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    cn.Execute(storedProc,
                        new
                        {
                            mode = "POST_OP",
                            opDetail = data.Detail,
                            opNumber = data.Header.OPNum,
                            postBy = empCode,
                        }, commandType: CommandType.StoredProcedure, commandTimeout: 0);


                    List<PCFOPHeader> headerTable = new List<PCFOPHeader>();
                    headerTable.Add(data.Header);

                    cn.Execute(storedProc,
                        new
                        {
                            mode = "POST_PCF_OP",
                            opHeader = headerTable.ToDataTable(),
                            opEntryPcfovpm = opEntryPcfovpm
                        }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }

                PCFUpdateOP(opEntryPcfovpm, data.Header.OPNum.ToString());
            }
            catch (Exception ex)
            {
                LogError(new PCFErrorLogs
                {
                    Module = "PCF TAG PCF OP",
                    ErrorMsg = ex.GetBaseException().Message,
                    PostedBy = empCode
                });

            }
        }

        private bool PCFUpdateOP(int OPNum, string DocEntry)
        {
            using (var sap = new SAPBusinessOne())
            {
                try
                {
                    sap.BeginTran();
                    var entry = sap.VendorPayments;
                    entry.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;

                    if (entry.GetByKey(OPNum))
                    {
                        entry.Reference2 = DocEntry;
                        entry.UserFields.Fields.Item("U_CardCode").Value = "PCF-" + DocEntry;
                        sap.Commit();
                    }
                    else
                    {
                        sap.Rollback();
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    sap.Rollback();

                    LogError(new PCFErrorLogs
                    {
                        Module = "PCF UPDATE OP",
                        ErrorMsg = ex.GetBaseException().Message,
                        PostedBy = empCode
                    });

                    return false;
                }
            }
        }

        public int PostPayment_EMS(PCFOP data)
        {
            using (IDbConnection cn = new SqlConnection(server.EMS_HPCOMMON))
            {
                List<PCFOPHeader> headerTable = new List<PCFOPHeader>();
                headerTable.Add(data.Header);
                return cn.ExecuteScalar<int>(
                      PcfBuilder.spPcfLegacy1051(),
                      new
                      {
                          mode = "POST_OP",
                          opHeader = headerTable.ToDataTable(),
                          opDetail = data.Detail.ToDataTable(),

                      }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        private PCFOPView GetPaymentTemplate(PCFOP data)
        {

            List<PCFOPHeader> headerTable = new List<PCFOPHeader>();

            headerTable.Add(data.Header);

            var output = new PCFOPView();

            try
            {
                using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                {
                    using (var multi = cn.QueryMultiple
                    (
                        "spPCFPosting",
                        new
                        {
                            mode = "GET_POSTOP_TEMPLATE",
                            opHeader = headerTable.ToDataTable(),
                            opDetail = data.Detail.ToDataTable(),
                        }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                    )
                    {
                        output.Header = multi.Read<PCFOPHeaderView>().Single();
                        output.Accounts = multi.Read<PCFOPAccountsView>().ToList();
                        output.Checks = multi.Read<PCFOPChecksView>().ToList();
                        return output;
                    }
                }
            }
            catch (Exception e )
            {

                throw e.GetBaseException();
            }

      
        }


        private void DeletePcfOVPM(int OpEntry)
        {
            try
            {
  

                using (IDbConnection cn = new SqlConnection(server.EMS_HPCOMMON))
                {
                    var storedProc = PcfBuilder.spPcfLegacy1051();
                    var parameters = new
                    {
                        mode = "DeletePcfovpm",
                        opEntryPcfovpm = OpEntry
                  
                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                  
                }

         
            }
            catch (Exception ex)
            {
                LogError(new PCFErrorLogs
                {
                    Module = "PCF TAG PCF OP",
                    ErrorMsg = ex.GetBaseException().Message,
                    PostedBy = empCode
                });

            }
        }

    }
}
