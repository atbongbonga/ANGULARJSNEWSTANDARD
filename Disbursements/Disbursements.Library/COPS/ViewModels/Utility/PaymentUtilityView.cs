using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Utility
{
    public class PaymentUtilityView
    {
        public PaymentUtilityHeaderView Header { get; set; }
        public IEnumerable<PaymentUtilityAccountView> Accounts { get; set; }
        public IEnumerable<PaymentUtilityCheckView>? Checks { get; set; }
        public IEnumerable<PaymentUtilityCCView>? CreditCards { get; set; }
        public IEnumerable<PaymentUtilityJEView>? JournalEntries { get; set; }

    }
}
