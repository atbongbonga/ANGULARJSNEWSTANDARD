using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    public class AutoReconTransactionModel
    {
        public int Id { get; set; }
        public int GroupNumber { get; set; }
        public int TransId { get; set; }
        public int Line_ID { get; set; }
        public decimal Amount { get; set; }
        public string? Segment_0 { get; set; }
        public string? Segment_1 { get; set; }
        public DateTime RefDate { get; set; }
        public DateTime SyncedDate { get; set; }
        public DateTime ReconDate { get; set; }
        public int SettlementId { get; set; }
        public int SettlementLineId { get; set; }
        public int ReconNum { get; set; }
        public int TransactionType { get; set; }
    }
}
