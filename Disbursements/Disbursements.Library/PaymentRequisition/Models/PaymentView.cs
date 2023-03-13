using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentView 
    {
        public PaymentHeaderView Header { get; set; }
        public IEnumerable<PaymentAccountView> Accounts { get; set; }
        public IEnumerable<PaymentCheckView> Checks { get; set; }
        public IEnumerable<PaymentCreditCardView> CreditCards { get; set; }
        public IEnumerable<PaymentInvoiceView> Invoices { get; set; }
        public IEnumerable<PaymentATCView> PaymentATC { get; set; }
    }
}
