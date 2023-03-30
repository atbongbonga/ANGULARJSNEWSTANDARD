using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class PCFOPAccountsView 
    {
        public string AcctCode { get; set; }
        public string FormatCode { get; set; }
        public decimal SumPaid { get; set; }
        public string Description { get; set; }
        public int U_Docline { get; set; }
    }
}
