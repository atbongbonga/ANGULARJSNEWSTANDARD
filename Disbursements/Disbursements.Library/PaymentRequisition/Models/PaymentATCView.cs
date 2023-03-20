using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentATCView
    {
        public int DocEntry { get; set; }
        public string ATCCode { get; set; } = string.Empty;
        public decimal ATCRate { get; set; }
        public string TaxGrp { get; set; } = string.Empty;
        public decimal Gross { get; set; }
        public int ManualVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal NetVAT { get; set; }
        public decimal EWT { get; set; }
    }
}
