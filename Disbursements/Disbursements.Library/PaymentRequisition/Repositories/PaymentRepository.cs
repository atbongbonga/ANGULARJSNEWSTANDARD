using AccountingLegacy.Core.Library;
using Dapper;
using Disbursements.Library.PaymentRequisition.Models;
using MoreLinq;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories
{
    internal class PaymentRepository
    {
        private readonly string userCode;
        private readonly SERVER server;
        public PaymentRepository(string userCode = "")
        {
            this.userCode = userCode;
            this.server = new SERVER("PAYMENT REQUISITION");
        }

        public void PostPayment(PaymentView Model)
        {
            try
            {
                var sapEntry = InsertRequestPayment(Model);
                var payment = GetTemplate(Model);
                using (var sap = new SAPBusinessOne())
                {
                    var pay = sap.VendorPayments;

                    if (payment.Header.DocType.Equals("A"))
                    { pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount; }
                    else
                    {
                        pay.CardCode = payment.Header.CardCode;
                        pay.DocType = SAPbobsCOM.BoRcptTypes.rSupplier;
                    }

                    //Header
                    pay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                    pay.Address = payment.Header.Address;
                    pay.JournalRemarks = payment.Header.JrnlMemo;
                    pay.CardName = payment.Header.CardName;
                    pay.DocDate = payment.Header.DocDate;
                    pay.DueDate = payment.Header.DueDate;
                    pay.DocCurrency = "PHP";
                    pay.Remarks = payment.Header.Comments;
                    pay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    pay.TaxDate = payment.Header.DocDate;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = payment.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = payment.Header.CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = payment.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = payment.Header.U_HPDVoucherNo;
                    pay.UserFields.Fields.Item("U_APDocNo").Value = sapEntry.ToString();
                    pay.Reference2 = "R" + sapEntry.ToString();

                    //Accounts
                    if (payment.Accounts is not null && payment.Accounts.Count() > 0)
                    {
                        foreach (var item in payment.Accounts)
                        {
                            pay.AccountPayments.AccountCode = item.AcctCode;
                            pay.AccountPayments.SumPaid = Convert.ToDouble(item.SumApplied);
                            pay.AccountPayments.Decription = item.Description;
                            pay.AccountPayments.UserFields.Fields.Item("U_DocLine").Value = item.U_DocLine;
                            pay.AccountPayments.Add();
                        }
                    }

                    // Set control and cash account for Payment on Acct Transaction.
                    if (payment.Header.PayOnAccount && !payment.Header.PaymentMeans.Equals("BANK TRANSFER"))
                    {
                        pay.ControlAccount = payment.Header.ControlAccount;
                        pay.CashAccount = payment.Header.AcctCode;
                    }

                    //Bank Transfer
                    if (payment.Header.PaymentMeans.Equals("BANK TRANSFER"))
                    {
                        pay.TransferAccount = payment.Header.AcctCode;
                        pay.TransferSum = Convert.ToDouble(payment.Header.NetAmount);
                        pay.TransferDate = payment.Header.DocDate;
                        pay.TransferReference = sapEntry.ToString();
                        pay.PrimaryFormItems.PaymentMeans = SAPbobsCOM.PaymentMeansTypeEnum.pmtBankTransfer;
                    }

                    //Checks
                    if (payment.Checks is not null && payment.Checks.Count() > 0)
                    {

                        foreach (var item in payment.Checks)
                        {
                            pay.Checks.Branch = item.Branch;
                            pay.Checks.AccounttNum = item.Branch;
                            pay.Checks.DueDate = payment.Header.DueDate;
                            pay.Checks.CountryCode = "PH";
                            pay.Checks.BankCode = item.BankCode;
                            pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            pay.Checks.CheckAccount = item.CheckAct;
                            pay.Checks.CheckSum = Convert.ToDouble(item.CheckAmt);
                            pay.Checks.Add();
                        }

                    }

                    //Credit Card
                    if (payment.CreditCards is not null && payment.CreditCards.Count() > 0)
                    {
                        foreach (var item in payment.CreditCards)
                        {
                            pay.CreditCards.PaymentMethodCode = 1;
                            pay.CreditCards.CreditType = SAPbobsCOM.BoRcptCredTypes.cr_Regular;
                            pay.CreditCards.CreditSum = Convert.ToDouble(item.CreditSum);
                            pay.CreditCards.VoucherNum = item.DocDate.Date.ToString("MM/dd/yyyy");
                            pay.CreditCards.UserFields.Fields.Item("U_ChckDate").Value = item.ChkDate;
                            pay.CreditCards.CreditAcct = item.CreditAcct;
                            pay.CreditCards.CreditCard = item.CreditCard;
                            pay.CreditCards.Add();

                        }
                    }

                    //Invoice
                    if (payment.Invoices is not null && payment.Invoices.Count() > 0)
                    {
                        foreach (var item in payment.Invoices)
                        {
                            pay.Invoices.DocEntry = item.InvoiceId;
                            pay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_PurchaseInvoice;
                            pay.Invoices.SumApplied = Convert.ToDouble(item.SumApplied);
                            pay.Invoices.Add();

                            if (item.EWT is not decimal.Zero)
                            {
                                pay.Invoices.DocEntry = item.TransIDEWT;
                                pay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;
                                pay.Invoices.SumApplied = Convert.ToDouble(item.EWT);
                                pay.Invoices.Add();
                            }
                        }
                    }

                    if (pay.Add() == 0)
                    {
                        var docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());
                        PostPaymentRequest(sapEntry, docNum, Model);
                    }
                    else
                    {
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }
                }
            }
            catch (Exception ex)
            {

                ClearSapEntry(Model.Header.Docentry);
                LogError(new PaymentsErrorLogs
                {
                    Module = "PAYMENT REQUISITION-PAYMENT",
                    ErrorMsg = ex.GetBaseException().Message,
                    DocEntry = Model.Header.Docentry,
                    Remarks = "Payment Requisition Payment Posting Failed."
                });
                throw new ApplicationException(ex.GetBaseException().Message);
            }
        }

        private PaymentView GetTemplate(PaymentView paramModel)
        {
            PaymentView model = new PaymentView();
            List<PaymentHeaderView> Header = new List<PaymentHeaderView>(); Header.Add(paramModel.Header);
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                try
                {
                    var reader = cn.QueryMultiple("spPaymentRequisition",
                                 param: new
                                 {
                                     Mode = "PAYMENT_TEMPLATE",
                                     UDTPaymentRequestHeader = Header.ToDataTable(),
                                     UDTPaymentRequestAccount = paramModel.Accounts.ToDataTable(),
                                     UDTPaymentRequestInvoice = paramModel.Invoices.ToDataTable()
                                 }, commandType: CommandType.StoredProcedure); ; ;
                    model.Header = reader.Read<PaymentHeaderView>().FirstOrDefault();
                    model.Accounts = reader.Read<PaymentAccountView>().ToList();
                    model.Checks = reader.Read<PaymentCheckView>().ToList();
                    model.CreditCards = reader.Read<PaymentCreditCardView>().ToList();
                    model.Invoices = reader.Read<PaymentInvoiceView>().ToList();
                    model.PaymentATC = reader.Read<PaymentATCView>().ToList();
                    return model;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.GetBaseException().Message);
                }

            }

        }
        public int InsertRequestPayment(PaymentView model)
        {
            List<PaymentHeaderView> Header = new List<PaymentHeaderView>(); Header.Add(model.Header);
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                try
                {
                    var storedProc = "spPaymentRequisition";
                    var parameters = new
                    {
                        mode = "INSERT_REQUEST_PAYMENT",
                        userID = this.userCode,
                        UDTPaymentRequestHeader = Header.ToDataTable()

                    };
                    var prDocentry = cn.ExecuteScalar<Int32>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    return prDocentry;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.GetBaseException().Message);
                }
            }
        }
        public void PostPaymentRequest(int sapEntry, int docNum, PaymentView payment)
        {
            List<PaymentHeaderView> Header = new List<PaymentHeaderView>(); Header.Add(payment.Header);
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                try
                {
                    var storedProc = "spPaymentRequisition";
                    var parameters = new
                    {
                        mode = "POST_PAYMENT_REQUEST",
                        opNum = docNum,
                        sapEntry = sapEntry,
                        userID = this.userCode,
                        UDTPaymentRequestHeader = Header.ToDataTable(),
                        UDTPaymentRequestAccount = payment.Accounts.ToDataTable()
                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    storedProc = "HPCOMMON..spOPPost";
                    var parameters2 = new
                    {
                        opnum = docNum,
                        payee = payment.Header.Payee,
                        chkRmrks = payment.Header.CheckRemarks,
                        chkprint = payment.Header.CheckPrint,
                        EmpID = this.userCode
                    };
                    cn.Execute(storedProc, parameters2, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.GetBaseException().Message);
                }
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
                        empCode = this.userCode
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        private void ClearSapEntry(int prReqNo) {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                cn.Execute(
                    "spPaymentRequisition",
                    new
                    {
                        mode = "CLEAR_SAP_ENTRY",
                        prReqNo = prReqNo
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }


    }
}
