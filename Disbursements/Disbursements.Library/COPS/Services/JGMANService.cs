using Disbursements.Library.COPS.Repositories;
using Disbursements.Library.COPS.ViewModels.JGMAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Services
{
    public class JGMANService
    {
        private readonly JGMANRepository repo;
        public JGMANService(string userCode = "")
        {
            repo = new JGMANRepository(userCode);
        }

        public void PostPayments(IEnumerable<SummaryView> data)
        {
            if (data is null) throw new ApplicationException("Data not found.");
            repo.PostPayments(data);
        }

        public IEnumerable<SummaryView> GetSummary(string genId, string acctType, bool active)
        {
            return repo.GetSummary(genId, acctType, active);
        }

        public IEnumerable<DetailView> GetDetails(string genId, string acctType, string brCode)
        {
            return repo.GetDetails(genId, acctType, brCode);
        }
    }
}
