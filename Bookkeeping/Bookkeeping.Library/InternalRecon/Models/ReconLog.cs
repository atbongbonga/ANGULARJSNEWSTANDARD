using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    public class ReconLog
    {
        public int DocEntry { get; set; }
        public string? EmpCode { get; set; }
        public string? ActionTaken { get; set; }
        public DateTime DocDate { get; set; }
    }
}
