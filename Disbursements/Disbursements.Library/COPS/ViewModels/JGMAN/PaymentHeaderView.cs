using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.JGMAN
{
    public class PaymentHeaderView : Payment
    {
        public string AcctCode { get; set; }
        public string PMode { get; set; }
        public decimal Balance { get; set; }
    }
}
