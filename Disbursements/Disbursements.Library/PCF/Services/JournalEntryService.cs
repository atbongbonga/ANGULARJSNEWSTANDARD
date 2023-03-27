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
            _repository = new JournalEntryRepository(userCode);
        }




        public int PostJrnlEntry(JrnlEntryView data)
        {

       
            return _repository.PostJrnlEntry(data);
        }

 



        



    }
}
