using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class PaymentCreditCard
    {
       public string CreditAcct { get; set; }
       public int CreditCard { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime ChkDate { get; set; }
       public string VoucherNum { get; set; }
       public decimal CreditSum { get; set; }

    }
}
