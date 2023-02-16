using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class PaymentInvoice
    {
        public int DocNum { get; set; }
        public int InvoiceId { get; set; }
        public int DocEntry { get; set; }
        public int InvType { get; set; }
        public decimal SumApplied { get; set; }
        public int DocTransId { get; set; }

    }
}
