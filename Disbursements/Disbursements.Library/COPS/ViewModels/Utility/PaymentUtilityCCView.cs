using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Utility
{
    public class PaymentUtilityCCView
    {
        public int DocNum { get; set; }
        public int CreditCard { get; set; }
        public string CreditAcct { get; set; }
        public decimal CreditAmt { get; set; }
    }
}
