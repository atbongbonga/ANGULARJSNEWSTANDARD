using AccountingLegacy;
using AccountingLegacy.Core.Library;
using Core.Library.Models;
using Disbursements.Library.COPS.ViewModels.JGMAN;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Repositories
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
                //ERROR LOGS
                throw ex;
            }
        }

        private PaymentView GetPayment(int docEntry, int sapEntry, string cardCode)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_PF))
            {
                return cn.Query<JrnlEntryDetail>(
                    "spProfFees",
                    new
                    {
                        mode = "CA_TEMPLATE",
                        docEntry = docEntry
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
