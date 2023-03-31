using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Lessor
{
    public class PaymentLessorHeaderView : Payment
    {
        public DateTime DueDate { get; set; }
        public string WhsCode { get; set; }
        public string Bank { get; set; }
        public string Remarks { get; set; }
        public string CheckPrint { get; set; }
        public string CheckRemarks { get; set; }
    }
}
