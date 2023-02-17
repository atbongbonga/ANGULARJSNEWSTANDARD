using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.JGMAN
{
    public class JGMANPostingHeaderView
    {
        public DateTime DocDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GenID { get; set; }
        public string UserID { get; set; }
    }
}
