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

namespace Disbursements.Library.COPS.Repositories
{
    internal class PaymentsRepository
    {
        private readonly SERVER server;

        public PaymentsRepository()
        {
            server = new SERVER("Outgoing Payment");

        }
        public IEnumerable<OutgoingPaymentAccountView> GetPayables(string cardCode)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spOutgoingPayment";
                var parameters = new
                {
                    mode = "GET_OUTGOING_PAYMENT_ACCOUNT_DATA",
                    cardcode = cardCode
                };
                return cn.Query<OutgoingPaymentAccountView>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
        public IEnumerable<OutgoingPaymentAccountView> GetAccounts(int docNum) {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spOutgoingPayment";
                var parameters = new
                {
                    mode = "GET_PAYMENT_ACCOUNT",
                    docnum = docNum
                };
                return cn.Query<OutgoingPaymentAccountView>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
        public void PostAccountPayment(OutgoingPaymentAccountView payment) 
        {

            using (var sap = new SAPBusinessOne())
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();

                    //POST OP

                    //POST JE

                    //LOGS

                    sap.Commit();
                }
                catch (Exception ex)
                {
                    sap.Rollback();
                    //ERROR LOGS
                    throw new ApplicationException(ex.Message);
                }
            }

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spPaymentAccount";
                var parameters = new
                {
                    mode = "SAVE_PAYMENT_ACCOUNT",
                    opdata = payment.Header,
                    accountdata = payment.Accounts.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void PostInvoicePayment()
        {
            using(var sap = new SAPBusinessOne())
            {

                try
                {
                    var pay = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();

                    
                    //POST OP

                    //POST JE

                    //LOGS

                    sap.Commit();
                }
                catch (Exception ex)
                {
                    sap.Rollback();
                    //ERROR LOGS
                    throw new ApplicationException(ex.Message);
                }
            }

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spPaymentAccount";
                var parameters = new
                {
                    mode = "SAVE_PAYMENT_ACCOUNT",
                    opdata = payment.Header,
                    accountdata = payment.Accounts.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void CancelPayment(int docNum)
        {
            //CANCEL OP
            //CANCEL JE IF INVOICE
        }

        public void SaveOutgoingPaymentAccount(IEnumerable<OutgoingPaymentAccountView> outgoingPayments)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "";
                var parameters = new
                {
                    mode = "SAVE_OUTGOING_PAYMENT_ACCOUNT",
                    data = outgoingPayments.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }

}
