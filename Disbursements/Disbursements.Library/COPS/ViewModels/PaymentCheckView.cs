using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public class PaymentCheckView
    {
        public DateTime DueDate { get; set; }
        public int CheckNum { get; set; }
        public string BankCode { get; set; }
        public string Branch { get; set; }
        public string AcctNum { get; set; }
        public string CheckAcct { get; set; }
        public decimal CheckAmt { get; set; }
    }
}
