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
        public string AcctCode { get; set; } = string.Empty;
        public string FormatCode { get; set; } = string.Empty;
        public DateTime DocDate { get; set; }
        public string BrCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }

    }
}
