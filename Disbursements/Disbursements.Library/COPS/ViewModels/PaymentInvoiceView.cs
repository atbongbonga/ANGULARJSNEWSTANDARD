using Core.Library.Models;
using Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public class PaymentInvoiceView : PaymentInvoice
    {
        public decimal Balance { get; set; }
        public string BrCode { get; set; }
        public string GLAcct { get; set; }
        public string ATC { get; set; }
        public decimal EWT { get; set; }
        public string EWTTransId { get; set; }
    }
}
