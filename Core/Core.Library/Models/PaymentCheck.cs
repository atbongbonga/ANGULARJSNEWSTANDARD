using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class PaymentCheck
    {
        public DateTime DueDate { get; set; }
        public int CheckNum { get; set; }
        public string BankCode { get; set; }
        public string Branch { get; set; }
        public string AcctNum { get; set; }
        public decimal CheckAmt { get; set; }
        public string CheckAct { get; set; }
        public int CheckAbs { get; set; }
        public string ManualChk { get; set; }
    }
}


