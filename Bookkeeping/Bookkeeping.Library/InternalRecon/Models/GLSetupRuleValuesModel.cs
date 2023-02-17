using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal class GLSetupRuleValuesModel
    {
        public int DocEntry { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }

    }
}
