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
                    sap.BeginTran();
                    var entry = sap.JournalEntries;
                    entry.ReferenceDate = jrnlEntry.Header.DocDate;
                    entry.Memo = jrnlEntry.Header.Memo.Trim();
                    entry.Reference = jrnlEntry.Header.Ref1.Trim();
                    entry.Reference2 = jrnlEntry.Header.Ref2.Trim();
                    entry.UserFields.Fields.Item("U_FTDocNo").Value = docEntry.ToString();
                    if(jrnlEntry.Header.Ref3 is not null) entry.Reference3 = jrnlEntry.Header.Ref3.Trim();

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
                            var storedProc = PcfBuilder.spPcfJE1051();
                            var parameters = new
                            {
                                mode = PcfBuilder.spModeJEUpdateTables(),
                                transId = transId,
                                pcfOP = jrnlEntry.Header.PCFOP,
                                pcfDoc = jrnlEntry.Header.Ref2.Trim(),

                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        }
                        using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                        {
                            var storedProc = "spJrnlEntryLogs";
                            var parameters = new
                            {
                                mode = "PCFJrnlEntry",
                                empID = empCode,
                                transId = transId,
                                module = "Copsweb CreateJE",

                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        }
                        sap.Commit();
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
                    ErrorMsg = ex.GetBaseException().Message
                });
                
                throw;
            }
        }

        private int UpdateData(JrnlEntryView jrnlEntry)
        {
            var output = new JrnlEntryView();
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
                    output.Header =  multi.Read<JournalEntryHeaderView>().Single();
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
                        docEntry = log.DocEntry,
                        remarks = log.Remarks,
                        empCode = this.empCode
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public int PostPayment(PCFOP data) 
        {
            try
            {


                var model = GetPaymentTemplate(data);

                using (var sap = new SAPBusinessOne())
                {
                    sap.BeginTran();
                    var entry = sap.VendorPayments;
                    entry.CardCode = string.Empty;
                    entry.CardName = model.Header.Payee;
                    entry.Address = model.Header.Address;
                    entry.JournalRemarks = model.Header.Payee;
                    entry.DocDate = model.Header.DocDate;
                    entry.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                    entry.DocCurrency = "PHP";
                    entry.Remarks = model.Header.Remarks;
                    entry.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    entry.TaxDate = model.Header.DocDate;
                    if (model.Header.ChkNum is not null) entry.UserFields.Fields.Item("U_ChkNum").Value = model.Header.ChkNum.Trim();
                    if (model.Header.BranchCode is not null) entry.UserFields.Fields.Item("U_BranchCode").Value = model.Header.BranchCode.Trim();
                    entry.UserFields.Fields.Item("U_HPDVoucherNo").Value = model.Header.U_HPDVoucherNo.Trim();

                    foreach (var item in model.Accounts)
                    {
                        entry.AccountPayments.Add();
                        entry.AccountPayments.AccountCode = item.AcctCode;
                        entry.AccountPayments.SumPaid = Convert.ToDouble(item.SumPaid);
                        entry.AccountPayments.Decription = item.Description;
                        entry.AccountPayments.UserFields.Fields.Item("U_DocLine").Value = item.U_Docline.ToString();
                      
                    }
                    foreach (var item in model.Checks)
                    {
                        entry.Checks.Branch = item.WhsCode;
                        entry.Checks.AccounttNum = item.AccountttNum;
                        entry.Checks.DueDate = item.DueDate;
                        entry.Checks.CountryCode = "PH";
                        entry.Checks.BankCode = item.BankCode;
                        entry.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                        entry.Checks.CheckAccount = item.CheckAccount;
                        entry.Checks.CheckSum = Convert.ToDouble(item.CheckSum);
                        entry.Checks.Add();

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
                                mode = "POST_OP",
                                opDetail = data.Detail,
                                opNumber = transId,
                                postBy = empCode,
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);


                        }
                        var opEntryPcfovpm =  UpdateEmsServer(data);
                        using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                        {
                            List<PCFOPHeader> headerTable = new List<PCFOPHeader>();
                            headerTable.Add(data.Header);
                            var storedProc = "spPCFPosting";
                            var parameters = new
                            {
                                mode = "POST_OPpcf",
                                opHeader = headerTable.ToDataTable(),
                                opEntryPcfovpm = opEntryPcfovpm
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }



                        sap.Commit();
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
                    ErrorMsg = ex.GetBaseException().Message
                });

                throw;
            }

            return 0;
        
        }



        private int UpdateEmsServer(PCFOP data)
        {
            var output = new JrnlEntryView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                List<PCFOPHeader> headerTable = new List<PCFOPHeader>();
                headerTable.Add(data.Header);
               return  cn.ExecuteScalar<int>(
                      PcfBuilder.spPcfJE1051(),
                     new
                     {
                         mode = "POST_OP",
                         opHeader = headerTable.ToDataTable(),
                         opDetail = data.Detail.ToDataTable(),

                     }, commandType: CommandType.StoredProcedure, commandTimeout: 0);

            }
        }


        private PCFOPView GetPaymentTemplate(PCFOP data) {

            List<PCFOPHeader> headerTable = new List<PCFOPHeader>();

            headerTable.Add(data.Header);

            var output = new PCFOPView();
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







    }
}
