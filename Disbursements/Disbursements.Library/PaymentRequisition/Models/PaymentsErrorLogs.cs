using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    internal class PaymentsErrorLogs
    {   public int ErrorId { get; set; }
        public DateTime ErrorDate { get; set; }
        public string ErrorMsg { get; set; }
        public string Module { get; set; }
        public int DocEntry { get; set; }
        public string Remarks { get; set; }
        public string PostedBy { get; set; }
    }
}
