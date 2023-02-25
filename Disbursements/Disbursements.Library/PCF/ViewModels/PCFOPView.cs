﻿using AccountingLegacy.Disbursements.Library.PCF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.PCF.ViewModels
{
    public class PCFOPView : PCFOPModel
    {
        public PCFPayHeader Header { get; set; }
        public List<PCFPayDetail> Detail { get; set; }
      
    }

    public class PCFView { 
        public PCFHdr Header { get; set; }
        public List<PCFDtl> Detail { get; set; }
    }
}