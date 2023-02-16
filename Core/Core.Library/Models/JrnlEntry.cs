using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Library.Models
{
    public class JrnlEntry
    {
        public int TransId { get; set; }
        public int TransType { get; set; }
        public int BaseRef { get; set; }
        public DateTime RefDate { get; set; }
        public string Memo { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string U_FTDocNo { get; set; }
    }
}
