using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.PCF.Models
{
    public class PCFPostTemplateModel
    {
    }

    public class PCFPaymentChecks
    {
        public string Branch { get; set; }
        public string AccountNo { get; set; }
        public DateTime DueDate { get; set; }
        public string CountryCode { get; set; }
        public string BankCode { get; set; }
        public string CheckAccount { get; set; }
        public decimal Amount { get; set; }
    }

    public class PCFInputsDetail { 
        public string SAP { get; set; }
        public string AcctCode { get; set; }
        public string WhsCode { get; set; }
        public string ATCCode { get; set; }
        public double WTax { get; set; }
        public double Amt { get; set; }
        public string Descr { get; set; }
    
    }

    public class PCFInputsHeader
    {
        public string Payee { get; set; }
        public string Addr { get; set; }
        public string Remarks { get; set; }
        public DateTime DocDate { get; set; }
        public string ChkNum { get; set; }
        public string BranchCode { get; set; }
        public string WhsCode { get; set; }
        public string Bank { get; set; }
        public decimal Total { get; set; }
        public decimal TaxAmount { get; set; }
        public string PostBy { get; set; }
    }


    public class PCFHeaderPostTemplate { 
        public string Payee { get; set; }
        public string Addr { get; set; }
        public string Remarks { get; set; }
        public DateTime DocDate { get; set;}
        public string ChkNum { get; set; }
        public string BranchCode { get; set; }
        public string WhsCode { get; set; }
        public string Bank { get; set; }
        public decimal SummApplied { get; set; }
        public string VoucherNo { get; set; }
        public string CheckAccount { get; set; }
        public string PostBy { get; set; }

    }

  


}
