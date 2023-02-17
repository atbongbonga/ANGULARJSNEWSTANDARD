using Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels
{
    public  class OutgoingPaymentView : OutgoingPaymentHeaderModel
    {
        public IEnumerable<OutgoingPaymentAccountModel>? OutgoingPaymentAccounts { get; set; }
        public IEnumerable<OutgoingPaymentSupplierModel>? OutgoingPaymentSuppliers { get; set; }
    }
}
