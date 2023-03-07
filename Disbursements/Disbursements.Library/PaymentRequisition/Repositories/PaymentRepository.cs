using AccountingLegacy.Core.Library;
using Dapper;
using Disbursements.Library.PaymentRequisition.Models;
using MoreLinq;
using SAPbobsCOM;
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

        public void PostPayment(int docEntry, int sapEntry, string cardCode)
        {
            try
            {
                InsertRequestPayment(docEntry, cardCode);
                var payment = GetPayment(docEntry, sapEntry, cardCode);
                using (var sap = new SAPBusinessOne("172.30.1.167"))
                {
                    var pay = sap.VendorPayments;

                    sap.BeginTran();
                    if (payment.Header.DocType.Equals("A"))
                    { pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount; }
                    else pay.DocType = SAPbobsCOM.BoRcptTypes.rSupplier;

                    pay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                    pay.Address = payment.Header.Address;
                    pay.JournalRemarks = payment.Header.JrnlMemo;
                    pay.CardCode = payment.Header.CardCode;
                    pay.CardName = payment.Header.CardName;
                    pay.DocDate = payment.Header.DocDate;
                    pay.DueDate = payment.Header.DocDueDate;
                    pay.DocCurrency = "PHP";
                    pay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    pay.TaxDate = payment.Header.DocDate;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = payment.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = payment.Header.CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = payment.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = payment.Header.U_HPDVoucherNo;


                    if (payment.Header.PMeans.Equals("BANK TRANSFER"))
                    {
                        pay.TransferAccount = payment.Header.AcctCode;
                        pay.TransferSum = Convert.ToDouble(payment.Header.Balance);
                        pay.TransferDate = payment.Header.DocDate;
                        pay.TransferReference = "";
                        pay.PrimaryFormItems.PaymentMeans = SAPbobsCOM.PaymentMeansTypeEnum.pmtBankTransfer;
                    }

                    //Checks
                    if (payment.Checks.Count() > 0)
                    {

                        foreach (var item in payment.Checks)
                        {
                            pay.Checks.Branch = item.Branch;
                            pay.Checks.AccounttNum = item.Branch;
                            pay.Checks.DueDate = item.DueDate;
                            pay.Checks.CountryCode = "PH";
                            pay.Checks.BankCode = item.BankCode;
                            pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            pay.Checks.CheckAccount = item.AcctNum;
                            pay.Checks.CheckSum = Convert.ToDouble(item.CheckAmt);
                            pay.Checks.Add();
                        }

                    }

                    //Accounts
                    if (payment.Accounts.Count() > 0)
                    {
                        foreach (var item in payment.Accounts)
                        {
                            pay.AccountPayments.AccountCode = item.AcctCode;
                            pay.AccountPayments.SumPaid = Convert.ToDouble(item.SumApplied);
                            pay.AccountPayments.Decription = item.Description;
                            pay.AccountPayments.Add();
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
                            pay.CreditCards.VoucherNum = item.DocDate.ToString("MM/dd/yyyy");
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
                            if (item.EWT is not decimal.Zero)
                            {
                                pay.Invoices.DocEntry = item.TransIDEWT;
                                pay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;
                                pay.Invoices.SumApplied = Convert.ToDouble(item.EWT);
                                pay.Invoices.Add();
                            }
                            else
                            {
                                pay.Invoices.DocEntry = item.DocEntry;
                                pay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_PurchaseInvoice;
                                pay.Invoices.SumApplied = Convert.ToDouble(item.SumApplied);
                                pay.Invoices.Add();
                            }
                        }
                    }

                    if (pay.Add() == 0)
                    {
                        var docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());
                        sap.Commit();
                        PostPaymentRequest(docNum, payment);
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

        private PaymentView GetPayment(int docEntry, int sapEntry, string cardCode)
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
                                     cardCode = cardCode,
                                     docEntry = docEntry,
                                     sapEntry = sapEntry
                                 }, commandType: CommandType.StoredProcedure);
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
        public void InsertRequestPayment(int docEntry, string cardCode)
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
                        docEntry = docEntry,
                        cardCode = cardCode

                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    oTransaction.Commit();
                }
                catch (Exception)
                {
                    oTransaction.Rollback();
                    throw;
                }


            }
        }
        public void PostPaymentRequest(int docNum, PaymentView payment)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
            {
                var oTransaction = cn.BeginTransaction();
                try
                {
                    var storedProc = "spPaymentRequisition";
                    var parameters = new
                    {
                        mode = "POST_PAYMENT_REQUEST",
                        opNum = docNum,
                        docEntry = payment.Header.Docentry,
                        sapEntry = 0,
                        ewtAmt = 0,
                        pmode = payment.Header.PMode,
                        atc = payment.Header.ATC,
                        atc2 = payment.Header.ATC2,
                        UDTPaymentRequest = payment.Accounts.ToDataTable()

                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    storedProc = "spOPPost";
                    var parameters2 = new
                    {
                      opnum = docNum ,
                      payee = "" ,
                      chkRmrks = "" ,
                      chkprint = "",
                      EmpID = ""
                    };
                    cn.Execute(storedProc, parameters2, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    oTransaction.Commit();
                }
                catch (Exception ex)
                {
                    oTransaction.Rollback();
                    throw new ApplicationException(ex.GetBaseException().ToString());
                }  
            }
        }



    }
}
