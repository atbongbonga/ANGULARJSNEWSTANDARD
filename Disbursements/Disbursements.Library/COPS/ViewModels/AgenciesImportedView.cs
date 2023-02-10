using AccountingLegacy.Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.COPS.ViewModels
{
    public class AgenciesImportedView : AgenciesImportedModel
    {
        public string EmpName { get; set; }
        public string BrName { get; set; }
        public string CorpName { get; set; }
        public string DeptName { get; set; }
        public string AcctName { get; set; }
    }
}
