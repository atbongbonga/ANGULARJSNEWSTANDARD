﻿using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.ViewModels.Utility
{
    public class PaymentUtilityAccountView : PaymentAccount
    {
        public string AcctNo { get; set; }
        public string InvNo { get; set; }
        public string BrWhs { get; set; }
        public string GLAcct { get; set; }
        public string BrCode { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsManualVat { get; set; }
        public decimal NetVat { get; set; }
        public decimal Vat { get; set; }
        public string TaxGroup { get; set; }
        public string ATC { get; set; }
        public decimal Rate { get; set; }
        public decimal EWT { get; set; }
        public decimal ARAmt { get; set; }
        public string ARRemarks { get; set; }
        public decimal WTE { get; set; }
        public bool ManualVat { get; set; }
        public int OPUtilDocEntry { get; set; }
        public int DetId { get; set; }

    }
}
