using Core.Library.Models;
using Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public  class PaymentAccountView : PaymentAccount
    {
        public string GLAcct { get; set; }
        public string BrCode { get; set; }
        public int ManualCheck { get; set; }
        public decimal NetVat { get; set; }
        public decimal Vat { get; set; }
        public string  TaxGroup { get; set; }
        public string ATC{ get; set; }
        public decimal Rate { get; set; }
        public decimal EWT { get; set; }

    }
}
