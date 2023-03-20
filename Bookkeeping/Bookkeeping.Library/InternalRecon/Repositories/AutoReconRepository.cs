using AccountingLegacy;
using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.ViewModels;
using Core.Library.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bookkeeping.Library.InternalRecon.Repositories
{
    public class AutoReconRepository
    {
        private readonly SERVER server;

        public AutoReconRepository()
        {
            server = new SERVER("Auto Reconciliation");
        }

        public IEnumerable<AutoReconTransactionModel> GetTransactions(int _transactionType)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spAutoInternalRecon";
                var parameter = new
                {
                    mode = "GET",
                    transactionType = _transactionType
                };
                return cn.Query<AutoReconTransactionModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void Update(int _transactionType, int _groupNumber, DateTime _syncedDate)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spAutoInternalRecon";
                var parameter = new
                {
                    mode = "UPDATE",
                    transactionType = _transactionType,
                    groupNumber = _groupNumber,
                    syncedDate = _syncedDate
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void Log(AutoReconTransactionModel _transaction, string _error)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spAutoInternalRecon";
                var parameter = new
                {
                    mode = "LOG",
                    groupNumber = _transaction.GroupNumber,
                    transactionType = _transaction.TransactionType,
                    syncDate = _transaction.SyncedDate,
                    reconNumber = _transaction.ReconNum,
                    error = _error
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
