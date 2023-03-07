using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    public class PaymentAccountView : PaymentAccount
    {
        public string ATC { get; set; }
        public decimal ATCRate { get; set; }
    }
}
