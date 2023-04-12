using AccountingLegacy.Disbursements.Library.COPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.Disbursements.Library.COPS.Services
{
    public class OPService
    {

        public bool ValidateField(OPPostingModel op) {
			try
			{
				bool Valid = true;

				return Valid;
			}
			catch (Exception){
                throw;
            }
			
        }


    }
}
