using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class AgenciesImportedGroupsModel
    {
        public int GrpId { get; set; }
        public string GrpName { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdateBy { get; set; }
        public string AcctCode { get; set; }
        public string BankType { get; set; }
    }
}
