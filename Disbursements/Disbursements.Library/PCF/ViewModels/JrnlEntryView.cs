using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class JrnlEntryView
    {
        public JournalEntryHeaderView Header { get; set; }
        public List<JournalEntrDetailView> Details { get; set; }
    }
}
