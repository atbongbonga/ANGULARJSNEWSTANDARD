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
using Disbursements.Library.COPS.Models;
using Core.Library.Models;
using System.Reflection.PortableExecutable;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Disbursements.Library.COPS.Repositories
{
    internal class PaymentRepository
    {
        private readonly SERVER server;
        private readonly string empCode;

        public PaymentRepository(string empCode = "")
        {
            server = new SERVER("Outgoing Payment");
            this.empCode = empCode;

        }

        public void PostPayment(PaymentView payment)
        {
            var data = GetPaymentData(payment);

            using(var sap = new SAPBusinessOne())
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();
                    //POST OP
                    pay.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;

                    if (data.Header.DocType.Equals("S"))
                    {
                        pay.CardCode = data.Header.CardCode;
                        pay.DocType = BoRcptTypes.rSupplier;
                    }
                    else pay.DocType = BoRcptTypes.rAccount;
                    pay.CardName = data.Header.CardName ?? "";
                    pay.DocDate = data.Header.DocDate;
                    pay.DueDate = data.Header.DueDate;
                    pay.TaxDate = data.Header.TaxDate;

                    pay.Remarks = data.Header.Comments ?? "";
                    pay.JournalRemarks = data.Header.JrnlMemo ?? "";
                    pay.Reference1 = data.Header.Ref1 ?? "";
                    pay.Reference2 = data.Header.Ref2 ?? "";

                    pay.Address = data.Header.Address;
                    pay.HandWritten = BoYesNoEnum.tNO;
                    pay.DocCurrency = "PHP";
                    pay.Remarks = data.Header.Comments;
                    pay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = string.IsNullOrEmpty(data.Header.U_ChkNum) == true ? "" : data.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = data.Header.CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = data.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = data.Header.U_ChkNum ?? "";
                    pay.UserFields.Fields.Item("U_CardCode").Value = data.Header.CardCode ?? "";
                    pay.UserFields.Fields.Item("U_BranchCode").Value = data.Header.U_BranchCode ?? "";
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = GetVoucher(data.Header.U_BranchCode, data.Header.DocDate);

                    if (data.Header.PMode.Equals("BANK TRANSFER"))
                    {
                        pay.TransferAccount = data.Header.AcctCode;
                        pay.TransferSum = (double)data.Header.DocTotal;
                        pay.TransferDate = data.Header.TransferDate ?? data.Header.DocDate;
                        pay.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                    }

                    if (data.Checks is not null && data.Checks.Count() > 0)

                    {
                        foreach (var item in data.Checks)
                        {
                            pay.Checks.Branch = item.AcctNum;
                            pay.Checks.AccounttNum = item.AcctNum;
                            pay.Checks.DueDate = item.DueDate;
                            pay.Checks.CountryCode = "PH";
                            pay.Checks.BankCode = item.BankCode;
                            pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            pay.Checks.CheckAccount = item.CheckAcct;
                            pay.Checks.CheckSum = (double)item.CheckAmt;
                            pay.Checks.Add();
                        }
                    }

                    if (data.Invoices is not null && data.Invoices.Count() > 0) {
                        foreach (var dtl in data.Invoices)
                        {
                            pay.Invoices.DocEntry = dtl.DocEntry;
                            pay.Invoices.InvoiceType = (BoRcptInvTypes)dtl.InvType;
                            pay.Invoices.SumApplied = (double)dtl.SumApplied;
                            pay.Invoices.Add();
                        }
                    }

                    if (data.Accounts is not null && data.Accounts.Count() > 0) {
                        foreach (var dtl in data.Accounts)
                        {
                            pay.AccountPayments.AccountCode = dtl.AcctCode;
                            pay.AccountPayments.SumPaid = (double)dtl.SumApplied;
                            pay.AccountPayments.Decription = dtl.Description;
                            pay.AccountPayments.Add();
                        }
                    }
                  
                    var returnValue = pay.Add();
                    var docNum = 0;
                    if (returnValue == 0) docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());
                    else throw new ApplicationException(sap.Company.GetLastErrorDescription());

                    using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                    {
                        var list = new List<PaymentHeaderView>{data.Header};
                        var storedProc = "spOutgoingPayment";
                        var parameters = new
                        {
                            mode = "POST_PAYMENT",
                            docnum = docNum,
                            empid = empCode,
                            opdata = list.ToDataTable(),
                            accountdata = data.Accounts.ToDataTable(),
                            invoicedata = data.Invoices.ToDataTable()

                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    }

                    sap.Commit();

                    //OLD SP

                    using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
                    {
                        var storedProc = "spOPPost";
                        var parameters = new
                        {
                            opnum = docNum,
                            payee = data.Header.CWPayee,
                            chkRmrks = "",
                            chkprint = data.Header.CheckPrintMode,
                            EmpID = empCode,
                            PMStat= data.Header.PaymentType,
                            CAOAres = data.Header.OAReason,
                            BillNo = data.Header.F2307Bill
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
        public void UpdatePayment(PaymentHeaderView payment) {
            
            using (var sap = new SAPBusinessOne()) {
                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;
                    pay.GetByKey(payment.DocNum);
                   
                    sap.BeginTran();

                    //UPDATE OP
                    pay.UserFields.Fields.Item("U_APDocNo").Value = payment.U_APDocNo;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = payment.U_ChkNum;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = payment.U_HPDVoucherNo;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = payment.U_BranchCode;
                    pay.UserFields.Fields.Item("Comments").Value = payment.Comments;

                    var returnValue = pay.Update();
                    if (returnValue != 0) { 
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }
                    sap.Commit();

                    using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                    {
                        var list = new List<PaymentHeaderView> { payment };
                        var storedProc = "spOutgoingPayment";
                        var parameters = new
                        {
                            mode = "UPDATE_PAYMENT",
                            docnum = payment.DocNum,
                            opdata = list.ToDataTable()
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    }

                }
                catch (Exception ex)
                {
                    sap.Rollback();

                    LogError(new PaymentsErrorLogs
                    {
                        Module = "UPDATE PAYMENT",
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
                    var data = GetPaymentByDocNum(docNum);

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

        private PaymentView GetPaymentByDocNum(int docNum)
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
        private PaymentView GetPaymentData(PaymentView payment)
        {
            var output = new PaymentView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {

                var list = new List<PaymentHeaderView> { payment.Header };
                using (var multi = cn.QueryMultiple
                (
                    "spOutgoingPayment",
                    new
                    {
                        mode = "GET_PAYMENT_DATA",
                        opdata = list.ToDataTable(),
                        accountdata = payment.Accounts.ToDataTable(),
                        invoicedata = payment.Invoices.ToDataTable()
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Header = multi.ReadFirst<PaymentHeaderView>();
                    output.Invoices = multi.Read<PaymentInvoiceView>();
                    output.Accounts = multi.Read<PaymentAccountView>();
                    output.Checks = multi.Read<PaymentCheckView>();
                    return output;
                }
            }
        }
        private string GetVoucher(string branchCode,DateTime docDate)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "HPCOMMON..spOPGetVOucher";
                var parameters = new
                {
                    whs = branchCode,
                    vdate = docDate
                };
                return cn.ExecuteScalar(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToString();
            }
        }
        private void LogError(PaymentsErrorLogs log)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
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
