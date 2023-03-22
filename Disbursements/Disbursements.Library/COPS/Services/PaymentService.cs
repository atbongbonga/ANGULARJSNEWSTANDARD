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
        private readonly PaymentRepository repo;
        private readonly string userCode;
        public PaymentService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new PaymentRepository(this.userCode);
        }
        public void PostPayment(PaymentView payment)
        {
            repo.PostPayment(payment);
        }

        public void UpdatePayment(PaymentHeaderView payment)
        {
            repo.UpdatePayment(payment);   
        }
        
        public void CancelPayment(int docNum) => repo.CancelPayment(docNum);
    }
}
