using AccountingLegacy;
using Disbursements.Library.COPS.ViewModels.JGMAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.COPS.Repositories
{
    internal class JGMANRepository
    {
        private readonly SERVER server;
        public JGMANRepository()
        {
            server = new SERVER("JGMAN Posting");
        }

        public void PostPayment(JGMANPostingView data)
        {

        }

    }
}
