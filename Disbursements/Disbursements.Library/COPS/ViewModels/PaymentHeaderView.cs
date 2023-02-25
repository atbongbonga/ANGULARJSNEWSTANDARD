using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public class PaymentHeaderView : Payment
    {
        
        public string PMode { get; set; }
        public string BankCode { get; set; }
        public DateTime DueDate { get; set; }
        public string AcctCode { get; set; }
        public string VoucherNo { get; set; }
    }
}
