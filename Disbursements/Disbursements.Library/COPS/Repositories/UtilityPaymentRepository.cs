using AccountingLegacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MoreLinq;
using SAPbobsCOM;
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

namespace Disbursements.Library.COPS.Repositories
{
    internal class UtilityPaymentRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public UtilityPaymentRepository()
        {
            server = new SERVER("Outgoing Payment");
            this.empCode = empCode;
        }

          private PaymentView GetPaymentData(PaymentView payment)
        {
            var output = new PaymentView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {

                using (var multi = cn.QueryMultiple
                (
                    "spUtilityPayment",
                    new
                    {
                        mode = "GET_UTILITY_PAYMENT_DATA",
                        opdata = payment.Header,
                        accountdata = payment.Accounts.ToDataTable(),
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Header = (PaymentHeaderView)multi.Read<PaymentHeaderView>();
                    output.Accounts = multi.Read<PaymentAccountView>();
                    return output;
                }
            }
        }
        public void PostPayment(PaymentView payment)
        {
            var data = GetPaymentData(payment);

            using (var sap = new SAPBusinessOne("172.30.1.167"))
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();
                    pay.CardName = data.Header.CardName;
                    pay.Address = data.Header.PayTo;
                    pay.JournalRemarks = data.Header.CardName;
                    pay.DocDate = data.Header.DocDate;
                    pay.DueDate = data.Header.DocDate;
                    pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                    pay.Remarks = data.Header.Comments;
                    pay.UserFields.Fields.Item("U_Checknum").Value = data.Header.U_ChkNum is null ? "" : data.Header.U_ChkNum;
                    pay.UserFields.Fields.Item("U_CardCode").Value = data.Header.CardCode;
                    pay.UserFields.Fields.Item("U_BranchCode").Value = data.Header.U_BranchCode;
                    pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = GetVoucher(data.Header.U_BranchCode, data.Header.DocDate);
                    pay.Reference2 = Convert.ToString(data.UtilDocEntry);


                    sap.Commit();
                }
                catch (Exception ex)
                {
                    sap.Rollback();

                    LogError(new PaymentsErrorLogs
                    {
                        Module = "UTILITY PAYMENT",
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
