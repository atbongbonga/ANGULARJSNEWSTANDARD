﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public  class PCFOP
    {
        public PCFOPHeader Header { get; set; }
        public List<PCFOPDetail> Detail { get; set; }

    }
}