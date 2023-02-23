using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    internal class JrnlEntryVoew
    {
        public JrnlEntry Header { get; set; }
        public IEnumerable<JrnlEntryDetail> Details { get; set; }
    }
}
