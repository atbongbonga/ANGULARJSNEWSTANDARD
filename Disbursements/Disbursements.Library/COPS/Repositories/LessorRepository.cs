using AccountingLegacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MoreLinq;
using SAPbobsCOM;
using Disbursements.Library.COPS.ViewModels.Lessor;
using Disbursements.Library.COPS.ViewModels.Utility;
using Disbursements.Library.COPS.Models;
using AccountingLegacy.Core.Library;
using System.Data.SqlClient;
using System.Data;

using Core.Library.Models;
using System.Reflection.PortableExecutable;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace Disbursements.Library.COPS.Repositories
{
    public class LessorRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public LessorRepository(string empCode = "")
        {
            server = new SERVER("Outgoing Payment");
            this.empCode = empCode;
        }

        private PaymentUtilityView GetPaymentData(PaymentUtilityView payment)
        {
            var output = new PaymentUtilityView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var opdata = new  List<PaymentUtilityHeaderView> { payment.Header};
                using (var multi = cn.QueryMultiple
                (
                    "spUtilityPayment",
                    new
                    {
                        mode = "GET_UTILITY_PAYMENT_DATA",
                        opdata = opdata.ToDataTable(),
                        accountdata = payment.Accounts.ToDataTable(),
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Header = multi.ReadFirst<PaymentUtilityHeaderView>();
                    output.Accounts = multi.Read<PaymentUtilityAccountView>();
                    output.Checks = multi.Read<PaymentUtilityCheckView>();
                    output.JournalEntries = multi.Read<PaymentUtilityJEView>();
                    return output;
                }
            }
        }
        public void PostLessorPayment(PaymentUtilityView payment)
        {
            var data = GetPaymentData(payment);

            using (var sap = new SAPBusinessOne())
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();
                    pay.CardName = data.Header.CardName;
                    pay.Address = data.Header.Address;
                    pay.JournalRemarks = data.Header.JrnlMemo;
                    pay.DocDate = data.Header.DocDate;
                    pay.DueDate = data.Header.DocDueDate;
                    //pay.TaxDate = data.Header.TaxDate;

                    pay.DocType = BoRcptTypes.rAccount;
                    pay.Remarks = data.Header.Comments;
                    pay.UserFields.Fields.Item("U_ChkNum").Value = data.Header.U_ChkNum is null ? "" : data.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = data.Header.CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = data.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = GetVoucher(data.Header.U_BranchCode, data.Header.DocDate);
                    pay.Reference2 = data.Header.Ref2;

                    if (data.Header.TransferAmt is not decimal.Zero)
                    {
                        pay.TransferAccount = data.Header.BankCode;
                        pay.TransferSum = (double)data.Header.DocTotal;
                        pay.TransferDate = data.Header.DocDate;
                        pay.TransferReference = Convert.ToString(data.Header.OPUtilDocEntry);
                        pay.PrimaryFormItems.PaymentMeans = SAPbobsCOM.PaymentMeansTypeEnum.pmtBankTransfer;
                    }
                    else if (data.Header.CreditAmt is not decimal.Zero) {
                        pay.CreditCards.CreditCard = (int)data.Header.CreditCard;
                        pay.CreditCards.CreditAcct = data.Header.CreditAcct;
                        pay.CreditCards.PaymentMethodCode = 1;
                        pay.CreditCards.CreditSum = (double)data.Header.CreditAmt;
                        pay.CreditCards.VoucherNum = "1";
                        pay.CreditCards.Add();
                    }

                    if (data.Header.PMode == "CHECKS") {
                        foreach (var item in data.Checks)
                        {
                            pay.Checks.Branch = item.Branch;
                            pay.Checks.AccounttNum = item.AcctNum;
                            pay.Checks.CountryCode = "PH";
                            pay.Checks.BankCode = item.BankCode;
                            pay.Checks.DueDate = item.DueDate;
                            pay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            pay.Checks.CheckAccount = item.CheckAcct;
                            pay.Checks.CheckSum = (double)item.CheckAmt;
                        }
                    }

                    foreach (var item in data.Accounts)
                    {
                        pay.AccountPayments.AccountCode = item.AcctCode;
                        pay.AccountPayments.SumPaid = (double)item.SumApplied;
                        pay.AccountPayments.Decription = item.Description;
                        //pay.AccountPayments.UserFields.Fields.Item("U_DocLine").Value = item.U_DocLine;
                        pay.AccountPayments.Add();
                    }

                    var returnValue = pay.Add();
                    var docNum = 0;
                    if (returnValue == 0) {
                        docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());
                    }
                    else
                    {
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }

                    if (data.Header.AdvDueJE == 1) { 
                        jrnlEntry = sap.JournalEntries;
                        jrnlEntry.Reference = "CV" + docNum;
                        jrnlEntry.TaxDate = data.Header.DocDate;
                        jrnlEntry.DueDate = data.Header.DocDueDate;
                        jrnlEntry.Memo = data.Header.Comments;

                        foreach (var item in data.JournalEntries)
                        {
                            jrnlEntry.Lines.AccountCode = item.AcctCode;
                            jrnlEntry.Lines.Debit = (double)item.Debit;
                            jrnlEntry.Lines.Credit = (double)item.Credit;
                            jrnlEntry.Lines.LineMemo = data.Header.Comments;
                            jrnlEntry.Lines.Reference1 = "CV" + docNum;
                            jrnlEntry.Lines.Add();
                        }
                        var JEReturnValue = jrnlEntry.Add();
                        if (JEReturnValue != 0) 
                        { 
                            var JETransid = Convert.ToInt32(sap.Company.GetNewObjectKey()); 
                        }
                        else 
                        {
                            throw new ApplicationException(sap.Company.GetLastErrorDescription());
                        }

                    }


                    using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                    {
                        var storedProc = "spUtilityPayment";
                        var opdata = new List<PaymentUtilityHeaderView> { data.Header };

                        var parameters = new
                        {
                            mode = "POST_UTILITY_PAYMENT",
                            docnum = docNum,
                            empid = empCode,
                            oputildocentry = data.Header.OPUtilDocEntry,
                            opdata = opdata.ToDataTable(),
                            accountdata = data.Accounts.ToDataTable()
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    }

                    sap.Commit();

                    //OLD SP UPDATE
                    //3.29
                    using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
                    {
                        var storedProc = "spOPPost";
                        var parameters = new
                        {
                            opnum = docNum,
                            payee = data.Header.CardName,
                            chkRmrks = data.Header.CheckRemarks,
                            chkprint = data.Header.CheckPrint,
                            EmpID = empCode,
                            CAOAres = data.Header.CAOARes,
                            BillNo = data.Header.FBillNo
                        };
                        cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        var storedProc3 = "spOPUtilJE";
                        var parameters3 = new
                        {
                            Docnum = docNum
                        };
                        cn.Execute(storedProc3, parameters3, commandType: CommandType.StoredProcedure, commandTimeout: 0);


                        if (data.Header.CardName == "PETRON CORPORATION")
                        {
                            var qry = "UPDATE PetronLogs SET OPNum = " + docNum + " WHERE DocEntry IN(SELECT MAX(DocEntry) FROM PetronLogs) AND ISNULL(OPNum,0)= 0";
                            cn.Execute(qry, "", commandType: CommandType.Text, commandTimeout: 0);
                        }
                    }

                    //10.51
                    if (data.Header.PCFDocNum != 0)
                    {
                        using (IDbConnection cn = new SqlConnection("192.171.10.51"))
                        {
                            var storedProc = "spOPUtil";
                            var parameters = new
                            {
                                pcfdocnum = data.Header.PCFDocNum,
                                oputildoc = data.Header.OPUtilDocEntry,

                            };
                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }
                    }
                    //END OLD SP

                }
                catch (Exception ex)
                {
                    sap.Rollback();

                    LogError(new PaymentsErrorLogs
                    {
                        Module = "LESSOR PAYMENT",
                        ErrorMsg = ex.GetBaseException().Message
                    });
                    throw new ApplicationException(ex.GetBaseException().Message);
                }
            }
        }

        public string GetVoucher(string branchCode, DateTime docDate)
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
