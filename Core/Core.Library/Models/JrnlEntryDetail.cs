using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class JrnlEntryDetail
    {
        public int TransId { get; set; }
        public int LineId { get; set; }
        public DateTime RefDate { get; set; }
        public string? Account { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? ShortName { get; set; }
        public string? LineMemo { get; set; }
        public string? Ref1 { get; set; }
        public string? Ref2 { get; set; }
        public string? Ref3 { get; set; }
        public string? U_EmpID { get; set; }
    }
}
