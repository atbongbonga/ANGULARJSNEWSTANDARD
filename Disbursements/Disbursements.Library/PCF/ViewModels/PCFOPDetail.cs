using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class PCFOPDetail
    {
        public int DocNum { get; set; }
        public int OPNum { get; set; }
        
        public int Docentry { get; set; }
        public int BREntry { get; set; }

        public int LineNum { get; set; }
        public double Amt { get; set; }
        public string Stat {get; set; }
        public string SAP { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime HDRDateCreated { get; set; }
        public string Sales { get; set; }
        public double TotalAmt { get; set; }
        public double Change { get; set; }
        public string ReceiptNo { get; set; }
        public int PONum { get; set; }
        public int APInv { get; set; }
        public string Descr { get; set; }
        public string RType { get; set; }
        public DateTime DocDate { get; set; }
        public string PostBr { get; set; }
        public string Dept { get; set; }
        public string PaidTo { get; set; }
        public string WhsCode { get; set; }
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string PxName { get; set; }
        public string Items { get; set; }
        public string ORNO { get; set; }
        public string Labno { get; set; }
        public string SourceName { get; set; }
        public string ATCCode { get; set; }
        public string Custodian { get; set; }
        public int VSales { get; set; }
        public int VSApp { get; set; }
        public string? PCFType { get; set; }
        public int OLDLn { get; set; }
        public int MnlVAT { get; set; }
        public double VAT { get; set; }
        public double NetVAT { get; set; }
        public string TaxGrp { get; set; }
        public double ATCRate { get; set; }
        public double WTax { get; set; }
    }
}
