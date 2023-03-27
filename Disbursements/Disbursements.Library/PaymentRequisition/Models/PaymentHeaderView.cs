using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentHeaderView : Payment
    {
        public string WhsCode { get; set; } = string.Empty;
        public int Docentry { get; set; }
        public int Sapentry { get; set; }
        public string PRNumber { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool UseDueDate { get; set; }
        public string Payee { get; set; } = string.Empty;
        public string AcctCode { get; set; } = string.Empty;
        public string AcctName { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string PaymentMeans { get; set; } = string.Empty;
        public string ATC { get; set; } = string.Empty;
        public string ATC2 { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public decimal EWTAmount { get; set; }
        public decimal EWTAmount2 { get; set; }
        public decimal NetAmount { get; set; }
        public string CheckPrint { get; set; } = string.Empty;
        public string CheckRemarks { get; set; } = string.Empty;
        public bool PayOnAccount { get; set; }
        public string ControlAccount { get; set; } = string.Empty;
        public decimal AccountAmount { get; set; }
        public string CWPayee { get; set; } = string.Empty;
        public string AcctType { get; set; } = string.Empty;

    }
}
