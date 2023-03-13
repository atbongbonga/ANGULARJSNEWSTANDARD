using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Utility
{
    public class PaymentHeaderView : Payment
    {
        public int? CreditCard { get; set; }
        public string CreditAcct { get; set; }
        public string WhsCode { get; set; }
        public string PMode { get; set; }
        public string PayTo { get; set; }
        public string CheckPrint { get; set; }
        public string CheckRemarks { get; set; }
        public string CheckPrintRemarks { get; set; }
        public string CheckPrintName { get; set; }
        public int PCFDocNum { get; set; }
        public string Bank { get; set; }
        public string BankCode { get; set; }
        public string UID{ get; set; }
        public string UName { get; set; }
        public string CAOARes { get; set; }
        public int AdvDueJE { get; set; }
        public string FBillNo { get; set; }
        public int? OPUtilDocEntry { get; set; }
    }
}
