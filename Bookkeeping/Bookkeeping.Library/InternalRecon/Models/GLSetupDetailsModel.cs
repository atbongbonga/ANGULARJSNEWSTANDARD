using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal class GLSetupDetailsModel : BaseModel
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int Line_ID { get; set; }
        public bool IsRequired { get; set; }
        public string? Operator { get; set; }
        public bool IsColumn { get; set; }
        public string? Value { get; set; }
        public bool SequenceNumber { get; set; }
        public int GroupNumber { get; set; }
    }
}
