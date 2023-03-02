using AccountingLegacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLegacy.Core.Library;
using Core.Library.Models;
using AccountingLegacy.Disbursements.Library.COPS.ViewModels;
using System.Data.SqlClient;
using System.Data;
using SAPbobsCOM;
using AccountingLegacy.Core.Library;
using AccountingLegacy.Disbursements.Library.Auth;
using AccountingLegacy.Disbursements.Library.COPS.Models;
using AccountingLegacy.Disbursements.Library.Interfaces.Repositories;
using Dapper;
using MoreLinq;
using Disbursements.Library.PaymentRequisition.Models;

namespace Disbursements.Library.PaymentRequisition.Repositories

{
    internal class PaymentRepository
    {
        private readonly SERVER server;
        public PaymentRepository()
        {
            server = new SERVER("Payment Posting");
        }
        public IEnumerable<DisbursementList> GetPaymentsAccount(string docEntry, string sapDoc)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spPRLoadPostDetails";
                var parameters = new
                {
                    docEntry = docEntry,
                    SapDoc = sapDoc
                    
                };
                return cn.Query<DisbursementList>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<SAPPaymentRequest> GetForPostingPayment(string docEntry, string sapDoc)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spPRLoadPostDetails";
                var parameters = new
                {
                    docEntry = docEntry,
                    SapDoc = sapDoc

                };
                return cn.Query<SAPPaymentRequest>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }





        public void PostPayment(IEnumerable<DisbursementList> requestpayments)
        {
            using (var sap = new SAPBusinessOne())
            {
                try
                {
                    sap.BeginTran();


                    sap.Commit();
                }catch(Exception ex)
                {
                    sap.Rollback();
                    throw ex;
                }
            }

            

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spPaymentRequisition";
                var parameters = new
                {
                    mode = "PAYMENT_POST",
                    data = requestpayments.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }



        }




    }
}
