using AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories;
using Disbursements.Library.COPS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Services
{
    public class PaymentService
    {
        private PaymentRepository repo = new PaymentRepository();
        public PaymentService(string userCode = "")
        {
            repo = new PaymentRepository(userCode);
        }
        public void PostPayment(int docEntry, int sapEntry, string cardCode)
        {
            try
            {
                repo.PostPayment(docEntry, sapEntry, cardCode);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.GetBaseException().ToString());
            }

        }


    }
}
