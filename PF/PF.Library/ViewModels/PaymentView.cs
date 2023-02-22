using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF.Library.ViewModels
{
    public class PaymentView
    {
        public IEnumerable<Payment> Headers { get; set; }
        public IEnumerable<PaymentAccount> Details { get; set; }
    }
}
