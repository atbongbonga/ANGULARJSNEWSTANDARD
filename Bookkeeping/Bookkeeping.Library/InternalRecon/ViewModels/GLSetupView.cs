using Bookkeeping.Library.InternalRecon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.ViewModels
{
    internal class GLSetupView
    {
        public string? UserIP { get; set; }
        private GLSetupHeaderModel header = new GLSetupHeaderModel();
        public GLSetupHeaderModel Header { get => header; set => header = value; }
        public IEnumerable<GLSetupDetailsModel> Details { get; set; }
        public IEnumerable<GLSetupLogsModel> Logs { get; set; }
    }
}
