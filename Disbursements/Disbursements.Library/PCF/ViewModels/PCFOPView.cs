using AccountingLegacy.Disbursements.Library.PCF.Models;
using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.PCF.ViewModels
{
    public class PCFUserInputView
    {
        public PCFUserInputHeader Header { get; set; }
        public List<PCFUserInputDetail> Detail { get; set; }
      
    }

    public class PCFPostGLAccountView : PaymentAccount
    { 
        public List<PaymentAccount> PaymentAccounts { get; set; }
        public List<PCFPostDuetoAdvanceAccounts> DuetoAdvancesAccounts { get; set; }
    }


}
