using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class Payment
    {
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string DocType { get; set; }
        public string PayNoDoc { get; set; }
        public decimal NoDocAmt { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public decimal DocTotal { get; set; }
        public decimal CashAmt { get; set; }
        public decimal CreditAmt { get; set; }
        public decimal CheckAmt { get; set; }
        public string CheckAcct { get; set; }
        public decimal TransferAmt { get; set; }
        public string TransferAcct { get; set; }
        public DateTime? TransferDate { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Comments { get; set; }
        public string JrnlMemo { get; set; }
    }
}
