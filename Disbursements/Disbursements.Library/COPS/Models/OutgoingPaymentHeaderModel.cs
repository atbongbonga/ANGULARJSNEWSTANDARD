using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class OutgoingPaymentHeaderModel
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
        public string APDocNo { get; set; }
        public string Remarks { get; set; }
        public string JERemarks { get; set; }
        public string ChkPrint { get; set; }
        public string ChkRmrks { get; set; }
        public decimal Total { get; set; }
        public string U_CardCode { get; set; }
        public string UserID { get; set; }
        public string PMStat { get; set; }
        public string PMDate { get; set; }
        public string PMode { get; set; }
        public int CAOARes { get; set; }
        public string FBillNo { get; set; }
        public int ConsolEWT { get; set; }
        public decimal EWTTotal { get; set; }

    }
}
