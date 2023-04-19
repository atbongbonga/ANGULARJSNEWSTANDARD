using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class PCFOPHeaderView : Payment
    {

        public string Payee { get; set; }
        public string WhsCode { get; set; }
        public string Bank { get; set; }
        public string BranchCode { get; set; }
        public string Remarks { get; set; }
        public string ChkNum { get; set; }
        public string AcctCode { get; set; }
        public string CheckSum { get; set; }
        public int joinKey{ get; set; }



    }
}
