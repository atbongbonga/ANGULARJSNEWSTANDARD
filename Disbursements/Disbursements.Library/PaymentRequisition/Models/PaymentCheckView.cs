using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentCheckView : PaymentCheck
    {
        public string CashAccount { get; set; }
        public string ControlAccount { get; set; }
    }
}
