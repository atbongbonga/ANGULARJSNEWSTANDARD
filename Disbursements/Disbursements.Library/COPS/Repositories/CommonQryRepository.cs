using AccountingLegacy.Core.Library;
using AccountingLegacy.Disbursements.Library.Auth;
using AccountingLegacy.Disbursements.Library.COPS.Models;
using AccountingLegacy.Disbursements.Library.COPS.ViewModels;
using AccountingLegacy.Disbursements.Library.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MoreLinq;
using SAPbobsCOM;
using System.Reflection.Emit;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Diagnostics;
using AccountingLegacy;

namespace AccountingLegacy.Disbursements.Library.COPS.Repositories
{
    public class CommonQryRepository
    {
        private readonly SERVER server;
        public CommonQryRepository()
        {
            server = new SERVER("Disbursement Common Query");
        }
        public string GetVoucherNum(string WhsCode, DateTime dtd) {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spOPGetVOucher";
                var parameters = new
                {
                    whs = WhsCode,
                    vdate = dtd
                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string GetAccountCode(string Acct) {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_CommonQry";
                var parameters = new
                {
                    Type = "GetAccountCode",
                    Acct = Acct
                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string GetCreditCard(string Acct)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_CommonQry";
                var parameters = new
                {
                    Type = "GetCreditCard",
                    Acct = Acct
                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string GetAccountName(string Acct)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_CommonQry";
                var parameters = new
                {
                    Type = "GetAccountName",
                    Acct = Acct
                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string GetGLByBankCodeAndBranch(string BankCode , string Branch)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_CommonQry";
                var parameters = new
                {
                    Type = "GetGLByBankCodeAndBranch",
                    BankCode = BankCode,
                    Branch = Branch
                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }


    }
}
