using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal abstract class BaseModel
    {
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; }
        public string? RemovedBy { get; set; }
        public DateTime? RemovedDate { get;  }
    }
}
