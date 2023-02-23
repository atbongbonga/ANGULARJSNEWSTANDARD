using PF.Library.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF.Library.Services
{
    public class BankArrangementService
    {
        private readonly BankArrangementRepository repo;
        private readonly string userCode;
        public BankArrangementService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new BankArrangementRepository(this.userCode);
        }

        public void PostSetup(int docEntry) => repo.PostSetup(docEntry);
        public void PostAccrual(int docEntry) => repo.PostAccrual(docEntry);
        public void PostReversal(int docEntry) => repo.PostReversal(docEntry);
        public void PostPayment(DateTime bankDate, IEnumerable<int> docEntries) => repo.PostPayment(bankDate, docEntries);
    }
}
