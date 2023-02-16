using AccountingLegacy;
using Bookkeeping.Library.InternalRecon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Repositories
{
    internal class GLSetupRepository
    {
        private readonly SERVER server;
        public GLSetupRepository()
        {
            server = new SERVER("GL Setup");
        }

        #region CRUD

        public IEnumerable<GLSetupHeaderViewModel> GetHeaders()
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = ""
            }
        }

        #endregion

    }
}
