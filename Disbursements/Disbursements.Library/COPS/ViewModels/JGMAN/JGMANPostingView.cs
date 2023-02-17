using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.JGMAN
{
    public class JGMANPostingView
    {
        public JGMANPostingHeaderView Header { get; set; }
        public IEnumerable<JGMANPostingDetailView> Details { get; set; }
    }
}
