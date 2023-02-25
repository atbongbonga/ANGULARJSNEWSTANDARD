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
    public class JournalEntryService
    {
        private readonly JournalEntryRepository _repository;
        private readonly string userCode;
        public JournalEntryService(string userCode = "")
        {
            this.userCode = userCode;
            _repository = new JournalEntryRepository();
        }

  
        public int PostJrnlEntry(JrnlEntryView data)
        {
            //if (string.IsNullOrEmpty(segment_0)) throw new ApplicationException("Segment_0 cannot be null or empty");

            data.Header.Ref1 = data.Header.Ref1.Length > 99 ? data.Header.Ref1.Substring(0, 99): data.Header.Ref1;
            data.Header.Ref2 = data.Header.Ref2.Length > 99 ? data.Header.Ref2.Substring(0, 99) : data.Header.Ref2;
            data.Header.Ref3 = data.Header.Ref3.Length > 26 ? data.Header.Ref3.Substring(0, 99) : data.Header.Ref3;
            data.Header.Memo = data.Header.Memo.Length > 50 ? data.Header.Ref3.Substring(0, 99) : data.Header.Memo;
            var model = ValidateDetail(data);
            return _repository.PostJrnlEntry(model);
        }

        public JrnlEntryView ValidateDetail(JrnlEntryView data)
        {
            JrnlEntryView model = new JrnlEntryView();
            model.Header = data.Header;
            int detailCount = data.Details.Count();
            for (var i = 0; i < detailCount; i++)
            {
                var account= data.Details[i].Account != null ? data.Details[i].Account : _repository.GettAcctCodeByFormatCode(data.Details[i].AccountCode);
                var shortName = data.Details[i].ShortName != null && data.Details[i].FormatCode.Substring(0, 5) == PcfBuilder.ShortNameCheck() ? data.Details[i].ShortName : account;
                model.Details.Add(new JournalEntrDetailView()
                {
                    Account = account,
                    AccountCode = account,
                    Debit = data.Details[i].Debit,
                    Credit = data.Details[i].Credit,
                    LineMemo =  data.Details[i].LineMemo.Length > 99 ? data.Details[i].LineMemo.Substring(0, 99) : data.Details[i].LineMemo,
                    ShortName = shortName,
                    Ref1 = data.Details[i].Ref1,
                    Ref2 = data.Details[i].Ref2,

                });


            }
                return model;
        }






    }
}
