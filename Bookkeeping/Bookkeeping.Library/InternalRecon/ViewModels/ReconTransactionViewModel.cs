using Bookkeeping.Library.InternalRecon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.ViewModels
{
    internal class ReconTransactionViewModel : ReconTransactionModel
    {
        public DateTime RefDate { get; set; }
        public string? Ref1 { get; set; }
        public string? Ref2 { get; set; }
        public string? Ref3Line { get; set; }
        public string? LineMemo { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal BalDueDeb { get; set; }
        public decimal BalDueCred { get; set; }
        public string? AcctName { get; set; }
    }
}
