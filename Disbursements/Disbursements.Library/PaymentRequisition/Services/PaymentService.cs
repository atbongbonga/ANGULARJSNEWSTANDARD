using AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories;
using Disbursements.Library.PaymentRequisition.Models;
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
        private AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories.PaymentRepository repo = new AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories.PaymentRepository();
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
