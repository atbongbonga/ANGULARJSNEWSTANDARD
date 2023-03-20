using AccountingLegacy.Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.COPS.ViewModels
{
    public class OPPostingView : OPPostingModel
    {
        public OPHdr OPPostHdr { get; set; }
        public List<OPSup> Supp { get; set; }
        public List<OPAcct> Acct { get; set; }
    }
}
