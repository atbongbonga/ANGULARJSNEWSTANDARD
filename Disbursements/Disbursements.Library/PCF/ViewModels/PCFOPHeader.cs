using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class PCFOPHeader 
    {
        public int DocEntry { get; set; }
        public int OPNum { get; set; }
        public string Payee { get; set; }
        public string Addr { get; set; }
        public DateTime DocDate { get; set; }
        public string WhsCode { get; set; }
        public string Bank { get; set; }
        public string BankName { get; set; }
        public double Total { get; set; }
        public double EWTTotal { get; set; }
        public string BranchCode { get; set; }
        public string Voucher { get; set; }
        public string Remarks { get; set; }
        public string ChkNum { get; set; }
        public string PostBy { get; set; }
        public string Custodian { get; set; }

    }
}
