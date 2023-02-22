using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class OutgoingPaymentAccountModel
    {
        public string Proj { get; set; }
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string WhsCode { get; set; }
        public string DocRemarks { get; set; }
        public decimal Amount { get; set; }
        public string TaxGrp { get; set; }
        public string ATC { get; set; }
        public decimal Rate { get; set; }
        public decimal EWT { get; set; }
        public int LineID { get; set; }
        public int ManualVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal NetVAT { get; set; }
    }
}
