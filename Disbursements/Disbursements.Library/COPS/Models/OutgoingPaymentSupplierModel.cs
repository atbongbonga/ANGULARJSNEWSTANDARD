using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class OutgoingPaymentSupplierModel
    {
        public int InvDoc { get; set; }
        public int InvType { get; set; }
        public string GlAcct { get; set; }
        public Double App { get; set; }
        public Double EWT { get; set; }
        public int TransIDEWT { get; set; }
        public string ATC { get; set; }
    }
}
