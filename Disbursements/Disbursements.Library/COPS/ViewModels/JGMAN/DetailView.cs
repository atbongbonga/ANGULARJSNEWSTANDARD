using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.JGMAN
{
    public class DetailView
    {
        public int Id { get; set; }
        public string GenId { get; set; }
        public int LineId { get; set; }
        public int DocNum { get; set; }
        public string CorpName { get; set; }
        public string BrCode { get; set; }
        public string AcctType { get; set; }
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public decimal GrossAmt { get; set; }
        public decimal VatAmt { get; set; }
        public decimal NetAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public string TaxGroup { get; set; }
        public string ATC { get; set; }
        public double Rate { get; set; }
        public string EmpName { get; set; }
        public string Remarks { get; set; }
    }
}
