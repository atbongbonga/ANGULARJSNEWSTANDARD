using AccountingLegacy.Disbursements.Library.PCF.ViewModels;
using AccountingLegacy.Disbursements.Library.PCF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MoreLinq.Extensions;

namespace AccountingLegacy.Disbursements.Library.PCF.Services
{
    public class PCFOPService
    {

        public int PostPCFOP(PCFOPView model) {
            try
            {
                PCFOPRepository repo = new PCFOPRepository();
                if (model.Header.PType == "WITH SAP") { repo.CheckIfPosted(model); } //Check If Already posted

                foreach (var item in model.Detail) // Check per line status if posted or with setup
                {
                    repo.CheckLineIfPosted(item.SAP, item.AcctCode, item.WhsCode);
                }

                int _OPNum = 0;
                if (model.Header.PType.ToString().ToUpper() == "WITH SAP")
                {
                    var _EwtList = model.Detail.Where(x => !string.IsNullOrEmpty(x.ATCCode));
                    model.Header.EWTTotal = _EwtList.Sum(x => x.WTax);
                    _OPNum = repo.PostPCFOP(model);
                    repo.OPChangesLogs(model.Header.PostBy);
                }
                else {
                    _OPNum = model.Header.OPNum;
                    repo.CheckBankDetail(_OPNum, model.Header.Bank);
                }

                int _OPEntry = repo.UpsertPCFDetails(model); // Update PCF Tables

                PCFView UpdateModel = new PCFView();
                UpdateModel.Header.DocEntry = _OPEntry;
                UpdateModel.Header.OPNum = _OPNum;
                repo.UpatePCFOP(UpdateModel);

                return _OPEntry;
            }
            catch (Exception)
            {

                throw;
            }
           
        }


    }
}
