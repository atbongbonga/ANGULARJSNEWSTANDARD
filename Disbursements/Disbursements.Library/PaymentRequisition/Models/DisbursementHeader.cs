using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PaymentRequisition.Models
{
    internal class DisbursementHeader
    {
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }
        public string ReqNo { get; set; }
        public string EmpCode { get; set; } 
        public string PMode { get; set; }
        public string Stat { get; set; }
        public string Whs { get; set; }
        public string Dept { get; set; }
        public string Cust { get; set; }
        public string Cat { get; set; }   
        public string Chkstat { get; set; }
        public string Pmeans { get; set; }
        public string CHWhs { get; set; }
        public string OPNum { get; set; }
        public string Disb { get; set; }


    }
}
