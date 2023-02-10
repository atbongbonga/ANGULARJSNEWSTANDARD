using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Models
{
    public class AgenciesImportedDetailModel
    {
        public int ImportID { get; set; }
        public int LineId { get; set; }
        public int SetupId { get; set; }
        public string ColumnId { get; set; }
        public string ColumnName { get; set; }
        public decimal ColumnValue { get; set; }
        public string AcctCode { get; set; }
        public string BankType { get; set; }
    }
}
