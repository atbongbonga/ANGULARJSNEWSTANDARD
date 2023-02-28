using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    internal class SAPPaymentRequest
    {
        public string Address { get; set; }
        public string JournalRemarks { get; set; }
        public DateOnly DocDate { get; set; }  
        public DateOnly DueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocType { get; set; }
        public string DocCurrency { get; set; }
        public string Remarks { get; set; }
        public DateOnly TaxDate { get; set; }
        public string U_ChkNum { get; set; }
        public string U_CardCode { get; set; }
        public string U_BranchCode { get; set; }
        public string U_HPDVoucherNo { get; set; }
        public int APDocNo { get; set; }
        public string Reference2 { get; set; }
        public int BankCode { get; set; }
        public int BranchCode { get; set; }
        public int Whscode { get; set; }

    }
}
