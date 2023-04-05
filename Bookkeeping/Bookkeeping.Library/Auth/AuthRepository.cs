using AccountingLegacy;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Bookkeeping.Library.Auth
{
    internal class AuthRepository
    {
        private readonly SERVER server;

        public AuthRepository()
        {
            server = new SERVER("BOOKKEEPING");
        }

        public EmployeeViewModel Login(string username, string password)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
            {
                var storedProc = "UserLogin";
                var parameters = new { UserName = username, Password = password };
                return cn.QueryFirstOrDefault<EmployeeViewModel>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
