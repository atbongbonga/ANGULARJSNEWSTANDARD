using Bookkeeping.Library.InternalRecon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.ViewModels
{
    internal class GLSetupHeaderViewModel : GLSetupHeaderModel
    {
        public string? CreatedBy_Name { get; set; }
        public string? AcctName { get; set; }
    }
}
