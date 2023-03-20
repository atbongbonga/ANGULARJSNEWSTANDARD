using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public class PaymentHeaderView : Payment
    {
        
        public string PMode { get; set; }
        public string WhsCode { get; set; }
        public string BankCode { get; set; }

        public DateTime DueDate { get; set; }
        public string AcctCode { get; set; }
        public string CWPayee { get; set; }
        public string F2307Bill { get; set; }
        public string CheckPrintMode { get; set; }
        public string PaymentType { get; set; }
        public string TaxGroup { get; set; }
        public string ATC { get; set; }
        public string OAReason { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalEWT { get; set; }
        public decimal TotalNet { get; set; }
        public string PostedBy { get; set; }
        public string CancelledBy { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime? LastUpdate { get; set; }

        public DateTime TaxDate { get; set; }

    }
}
