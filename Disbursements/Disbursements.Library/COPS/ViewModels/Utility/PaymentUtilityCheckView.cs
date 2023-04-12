using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Utility
{
    public class PaymentUtilityCheckView
    {
        public string Branch { get; set; }
        public string AcctNum { get; set; }
        public string BankCode { get; set; }
        public DateTime DueDate { get; set; }
        public string CheckAcct { get; set; }
        public decimal CheckAmt { get; set; }
    }
}
