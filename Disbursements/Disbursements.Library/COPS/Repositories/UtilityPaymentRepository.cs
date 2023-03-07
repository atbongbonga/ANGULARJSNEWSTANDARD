using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLegacy;
using Disbursements.Library.COPS.ViewModels.Utility;

namespace Disbursements.Library.COPS.Repositories
{
    internal class UtilityPaymentRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public UtilityPaymentRepository()
        {
            server = new SERVER("Outgoing Payment");
            this.empCode = empCode;
        }
    }
}
