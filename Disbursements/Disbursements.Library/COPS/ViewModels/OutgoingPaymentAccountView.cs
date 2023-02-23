using Core.Library.Models;
using Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public  class OutgoingPaymentAccountView
    {
        public Payment Header { get; set; }
        public IEnumerable<PaymentAccount>? Accounts { get; set; }

        public string PaymentMeans { get; set; }
    }
}
