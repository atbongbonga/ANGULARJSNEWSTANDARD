using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal class GLSetupLogsModel
    {
        public int DocEntry { get; set; }
        public int Line_ID { get; set; }
        public bool Status { get; set; }
        public bool IsRequired { get; set; }
        public string? Operator { get; set; }
        public bool IsColumn { get; set; }
        public string? Value { get; set; }
        public int Number { get; set; }
        public int PropertyId { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? UserIP { get; set; }
    }
}
