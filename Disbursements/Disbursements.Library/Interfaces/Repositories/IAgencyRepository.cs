using AccountingLegacy.Disbursements.Library.COPS.Models;
using AccountingLegacy.Disbursements.Library.COPS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.Interfaces.Repositories
{
    public interface IAgencyRepository
    {
        void SaveAgencies(IEnumerable<AgenciesImportedModel> agencies);
        void PostAgencies(IEnumerable<AgenciesImportedModel> agencies);
        IEnumerable<AgenciesImportedView> GetAgencies();
        void RemoveAgencies(IEnumerable<AgenciesImportedModel> agencies);
    }
}
