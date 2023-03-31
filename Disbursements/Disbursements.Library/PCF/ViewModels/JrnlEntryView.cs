using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class JrnlEntryView
    {
        //123
        public JournalEntryHeaderView Header { get; set; }
        public IEnumerable<JournalEntrDetailView> Details { get; set; }



    }
}
