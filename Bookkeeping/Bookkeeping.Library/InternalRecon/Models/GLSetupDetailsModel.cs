using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal class GLSetupDetailsModel
    {
        public int DocEntry { get; set; }
        public int Line_ID { get; set; }
        public bool IsRequired { get; set; }
        public string Operator { get; set; }
        public bool IsColumn { get; set; }

        
    }
}
