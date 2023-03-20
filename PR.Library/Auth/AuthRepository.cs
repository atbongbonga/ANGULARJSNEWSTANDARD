using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.PR.Library.Auth
{
    internal class AuthRepository
    {
        private readonly SERVER server;

        public AuthRepository()
        {
            server = new SERVER("PR");
        }

        public EmployeeModel Login(string username, string password)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
            {
                var storedProc = "UserLogin";
                var parameters = new { UserName = username, Password = password };
                return cn.QueryFirst<EmployeeModel>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
