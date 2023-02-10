using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.COPS.Models
{
    public class AgenciesImportedModel
    {
        public int ImportID { get; set; }
        public DateTime ImportDate { get; set; }
        public string ImportBy { get; set; }
        public int DocNumCA { get; set; }
        public int DocNumOA { get; set; }
        public int GenID { get; set; }
        public int Yr { get; set; }
        public int Pd { get; set; }
        public int CutOff { get; set; }
        public bool Active { get; set; }
        public int LineNum { get; set; }
        public string BrCode { get; set; }
        public string BankType { get; set; }
        public string CardCode { get; set; }
        public string AcctCode { get; set; }
        public decimal GrossAmt { get; set; }
        public decimal NetAmt { get; set; }
        public decimal VatAmt { get; set; }
        public decimal EWTAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal OtherAmt { get; set; }
        public string CorpCode { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public int DeptCode { get; set; }
        public string ATC { get; set; }
        public decimal Rate { get; set; }
        public string Remarks { get; set; }
    }
}
