using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal class GLSetupHeaderModel : BaseModel
    {
        public int DocEntry { get; set; }
        public string? Segment_0 { get; set; }
    }
}
