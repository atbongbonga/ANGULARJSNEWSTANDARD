using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disbursements.Library.COPS;
using Disbursements.Library.COPS.Repositories;
using Disbursements.Library.COPS.ViewModels;

namespace Disbursements.Library.COPS.Services
{
    public class PaymentService
    {
        private readonly PaymentsRepository repo;
        private readonly string userCode;
        public PaymentService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new PaymentsRepository(this.userCode);
        }
        public void PostPayment(PaymentView payment) => repo.PostPayment(payment);
        public void CancelPayment(int docNum) => repo.CancelPayment(docNum);
    }
}
