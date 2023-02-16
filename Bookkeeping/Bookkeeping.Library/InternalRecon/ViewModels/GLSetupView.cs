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
        private GLSetupHeaderViewModel header = new GLSetupHeaderViewModel();
        public GLSetupHeaderViewModel Header { get => header; set => header = value; }
        public IEnumerable<GLSetupDetailsModel>? Details { get; set; }
        public IEnumerable<GLSetupDetailPropertiesModel>? DetailsProperties { get; set; }
    }
}
