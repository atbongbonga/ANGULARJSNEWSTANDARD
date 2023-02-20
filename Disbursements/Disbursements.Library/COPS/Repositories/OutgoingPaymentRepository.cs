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

namespace Disbursements.Library.COPS.Repositories
{
    internal class OutgoingPaymentRepository
    {
        private readonly SERVER server;

        public OutgoingPaymentRepository()
        {
            server = new SERVER("Outgoing Payment");

        }
        public IEnumerable<OutgoingPaymentView> GetOutgoingPayments(int opid) {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "";
                var parameters = new
                {
                    mode = "GET_OUTGOING_PAYMENTS",
                    opid = opid
                };
                return cn.Query<OutgoingPaymentView>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
        public void PostOutgoingPayment(IEnumerable<OutgoingPaymentView> outgoingPayments) 
        {

            using (var sap = new SAPBusinessOne())
            {

                try
                {
                    var payment = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();


                    sap.Commit();
                }
                catch (Exception ex)
                {
                    sap.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "";
                var parameters = new
                {
                    mode = "POST_OUTGOING_PAYMENT",
                    data = outgoingPayments.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
        public void RemoveOutgoingPayment(IEnumerable<OutgoingPaymentView> outgoingPayments)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "";
                var parameters = new
                {
                    mode = "REMOVE_OUTGOING_PAYMENT",
                    data = outgoingPayments.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void SaveOutgoingPayment(IEnumerable<OutgoingPaymentView> outgoingPayments)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "";
                var parameters = new
                {
                    mode = "SAVE_OUTGOING_PAYMENT",
                    data = outgoingPayments.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }


    }

}
