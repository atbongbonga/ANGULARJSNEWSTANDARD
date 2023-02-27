using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.PCF.Models
{
    public class PCFOPModel
    {
 
    }

    public class PCFHdr
    {
        public int DocEntry { get; set; }
        public int OPNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DueDate { get; set; }
        public string WhsCode { get; set; }
        public string CardName { get; set; }
        public string U_CardCode { get; set; }
        public string Addr { get; set; }
        public string Bank { get; set; }
        public string BranchCode { get; set; }
        public string ChkNum { get; set; }
        public string Remarks { get; set; }
        public Double Total { get; set; }
        public string PMeans { get; set; }
        public string UserID { get; set; }
    }
    public class PCFDtl
    {
        public string AcctCode { get; set; }
        public string WhsCode { get; set; }
        public string DocRemarks { get; set; }
        public Double Amount { get; set; }

    }
    public class PCFPayHeader {
        public int DocEntry { get; set; }
        public int OPNum { get; set; }
        public string PType { get; set; }
        public string Payee { get; set; }
        public string Addr { get; set; }
        public DateTime DocDate { get; set; }
        public string WhsCode { get; set; }
        public string Bank { get; set; }
        public string BankName { get; set; }

        public double Total { get; set; }
        public double EWTTotal { get; set; }
        public string BranchCode { get; set; }
        public string Voucher { get; set; }
        public string Remarks { get; set; }
        public string ChkNum { get; set; }
        public string PostBy { get; set; }
        public string Custodian { get; set; }
    }
    public class PCFPayDetail
    {
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public int BREntry { get; set; }

        public int LineNum { get; set; }
        public double Amt { get; set; }
        public string Stat { get; set; }
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
        public int MnlVAT { get; set; }
        public double VAT { get; set; }
        public double NetVAT { get; set; }
        public string TaxGrp { get; set; }
        public double ATCRate { get; set; }
        public double WTax { get; set; }



    }
   

}
