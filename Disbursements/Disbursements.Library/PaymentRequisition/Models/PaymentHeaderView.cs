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
        public int Docentry { get; set; }
        public DateTime Docdate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Comments { get; set;}
        public string JrnlMemo { get; set; }
        public string Address { get; set; }
        public string U_ChkNum { get; set; }
        public string U_HPDVoucherNo { get; set; }
        public string U_BranchCode { get; set; }
        public string AcctCode { get; set; }
        public string AcctName { get; set; }
        public string PMode { get; set; }
        public string DocType { get; set;}

        public string PayNoDoc { get; set; }
        public decimal NoDocAmt { get; set; }
        public decimal Balance { get; set; }
    }
}
