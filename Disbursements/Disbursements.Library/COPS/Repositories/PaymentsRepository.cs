using AccountingLegacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MoreLinq;
using SAPbobsCOM;
using Disbursements.Library.COPS.ViewModels;
using AccountingLegacy.Core.Library;
using System.Data.SqlClient;
using System.Data;
using AccountingLegacy.Disbursements.Library.COPS.Models;
using AccountingLegacy.Disbursements.Library.COPS.ViewModels;
using Core.Library.Models;
using Disbursements.Library.COPS.Models;
using System.Reflection.PortableExecutable;
using System.Net;

namespace Disbursements.Library.COPS.Repositories
{
    internal class PaymentsRepository
    {
        private readonly SERVER server;
        private readonly string empCode;

        public PaymentsRepository(string empCode="")
        {
            server = new SERVER("Outgoing Payment");
            this.empCode = empCode;

        }
        public PaymentView GetPayment(int docNum)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spOutgoingPayment";
                var parameters = new
                {
                    mode = "GET_PAYMENT",
                    docnum = docNum
                };
                return cn.QuerySingle<PaymentView>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
     
        public void PostPayment(PaymentView payment)
        {
            using(var sap = new SAPBusinessOne())
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();
                    //POST OP
                    pay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                    pay.Address = payment.Header.Address;
                    pay.JournalRemarks = payment.Header.JrnlMemo;
                    pay.DocDate = payment.Header.DocDate;

                    pay.CardCode = payment.Header.CardCode;
                    pay.CardName = payment.Header.CardName;
                    pay.DocType = payment.Header.DocType == "S" ? SAPbobsCOM.BoRcptTypes.rSupplier : SAPbobsCOM.BoRcptTypes.rAccount;
                    pay.DocCurrency = "PHP";
                    pay.Remarks = payment.Header.Comments;
                    pay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    pay.TaxDate = payment.Header.DocDate;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = payment.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = payment.Header.U_CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = payment.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = payment.Header.VoucherNo;

                    if (payment.Header.TransferAmt is not decimal.Zero)
                    {
                        pay.TransferAccount = payment.Header.BankCode;
                        pay.TransferSum = (double)payment.Header.DocTotal;
                        pay.TransferDate = payment.Header.DueDate;
                        pay.PrimaryFormItems.PaymentMeans = SAPbobsCOM.PaymentMeansTypeEnum.pmtBankTransfer;
                    }

                    if (payment.Header.CheckAmt is not decimal.Zero)
                    {
                        pay.Checks.Branch = payment.Header.U_BranchCode;
                        pay.Checks.AccounttNum = payment.Header.U_BranchCode;
                        pay.Checks.DueDate = payment.Header.DueDate;
                        pay.Checks.CountryCode = "PHP";
                        pay.Checks.BankCode = payment.Header.BankCode;
                        pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                        pay.Checks.CheckAccount = payment.Header.AcctCode;
                        pay.Checks.CheckSum = (double)payment.Header.DocTotal;
                    }

                    if (payment.Header.CashAmt is not decimal.Zero)
                    {
                        pay.CashAccount = payment.Header.BankCode;
                        pay.CashSum = (double)payment.Header.DocTotal;
                    }

                    if (payment.Invoices.Count() > 0) {
                        foreach (var dtl in payment.Invoices)
                        {
                            pay.Invoices.DocEntry = dtl.DocEntry;
                            pay.Invoices.InvoiceType = (SAPbobsCOM.BoRcptInvTypes)dtl.InvType;
                            pay.Invoices.SumApplied = (double)dtl.SumApplied;
                            pay.Invoices.Add();

                        }
                    }
                    else if (payment.Accounts.Count() > 0) {
                        foreach (var dtl in payment.Accounts)
                        {
                            pay.AccountPayments.AccountCode = dtl.AcctCode;
                            pay.AccountPayments.SumPaid = (double)dtl.SumApplied;
                            pay.AccountPayments.Decription = "WTAX";
                            pay.AccountPayments.Add();
                        }
                    }
                  
                    var returnValue = pay.Add();
                    var docNum = 0;
                    if (returnValue != 0) docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());

                    sap.Commit();

                    using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                    {
                        var storedProc = "spOutgoingPayment";
                        var parameters = new
                        {
                            mode = "POST_PAYMENT",
                            docnum = docNum,
                            opdata = payment.Header,
                            invoicedata = payment.Invoices.ToDataTable()
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    }

                }
                catch (Exception ex)
                {
                    sap.Rollback();

                    LogError(new PaymentsErrorLogs
                    {
                        Module = "PAYMENT",
                        ErrorMsg = ex.GetBaseException().Message
                    });
                    throw new ApplicationException(ex.GetBaseException().Message);
                }
            }

         
        }

        public void CancelPayment(int docNum)
        {

            using (var sap = new SAPBusinessOne())
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;
                    var data = GetPayment(docNum);

                    sap.BeginTran();
                    //CANCEL OP

                    if (pay.GetByKey(docNum) == true)
                    {
                        var returnValue = pay.Cancel();
                        if (returnValue != 0)
                        {
                            throw new ApplicationException(sap.Company.GetLastErrorDescription());
                        }
                    }
                    else
                    {
                        throw new ApplicationException("OP not found");
                    }

                    if (data.Header.DocType == "S") {
                        foreach (var invitem in data.Invoices)
                        {
                            if (jrnlEntry.GetByKey(invitem.DocTransId) == true)
                            {
                                var returnValue = jrnlEntry.Cancel();
                                if (returnValue != 0)
                                {
                                    throw new ApplicationException(sap.Company.GetLastErrorDescription());
                                }
                            }
                            else
                            {
                                throw new ApplicationException("JE not found");
                            }
                        }
                        
                    }

                    sap.Commit();

                    using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                    {
                        var storedProc = "spOutgoingPayment";
                        var parameters = new
                        {
                            mode = "CANCEL_PAYMENT",
                            docnum = docNum
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    }

                }
                catch (Exception ex)
                {
                    sap.Rollback();

                    LogError(new PaymentsErrorLogs
                    {
                        Module = "CANCEL PAYMENT",
                        ErrorMsg = ex.GetBaseException().Message
                    });
                    throw new ApplicationException(ex.GetBaseException().Message);
                }
            }


        }

        private void LogError(PaymentsErrorLogs log)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {
                cn.Execute(
                    "spPaymentsError",
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
