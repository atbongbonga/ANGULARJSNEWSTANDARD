using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.COPS.Models
{

    public class OPPostingModel { 
    
    }
    public class OPHdr
    {
        public int DocEntry { get; set; }
        public string DocType { get; set; }
        public string WhsCode { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Addr { get; set; }
        public string Payee { get; set; }
        public string Bank { get; set; }
        public string Ref2 { get; set; }
        public string BranchCode { get; set; }
        public string ChkNum { get; set; }
        public string VoucherNo { get; set; }
        public string Remarks { get; set; }
        public string JERemarks { get; set; }
        public string CkhPrint { get; set; }
        public string ChkRmrks { get; set; }
        public double Total { get; set; }
        public string U_CardCode { get; set; }
        public string UserID { get; set; }
        public string PMStat { get; set; }
        public string PMDate { get; set; }

        public string PMode { get; set; }
        public int CAOARes { get; set; }
        public string FBillNo { get; set; }
        public int ConsolEWT { get; set; }
        public double EWTTotal { get; set; }

    }

    public class OPSup {
        public int InvDoc { get; set; }
        public int InvType { get; set; }
        public string GlAcct { get; set; }
        public double App { get; set; }
        public double EWT { get; set; }
        public int TransIDEWT { get; set; }
        public string ATC { get; set; }
    }

    public class OPAcct {
        public string Proj { get; set; }
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string WhsCode { get; set; }
        public string DocRemarks { get; set;}
        public double Amount { get; set; }
        public string TaxGrp { get; set; }
        public string ATC { get; set; }
        public double Rate { get; set; }
        public double EWT { get; set; }
        public int LineID { get; set; }
        public int ManualVAT { get; set; }
        public double VAT { get; set; }
        public double NetVAT { get; set; }
    }


}
