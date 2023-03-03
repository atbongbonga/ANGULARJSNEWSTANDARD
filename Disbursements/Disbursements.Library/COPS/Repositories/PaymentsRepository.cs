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
using System.Security.Cryptography;

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
        private PaymentView GetPaymentData(PaymentView payment)
        {
            var output = new PaymentView();
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {

                using (var multi = cn.QueryMultiple
                (
                    "spOutgoingPayment",
                    new
                    {
                        mode = "GET_PAYMENT_DATA",
                        opdata = payment.Header,
                        accountdata = payment.Accounts.ToDataTable(),
                        invoicedata = payment.Invoices.ToDataTable()
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Header = (PaymentHeaderView)multi.Read<PaymentHeaderView>();
                    output.Invoices = multi.Read<PaymentInvoiceView>();
                    output.Accounts = multi.Read<PaymentAccountView>();
                    output.Checks = multi.Read<PaymentCheckView>();
                    return output;
                }
            }
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
                    pay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                    pay.Address = data.Header.Address;
                    pay.JournalRemarks = data.Header.JrnlMemo;
                    pay.DocDate = data.Header.DocDate;

                    pay.CardCode = data.Header.CardCode;
                    pay.CardName = data.Header.CardName;
                    pay.DocType = data.Header.DocType == "S" ? SAPbobsCOM.BoRcptTypes.rSupplier : SAPbobsCOM.BoRcptTypes.rAccount;
                    pay.DocCurrency = "PHP";
                    pay.Remarks = data.Header.Comments;
                    pay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    pay.TaxDate = data.Header.DocDate;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = data.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = data.Header.U_CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = data.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = GetVoucher(data.Header.U_BranchCode, data.Header.DocDate);

                    //BANK TRANSFER
                    if (data.Header.TransferAmt is not decimal.Zero)
                    {
                        pay.TransferAccount = data.Header.BankCode;
                        pay.TransferSum = (double)data.Header.DocTotal;
                        pay.TransferDate = data.Header.DueDate;
                        pay.PrimaryFormItems.PaymentMeans = SAPbobsCOM.PaymentMeansTypeEnum.pmtBankTransfer;
                    }

                    //CHECKS
                    foreach (var item in data.Checks)
                    {
                        pay.Checks.Branch = item.AcctNum;
                        pay.Checks.AccounttNum = item.AcctNum;
                        pay.Checks.DueDate = item.DueDate;
                        pay.Checks.CountryCode = "PHP";
                        pay.Checks.BankCode = item.BankCode;
                        pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                        pay.Checks.CheckAccount = item.CheckAcct;
                        pay.Checks.CheckSum = (double)item.CheckAmt;
                        pay.Checks.Add();

                    }

                    if (data.Invoices.Count() > 0) {
                        foreach (var dtl in data.Invoices)
                        {
                            pay.Invoices.DocEntry = dtl.DocEntry;
                            pay.Invoices.InvoiceType = (SAPbobsCOM.BoRcptInvTypes)dtl.InvType;
                            pay.Invoices.SumApplied = (double)dtl.SumApplied;
                            pay.Invoices.Add();

                            if (dtl.EWT is not decimal.Zero) {
                                pay.Invoices.DocEntry = dtl.EWTTransId;
                                pay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;
                                pay.Invoices.SumApplied = -(double)dtl.EWT;
                                pay.Invoices.Add();

                            }


                        }
                    }
                    else if (data.Accounts.Count() > 0) {
                        foreach (var dtl in data.Accounts)
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
                            opdata = data.Header,
                            invoicedata = data.Invoices.ToDataTable()
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    }
                    //OLD SP
                    using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
                    {
                        var storedProc = "spOPPost";
                        var parameters = new
                        {
                            opnum = docNum,
                            payee = data.Header.CWPayee,
                            chkRmrks = data.Header.Comments,
                            chkprint = data.Header.CheckPrintMode,
                            EmpID = empCode,
                            PMStat= data.Header.PaymentType,
                            CAOAres = data.Header.OAReason,
                            FBillNo = data.Header.F2307Bill
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        if (data.Header.DocType == "A") {
                            foreach (var item in data.Accounts)
                            {
                                var storedProc = "spOPVPM4";
                                var parameters = new
                                {
                                    Docnum = docNum,
                                    lineId = item.LineId,
                                    Formatcode = item.AcctCode,
                                    acctname = "",
                                    sumapp = item.SumApplied,
                                    descrip = item.Description,
                                    whscode = item.BrCode,
                                    ATCCode = item.ATC,
                                    EWTAmt = item.EWT,
                                    AtcRate =item.Rate,
                                    TaxGrp =item.TaxGroup
                                };
                                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                            }
                            
                        }
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

        public string GetVoucher(string branchCode,DateTime docDate)
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
