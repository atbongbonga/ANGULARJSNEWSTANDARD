using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class PaymentAccount
    {
        public int DocNum { get; set; }
        public int LineId { get; set; }
        public string AcctCode { get; set; }
        public decimal SumApplied { get; set; }
        public string Description { get; set; }
        public string U_EmpID { get; set; }
        public string U_DocLine { get; set; }
    }
}
