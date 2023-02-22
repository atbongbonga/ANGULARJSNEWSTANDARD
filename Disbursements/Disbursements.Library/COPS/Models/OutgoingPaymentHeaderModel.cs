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
        public int? DocEntry { get; set; }
        public string DocType { get; set; }
        public string WhsCode { get; set; }
        public DateTime DocDate{ get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string PaymentMeans { get; set; }
        public string CardCode { get; set; }
        public string? U_CardCode { get; set; }
        public string? Address { get; set; }
        public string? Payee { get; set; }
        public string? Reference { get; set; }
        public string? Bank { get; set; }
        public string? BranchCode { get; set; }
        public string? VoucherNo { get; set; }
        public string? CheckNum { get; set; }
        public string? F2307BillNo { get; set; }
        public string? Remarks { get; set; }
        public string? JERemarks { get; set; }
        public string? CheckPrintMode { get; set; }
        public string? PaymentMode { get; set; }
        public string? TaxGroup { get; set; }
        public string? ATC { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal? EWTTotal { get; set; }
        public decimal NetTotal { get; set; }

    }
}
