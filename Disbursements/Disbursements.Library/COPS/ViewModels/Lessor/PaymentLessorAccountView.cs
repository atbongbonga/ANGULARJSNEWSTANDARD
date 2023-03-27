using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Lessor
{
    public class PaymentLessorAccountView : PaymentAccount
    {
        public string AcctName { get; set; }
        public string WhsCode { get; set; }
        public string TaxGroup { get; set; }
        public string ATC { get; set; }
        public decimal Rate { get; set; }
        public decimal Vat { get; set; }
        public decimal NetVAT { get; set; }
        public decimal WTAX { get; set; }
        public decimal EWT { get; set; }
        public int Proj { get; set; }
    }
}
