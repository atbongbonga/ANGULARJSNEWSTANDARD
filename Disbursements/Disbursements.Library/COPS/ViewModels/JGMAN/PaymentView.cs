using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.JGMAN
{
    public class PaymentView
    {
        public PaymentHeaderView Header { get; set; }
        public IEnumerable<PaymentAccountView> Accounts { get; set; }
    }
}
