using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentAccountView : PaymentAccount
    {
        public string AcctName { get; set; }
        public string WhsCode { get; set; }
        public string ATC { get; set; }
        public decimal ATCRate { get; set; }
        public decimal EWT { get; set; }
        public int ManualVAT { get; set; }
        public decimal NetVAT { get; set; }
        public decimal VAT { get; set; }
        public string TaxGroup { get; set; }

    }
}
