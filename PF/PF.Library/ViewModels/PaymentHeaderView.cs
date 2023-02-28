using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF.Library.ViewModels
{
    public class PaymentHeaderView : Payment
    {
        public decimal BankAmt { get; set; }
        public string CorpCode { get; set; }
        public string AcctType { get; set; }
        public string PMode { get; set; }
        public string BankCode { get; set; }
    }
}
