using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disbursements.Library.COPS.Repositories;
using Disbursements.Library.COPS.ViewModels.Utility;
namespace Disbursements.Library.COPS.Services
{
    public class UtilityService
    {
        private readonly UtilityRepository repo;
        private readonly string userCode;
        public UtilityService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new UtilityRepository(this.userCode);
        }
        public void PostUtilityPayment(PaymentUtilityView payment)
        {

            if (payment is null || payment.Header is null) throw new ApplicationException("Data not found.");

            if (string.IsNullOrEmpty(payment.Header.WhsCode)) throw new ApplicationException("Payment branch is required.");

            if (string.IsNullOrEmpty(payment.Header.PMode)) throw new ApplicationException("Payment mode is required.");

            if (string.IsNullOrEmpty(payment.Header.BankCode)) throw new ApplicationException("Bank code is required.");

            if (string.IsNullOrEmpty(payment.Header.Comments)) throw new ApplicationException("Remarks is required.");

            repo.PostUtilityPayment(payment);
        }
    }
}
