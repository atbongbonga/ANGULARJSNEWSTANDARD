using AccountingLegacy.Core.Library;
using Disbursements.Library.PCF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.Repositories
{
    internal class ProfFeesRepository
    {
        public void PostJrnlEntry(JrnlEntryVoew data)
        {
			try
			{
				using (var sap = new SAPBusinessOne())
				{
					var entry = sap.JournalEntries;

				}
			}
			catch (Exception ex)
			{
				LogError(ex.GetBaseException().Message, 1);
				throw;
			}
        }

		private void LogError(string msg, int docentry)
		{

		}
    }
}
