using AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories;
using Disbursements.Library.PaymentRequisition.Models;

namespace Disbursements.Library.PaymentRequisition.Services
{
    public class PaymentService
    {
        private PaymentRepository repo = new PaymentRepository();
        private readonly string userCode;
        public PaymentService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories.PaymentRepository(userCode);
        }
        public void PostPayment(PaymentView Model)
        {
            try
            {
                repo.PostPayment(Model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.GetBaseException().ToString());
            }

        }


    }
}
