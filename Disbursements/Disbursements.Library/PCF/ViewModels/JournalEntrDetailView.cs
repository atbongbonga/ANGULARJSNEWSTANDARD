using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class JournalEntrDetailView : JrnlEntryDetail
    {
        public int Docentry { get; set; }
        public string AcctCode { get; set; } 
        public string FormatCode { get; set; } 
        public DateTime DocDate { get; set; }
        public string BrCode { get; set; } 
        public decimal Amount { get; set; }

    }
}
