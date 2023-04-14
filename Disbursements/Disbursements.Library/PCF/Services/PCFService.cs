﻿using Core.Library.Models;
using Disbursements.Library.PCF.Helpers;
using Disbursements.Library.PCF.Repositories;
using Disbursements.Library.PCF.ViewModels;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.Services
{
    public class PCFService
    {
        private readonly PCFRepository _repository;
        private readonly string userCode;
        public PCFService(string userCode = "")
        {
            this.userCode = userCode;
            _repository = new PCFRepository(userCode);
        }

        public int PostJrnlEntry(JrnlEntryView data)
        {

            if (string.IsNullOrEmpty(data.Header.Memo)) throw new ApplicationException("Remarks is required.");
            if (string.IsNullOrEmpty(data.Header.Ref1)) throw new ApplicationException("Ref1 is required.");
            if (string.IsNullOrEmpty(data.Header.Ref2)) throw new ApplicationException("Ref2 is required.");
            if (string.IsNullOrEmpty(data.Header.Ref3)) throw new ApplicationException("Ref3 is required.");
            if(data.Details.Sum(x => x.Amount) != 0) throw new ApplicationException("Unbalanced transaction.");

            return _repository.PostJrnlEntry(data);
        }

        public void TagPcfPayment(PCFOP data)
        {
            var opEntryPcfovpm = _repository.PostPayment_EMS(data);
            _repository.TagPcfPayment(data, opEntryPcfovpm);
        }

        public int PostPayment(PCFOP data)
        {
            if (string.IsNullOrEmpty(data.Header.Bank)) throw new ApplicationException("BankCode is required.");
            if (string.IsNullOrEmpty(data.Header.Remarks)) throw new ApplicationException("Remarks is required.");
            if (string.IsNullOrEmpty(data.Header.BranchCode)) throw new ApplicationException("WhsV is required.");
            
            return _repository.PostPayment(data);
        }








    }
}
