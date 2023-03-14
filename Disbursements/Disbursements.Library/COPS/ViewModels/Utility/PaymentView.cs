using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Utility
{
    public class PaymentView
    {
        public PaymentHeaderView Header { get; set; }
        public IEnumerable<PaymentAccountView> Accounts { get; set; }
        public IEnumerable<PaymentCheckView>? Checks { get; set; }
        public IEnumerable<PaymentJEView>? JournalEntries { get; set; }

    }
}
