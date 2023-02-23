using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public class OutgoingPaymentInvoiceView
    {
        public Payment Header { get; set; }
        public IEnumerable<PaymentInvoice>? Invoices { get; set; }

    }
}
