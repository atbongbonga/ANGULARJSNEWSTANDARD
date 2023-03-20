using AccountingLegacy.Core.Library;
using Dapper;
using Disbursements.Library.PaymentRequisition.Models;
using MoreLinq;
using System.Data;
using System.Data.SqlClient;

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
                //InsertRequestPayment(header.Docentry, header.CardCode);
                var sapEntry = InsertRequestPayment(Model);
                var payment = GetTemplate(Model);
                using (var sap = new SAPBusinessOne("172.30.1.167"))
                {
                    var pay = sap.VendorPayments;

                    sap.BeginTran();
                    if (payment.Header.DocType.Equals("A"))
                    { pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount; }
                    else {
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
                    pay.UserFields.Fields.Item("U_APDocNo").Value = payment.Header.U_APDocNo;
                    pay.Reference2 = "R" + sapEntry.ToString();

                    //Accounts
                    if (payment.Accounts.Count() > 0)
                    {
                        foreach (var item in payment.Accounts)
                        {
                            pay.AccountPayments.AccountCode = item.AcctCode;
                            pay.AccountPayments.SumPaid = Convert.ToDouble(item.SumApplied);
                            pay.AccountPayments.Decription = item.Description;
                            pay.AccountPayments.Add();

                            if (item.EWT is not decimal.Zero)
                            {
                                pay.AccountPayments.AccountCode = item.AcctCode;
                                pay.AccountPayments.SumPaid = Convert.ToDouble(item.EWT);
                                pay.AccountPayments.Decription = item.Description;
                                pay.AccountPayments.Add();

                            }
                        }
                    }

                    // Set control and cash account for Payment on Acct Transaction.
                    if (payment.Header.PayOnAccount)
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
                    if (payment.Checks.Count() > 0)
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
                    if (payment.CreditCards.Count() > 0)
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
                    if (payment.Invoices.Count() > 0)
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
                        sap.Commit();
                        PostPaymentRequest(sapEntry ,docNum, payment);
                    }
                    else
                    {
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.GetBaseException().ToString());
            }
        }

        private PaymentView GetTemplate(PaymentView paramModel)
        {
            PaymentView model = new PaymentView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                try
                {
                    var reader = cn.QueryMultiple("spPaymentRequisition",
                                 param: new
                                 {
                                     Mode = "PAYMENT_TEMPLATE",
                                     cardCode = paramModel.Header.CardCode,
                                     docEntry = paramModel.Header.Docentry,
                                     sapEntry = paramModel.Header.Sapentry,
                                     payOnAcct = paramModel.Header.PayOnAccount,
                                     controlAcct = paramModel.Header.ControlAccount ,
                                     docTotal = paramModel.Header.DocTotal,
                                     ewtAmt = paramModel.Header.EWTAmount,
                                     ewtAmt2 = paramModel.Header.EWTAmount2,
                                     UDTPaymentRequestAccount = paramModel.Accounts.ToDataTable(),
                                     UDTPaymentRequestInvoice = paramModel.Invoices.ToDataTable(),
                                     UDTPaymentRequestATC = paramModel.PaymentATC.ToDataTable()
                                 }, commandType: CommandType.StoredProcedure); ; ;
                    model.Header = reader.Read<PaymentHeaderView>().Single();
                    model.Accounts = reader.Read<PaymentAccountView>().ToList();
                    model.Checks = reader.Read<PaymentCheckView>().ToList();
                    model.CreditCards = reader.Read<PaymentCreditCardView>().ToList();
                    model.Invoices = reader.Read<PaymentInvoiceView>().ToList();
                    return model;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.GetBaseException().ToString());
                }

            }

        }
        public int InsertRequestPayment(PaymentView model)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var oTransaction = cn.BeginTransaction();
                try
                {
                    var storedProc = "spPaymentRequisition";
                    var parameters = new
                    {
                        Mode = "INSERT_REQUEST_PAYMENT",
                        docEntry = model.Header.Docentry,
                        cardCode = model.Header.CardCode,
                        bankCode = model.Header.BankCode,
                        bankName = model.Header.BankName,
                        address = model.Header.Address,
                        whsCode = model.Header.WhsCode,
                        branchCode = model.Header.U_BranchCode,
                        userID = this.userCode,
                        docType = model.Header.DocType,
                        apDocNo = model.Header.U_APDocNo,
                        checkPrint = model.Header.CheckPrint,
                        checkRemarks = model.Header.CheckRemarks,
                        cwPayee = model.Header.CWPayee

                    };
                    var prDocentry = Convert.ToInt32(cn.ExecuteScalar(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0));
                    oTransaction.Commit();
                    return prDocentry;
                }
                catch (Exception)
                {
                    oTransaction.Rollback();
                    throw;
                }
            }
        }
        public void PostPaymentRequest(int sapEntry , int docNum, PaymentView payment)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
            {
           
                try
                {
                    var storedProc = "spPaymentRequisition";
                    var parameters = new
                    {
                        mode = "POST_PAYMENT_REQUEST",
                        opNum = docNum,
                        docEntry = payment.Header.Docentry,
                        sapEntry = sapEntry,
                        ewtAmt = payment.Header.EWTAmount,
                        pmode = payment.Header.PaymentMode,
                        atc = payment.Header.ATC,
                        atc2 = payment.Header.ATC2,
                        UDTPaymentRequestAccount = payment.Accounts.ToDataTable(),
                        atcCount = payment.PaymentATC.Count()

                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    storedProc = "spOPPost";
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
                    throw new ApplicationException(ex.GetBaseException().ToString());
                }
            }
        }



    }
}
