using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.JGMAN
{
    public class SummaryView
    {
        public string GenId { get; set; }
        public string CorpCode { get; set; }
        public string CorpName { get; set; }
        public string BrCode { get; set; }
        public string BrName { get; set; }
        public string AcctType { get; set; }
        public decimal Amount { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CollnRemarks { get; set; }
        public string OffRcptNo { get; set; }
        public DateTime? OffRcptDate { get; set; }
    }
}
