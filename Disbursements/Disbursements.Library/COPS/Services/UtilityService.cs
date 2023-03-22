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
            repo.PostUtilityPayment(payment);
        }
    }
}
