using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class PCFOPChecksView
    {
        public string WhsCode  { get; set; }
        public string AccounttNum { get; set; }
        public DateTime  DueDate { get; set; }
        public string BankCode { get; set; }
        public string CheckAccount { get; set; }
        public decimal CheckSum { get; set; }
    }
}
