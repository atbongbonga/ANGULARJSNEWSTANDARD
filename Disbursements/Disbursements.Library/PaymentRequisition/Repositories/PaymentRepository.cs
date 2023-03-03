using AccountingLegacy;
using AccountingLegacy.Core.Library;
using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disbursements.Library.PaymentRequisition.Models;
using Dapper;
using SAPbobsCOM;

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

                    if(payment.Header.DocType.Equals("A"))
                    {
                        pay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                        pay.CardCode = payment.Header.CardCode;
                    }
                    else pay.DocType = SAPbobsCOM.BoRcptTypes.rSupplier;

                    pay.CardName = payment.Header.CardName;
                    pay.DocDate = payment.Header.DocDate;
                    pay.DueDate = payment.Header.DocDueDate;

                    if (pay.Add() == 0)
                    {

                    } else
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

                                 }, commandType: CommandType.StoredProcedure);
                    model.Header = reader.Read<PaymentHeaderView>().Single();
                    model.Accounts = reader.Read<PaymentAccountView>().ToList();

                    return model;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.GetBaseException().ToString());
                }
               
            }
            
        }

        public void InsertRequestPayment(int docEntry, string cardCode) {

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
        
    }
}
