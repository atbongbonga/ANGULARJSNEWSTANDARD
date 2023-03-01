using AccountingLegacy.Core.Library;
using Disbursements.Library.COPS.ViewModels.JGMAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Repositories
{
    internal class PaymentRepository
    {
        private readonly string userCode;
        public PaymentRepository(string userCode = "")
        {
            this.userCode = userCode;
        }

        public void PostPayment(PaymentView payment)
        {
            try
            {
                using (var sap = new SAPBusinessOne("172.30.1.167"))
                {
                    var pay = sap.VendorPayments;
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
    }
}
