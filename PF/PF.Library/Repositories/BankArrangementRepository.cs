using AccountingLegacy.Core.Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Library.Models;
using Dapper;
using AccountingLegacy;
using MoreLinq;
using PF.Library.Models;
using PF.Library.ViewModels;

namespace PF.Library.Repositories
{
    internal class BankArrangementRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public BankArrangementRepository(string empCode = "")
        {
            server = new SERVER("PF Bank Arrangement");
            this.empCode = empCode;
        }

        public void PostSetup(int docEntry)
        {
            using (var sap = new SAPBusinessOne())
            {
                try
                {
                    var data = GetSetupTemplate(docEntry);
                    if (data is null) return;
                    if (data.Count() == 0) return;
                    var jrnlEntry = sap.JournalEntries;
                    jrnlEntry.ReferenceDate = data.First().RefDate;
                    jrnlEntry.DueDate = data.First().RefDate;
                    jrnlEntry.TaxDate = data.First().RefDate;
                    jrnlEntry.Memo = data.First().LineMemo.Trim();
                    jrnlEntry.Reference = data.First().Ref1.Trim();
                    jrnlEntry.Reference2 = data.First().Ref2.Trim();
                    jrnlEntry.Reference3 = data.First().Ref3.Trim();
                    jrnlEntry.UserFields.Fields.Item("U_FTDocNo").Value = docEntry.ToString();

                    foreach (var item in data)
                    {
                        jrnlEntry.Lines.AccountCode = item.Account;
                        jrnlEntry.Lines.Debit = Convert.ToDouble(item.Debit);
                        jrnlEntry.Lines.Credit = Convert.ToDouble(item.Credit);
                        jrnlEntry.Lines.LineMemo = item.LineMemo;
                        jrnlEntry.Lines.Reference1 = item.Ref1;
                        jrnlEntry.Lines.Reference2 = item.Ref2;
                        jrnlEntry.Lines.AdditionalReference = item.Ref3;
                        //jrnlEntry.Lines.UserFields.Fields.Item("U_EmpID").Value = item.Ref1;
                        jrnlEntry.Lines.Add();
                    }

                    int returnValue = jrnlEntry.Add();
                    if (returnValue == 0)
                    {
                        var transId = Convert.ToInt32(sap.Company.GetNewObjectKey());

                        using (IDbConnection cn = new SqlConnection(server.SAP_PF))
                        {
                            var storedProc = "spProfFees";
                            var parameters = new
                            {
                                mode = "POST_CA",
                                docEntry = docEntry,
                                transId = transId,
                                empCode = empCode,
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }

                    }
                    else
                    {
                        //INSERT LOGS
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }


                }
                catch (Exception ex)
                {
                    LogError(new BankArrErrorLogs
                    {
                        Module = "SETUP",
                        DocEntry = docEntry,
                        ErrorMsg = ex.GetBaseException().Message
                    });

                    throw ex;
                }
            }
        }

        public void PostAccrual(int docEntry)
        {
            using (var sap = new SAPBusinessOne())
            {
                try
                {
                    var data = GetAccrualTemplate(docEntry);
                    if (data is null) return;
                    if (data.Count() == 0) return;
                    var jrnlEntry = sap.JournalEntries;
                    jrnlEntry.ReferenceDate = data.First().RefDate;
                    jrnlEntry.DueDate = data.First().RefDate;
                    jrnlEntry.TaxDate = data.First().RefDate;
                    jrnlEntry.Memo = data.First().LineMemo.Trim();
                    jrnlEntry.Reference = data.First().Ref1.Trim();
                    jrnlEntry.Reference2 = data.First().Ref2.Trim();
                    jrnlEntry.Reference3 = data.First().Ref3.Trim();
                    jrnlEntry.UserFields.Fields.Item("U_FTDocNo").Value = docEntry.ToString();

                    foreach (var item in data)
                    {
                        jrnlEntry.Lines.AccountCode = item.Account;
                        jrnlEntry.Lines.Debit = Convert.ToDouble(item.Debit);
                        jrnlEntry.Lines.Credit = Convert.ToDouble(item.Credit);
                        jrnlEntry.Lines.LineMemo = item.LineMemo;
                        jrnlEntry.Lines.Reference1 = item.Ref1;
                        jrnlEntry.Lines.Reference2 = item.Ref2;
                        jrnlEntry.Lines.AdditionalReference = item.Ref3;
                        jrnlEntry.Lines.UserFields.Fields.Item("U_EmpID").Value = item.Ref1;
                        jrnlEntry.Lines.Add();
                    }

                    int returnValue = jrnlEntry.Add();
                    if (returnValue == 0)
                    {
                        var transId = Convert.ToInt32(sap.Company.GetNewObjectKey());

                        using (IDbConnection cn = new SqlConnection(server.SAP_PF))
                        {
                            var storedProc = "spProfFees";
                            var parameters = new
                            {
                                mode = "POST_ACCRUAL",
                                docEntry = docEntry,
                                transId = transId,
                                empCode = empCode,
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }

                    }
                    else
                    {
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }


                }
                catch (Exception ex)
                {
                    LogError(new BankArrErrorLogs
                    {
                        Module = "ACCRUAL",
                        DocEntry = docEntry,
                        ErrorMsg = ex.GetBaseException().Message
                    });

                    throw ex;
                }
            }
        }

        public void PostReversal(int docEntry)
        {
            using (var sap = new SAPBusinessOne())
            {
                try
                {
                    var data = GetReversalTemplate(docEntry);
                    if (data is null) return;
                    if (data.Count() == 0) return;

                    var jrnlEntry = sap.JournalEntries;
                    jrnlEntry.ReferenceDate = data.First().RefDate;
                    jrnlEntry.DueDate = data.First().RefDate;
                    jrnlEntry.TaxDate = data.First().RefDate;
                    jrnlEntry.Memo = data.First().LineMemo.Trim();
                    jrnlEntry.Reference = data.First().Ref1.Trim();
                    jrnlEntry.Reference2 = data.First().Ref2.Trim();
                    jrnlEntry.Reference3 = data.First().Ref3.Trim();
                    jrnlEntry.UserFields.Fields.Item("U_FTDocNo").Value = docEntry.ToString();

                    foreach (var item in data)
                    {
                        jrnlEntry.Lines.AccountCode = item.Account;
                        jrnlEntry.Lines.Debit = Convert.ToDouble(item.Debit);
                        jrnlEntry.Lines.Credit = Convert.ToDouble(item.Credit);
                        jrnlEntry.Lines.LineMemo = item.LineMemo;
                        jrnlEntry.Lines.Reference1 = item.Ref1;
                        jrnlEntry.Lines.Reference2 = item.Ref2;
                        jrnlEntry.Lines.AdditionalReference = item.Ref3;
                        jrnlEntry.Lines.UserFields.Fields.Item("U_EmpID").Value = item.Ref1;
                        jrnlEntry.Lines.Add();
                    }

                    int returnValue = jrnlEntry.Add();
                    if (returnValue == 0)
                    {
                        var transId = Convert.ToInt32(sap.Company.GetNewObjectKey());

                        using (IDbConnection cn = new SqlConnection(server.SAP_PF))
                        {
                            var storedProc = "spProfFees";
                            var parameters = new
                            {
                                mode = "POST_REVERSAL",
                                docEntry = docEntry,
                                transId = transId,
                                empCode = empCode,
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }

                    }
                    else
                    {
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }


                }
                catch (Exception ex)
                {
                    LogError(new BankArrErrorLogs
                    {
                        Module = "REVERSAL",
                        DocEntry = docEntry,
                        ErrorMsg = ex.GetBaseException().Message
                    });

                    throw ex;
                }
            }
        }

        public void PostPayment(DateTime bankDate, List<PayTransModel> docEntries)
        {
            var data = GetPaymentTemplate(bankDate, docEntries);
            foreach (var header in data.Headers)
            {
                try
                {
                    using (var sap = new SAPBusinessOne())
                    {
                        
                        var pay = sap.VendorPayments;

                        pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                        pay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                        pay.CardName = header.CardName;

                        
                        if (header.PMode.IndexOf("ATM") >= 0)
                        {
                            pay.TransferAccount = header.AcctCode;
                            pay.TransferSum = (double)header.BankAmt;
                        }
                        else if (header.PMode.Equals("CHEQUE"))
                        {
                            pay.Checks.BankCode = header.BankCode;
                            pay.Checks.Branch = header.U_BranchCode;
                            pay.Checks.AccounttNum = header.U_BranchCode;
                            pay.Checks.CheckAccount = header.AcctCode;
                            pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            pay.Checks.CheckSum = (double)header.BankAmt;
                        }
                        else
                        {
                            throw new ApplicationException("Payment mode not found.");
                        }

                        pay.DocCurrency = "PHP";
                        pay.UserFields.Fields.Item("U_ChkNum").Value = header.U_ChkNum;
                        pay.UserFields.Fields.Item("U_CardCode").Value = header.U_CardCode;
                        pay.UserFields.Fields.Item("U_BranchCode").Value = header.U_BranchCode;

                        foreach (var detail in data.Details.Where(x => x.DocNum == header.DocNum))
                        {
                            pay.AccountPayments.AccountCode = detail.AcctCode;
                            pay.AccountPayments.SumPaid = Convert.ToDouble(detail.SumApplied);
                            pay.AccountPayments.Decription = detail.Description;
                            //pay.AccountPayments.UserFields.Fields.Item("U_EmpID").Value = header.U_CardCode;
                            pay.AccountPayments.Add();
                        }

                        int returnValue = pay.Add();
                        if (returnValue == 0)
                        {
                            var docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());

                            using (IDbConnection cn = new SqlConnection(server.SAP_PF2))
                            {
                                var storedProc = "spProfFees";
                                var parameters = new
                                {
                                    mode = "POST_PAYMENT",
                                    empCode = empCode,
                                    type = header.AcctType,
                                    docNum = docNum,
                                    docEntries = data.DocEntries.Where(x => x.DocNum == header.DocNum).Select(x => x.DocEntry).ToDataTable(),
                                };

                                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                            }

                        }
                        else
                        {
                            throw new ApplicationException(sap.Company.GetLastErrorDescription());
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogError(new BankArrErrorLogs
                    {
                        Module = "PAYMENT",
                        ErrorMsg = ex.GetBaseException().Message
                    });
                }
            }
        }

        private IEnumerable<JrnlEntryDetail> GetSetupTemplate(int docEntry)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {
                return cn.Query<JrnlEntryDetail>(
                    "spProfFees",
                    new
                    {
                        mode = "CA_TEMPLATE",
                        docEntry = docEntry
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        private IEnumerable<JrnlEntryDetail> GetAccrualTemplate(int docEntry)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {
                return cn.Query<JrnlEntryDetail>(
                    "spProfFees",
                    new
                    {
                        mode = "ACCRUAL_TEMPLATE",
                        docEntry = docEntry
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        private IEnumerable<JrnlEntryDetail> GetReversalTemplate(int docEntry)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {
                return cn.Query<JrnlEntryDetail>(
                    "spProfFees",
                    new
                    {
                        mode = "REVERSAL_TEMPLATE",
                        docEntry = docEntry
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        private PaymentView GetPaymentTemplate(DateTime bankDate, List<PayTransModel> docEntries)
        {
            var output = new PaymentView();
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {

                using (var multi = cn.QueryMultiple
                (
                    "spProfFees",
                    new
                    {
                        mode = "PAYMENT_TEMPLATE",
                        docDate = bankDate,
                        docEntries = docEntries.ToDataTable(),
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Headers = multi.Read<PaymentHeaderView>();
                    output.Details = multi.Read<PaymentAccountView>();
                    output.DocEntries = multi.Read<DocumentEntryView>();
                    return output;
                }
            }
        }

        private void LogError(BankArrErrorLogs log)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {
                cn.Execute(
                    "spProfFeesError",
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

    }
}
