using AccountingLegacy.Disbursements.Library.PCF.Models;
using Core.Library.Models;
using Disbursements.Library.PCF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.PCF.ViewModels
{
    public class PCFOPView 
    {
        public PCFOPHeaderView Header { get; set; }
        public List<PCFOPAccountsView> Accounts { get; set; }
        public List<PCFOPChecksView> Checks { get; set; }

    }

}
