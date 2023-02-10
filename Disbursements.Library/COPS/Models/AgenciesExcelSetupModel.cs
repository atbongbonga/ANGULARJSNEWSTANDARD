using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class AgenciesExcelSetupModel
    {
        public int SetupId { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdateBy { get; set; }
        public string ColumnId { get; set; }
        public int SortId { get; set; }
        public bool Active { get; set; }
        public string ColumnName { get; set; }
        public string AcctCode { get; set; }
        public string BankType { get; set; }
    }
}
