using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class JournalEntryHeaderView: JrnlEntry
    {
        public int Docentry { get; set; }
        public string PCFOP { get; set; }
        public string PCFDoc { get; set; }
        public string PostBy { get; set; }
        public string BrCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime DocDate { get; set; }

    }
}
 