﻿using Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.ViewModels
{
    public class JournalEntrDetailView : JrnlEntryDetail
    {
      public string AccountCode { get; set; }
      public string FormatCode { get; set; }
    }
}