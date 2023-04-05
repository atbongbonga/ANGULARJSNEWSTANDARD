using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Lessor
{
    public class PaymentLessorView
    {
        public PaymentLessorHeaderView Header { get; set; }
        public IEnumerable<PaymentLessorAccountView> Accounts { get; set; }

    }
}
