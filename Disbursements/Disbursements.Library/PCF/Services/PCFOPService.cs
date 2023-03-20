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

        public int PostPCFOP(PCFUserInputView model) {
            try
            {
                PCFOPRepository repo = new PCFOPRepository();
                int _OPNum = 0;
                if (model.Header.PType.ToString().ToUpper() == "WITH SAP")
                {
                    _OPNum = repo.PostPCFOP(model);
                    repo.InsertChangesLogs(model.Header.PostBy);
                }
                else {
                    _OPNum = model.Header.OPNum;
                    repo.CheckBankDetail(_OPNum, model.Header.Bank);
                }

                int _OPEntry = repo.UpdatePCFTable(model); 

                model.Header.DocEntry = _OPEntry;
                model.Header.OPNum = _OPNum;
                repo.UpatePCFOPReference(model);

                return _OPEntry;
            }
            catch (Exception)
            {

                throw;
            }
           
        }


    }
}
