using Core.Library.Models;
using Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public  class OutgoingPaymentView : Payment
    {
        public IEnumerable<PaymentAccount>? OutgoingPaymentAccounts { get; set; }
        public IEnumerable<PaymentInvoice>? OutgoingPaymentInvoices { get; set; }
    }
}
