using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Repositories
{
    internal class PostGLSetupParameterHelper
    {
        public string? mode { get; set; }
        public DataTable? header { get; set; }
        public DataTable? details { get; set; }
        public DataTable? detailProperties { get; set; }
    }
}
