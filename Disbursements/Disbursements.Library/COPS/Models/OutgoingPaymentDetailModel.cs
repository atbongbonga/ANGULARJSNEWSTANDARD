using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class OutgoinPaymentDetailModel
    {
        public int LineId { get; set; }
        public int DoceEntry { get; set; }
        public int ObjectType { get; set; }
        public DateTime DocDate { get; set; }
        public decimal Outstanding{ get; set; }
        public int Overdue { get; set; }
        public decimal App{ get; set; }
        public decimal Balance{ get; set; }
        public string WhsCode { get; set; }
        public string GLAcct { get; set; }
        public string Status { get; set; }
        public string ATC { get; set; }
        public decimal EWT { get; set; }
        public int TransIdEWT { get; set; }

    }
}
