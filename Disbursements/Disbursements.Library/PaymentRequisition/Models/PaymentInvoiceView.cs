using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentInvoiceView : PaymentInvoice
    {
        public string ATC { get; set; } = string.Empty;
        public decimal EWT { get; set; }
        public int TransIDEWT { get; set; }
        public string WhsCode { get; set; }
    }

}
