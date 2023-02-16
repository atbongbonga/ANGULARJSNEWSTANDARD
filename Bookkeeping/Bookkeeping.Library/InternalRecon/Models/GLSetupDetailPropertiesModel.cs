using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Models
{
    internal class GLSetupDetailPropertiesModel : BaseModel
    {
        public int DocEntry { get; set; }
        public int Line_ID { get; set; }
        public string? Value { get; set; }
        public int Id { get; set; }
        public int Number { get; set; }
        public int GroupNumber { get; set; }
    }
}
