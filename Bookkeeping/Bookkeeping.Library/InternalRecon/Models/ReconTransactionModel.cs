using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    public class ReconTransactionModel 
    {
        public int DocEntry { get; set; }
        public int GroupNumber { get; set; }
        public int ReconNumber { get; set; }
        public decimal ReconAmount { get; set; }
        public DateTime ReconDate { get; set; }
        public string? ReconBy { get; set; }
        public int TransId { get; set; }
        public int Line_ID { get; set; }
        public string? Segment_0 { get; set; }
        public string? Segment_1 { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime CanceledDate { get; set; }
        public string? CanceledBy { get; set; }
        public bool IsActive { get; set; }
        public string? PostedBy { get; set; }
        public DateTime PostedDate { get; set; }
        public string? ErrorMessage { get; set; }
        public int TransactionType { get; set; }
    }
}
