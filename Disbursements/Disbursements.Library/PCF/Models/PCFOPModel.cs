using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.PCF.Models
{
    
    public class PCFUserInputHeader {
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
    public class PCFUserInputDetail
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
    public class PCFPostDuetoAdvanceAccounts
    {
        public string Branch { get; set; }
        public string AccountNo { get; set; }
        public DateTime DueDate { get; set; }
        public string CountryCode { get; set; }
        public string BankCode { get; set; }
        public string CheckAccount { get; set; }
        public decimal Amount { get; set; }
    }
    public class PCFStoredProcTableDetail
    {
        public string SAP { get; set; }
        public string AcctCode { get; set; }
        public string WhsCode { get; set; }
        public string ATCCode { get; set; }
        public double WTax { get; set; }
        public double Amt { get; set; }
        public string Descr { get; set; }

    }
    public class PCFStoredProcTableHeader
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
    public class PCFPostingHeaderTemplate
    {
        public string Payee { get; set; }
        public string Addr { get; set; }
        public string Remarks { get; set; }
        public DateTime DocDate { get; set; }
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
