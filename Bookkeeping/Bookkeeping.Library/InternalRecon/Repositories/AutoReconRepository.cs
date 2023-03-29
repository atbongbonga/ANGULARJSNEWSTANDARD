using AccountingLegacy;
using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.ViewModels;
using Core.Library.Enums;
using Dapper;
using MoreLinq;
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

        public IEnumerable<AutoReconTransactionModel> GetTransactions(int _transactionType, string _segment_1, bool _isRecon)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spAutoInternalRecon";
                var parameter = new
                {
                    mode = "GET",
                    transactionType = _transactionType,
                    segment_1 = _segment_1,
                    isRecon = _isRecon
                };
                return cn.Query<AutoReconTransactionModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<TransactionTypeModel> GetTypes()
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spAutoInternalRecon";
                var parameter = new
                {
                    mode = "GET_TYPE"
                };
                return cn.Query<TransactionTypeModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
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
                    syncedDate = _transaction.SyncedDate,
                    reconNumber = _transaction.ReconNum,
                    error = _error
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<int> GetTransactionRows(IEnumerable<AutoReconTransactionModel> _transactions, string segment_0, string segment_1, DateTime reconDate)
        {
            var list = _transactions.Select(x => new AutoReconJEDetailsModel
            {
                TransId = x.TransId,
                Line_ID = x.Line_ID
            });

            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spAutoInternalRecon";
                var parameter = new
                {
                    mode = "GET_RECON_ROWS",
                    jeDetails = list.ToDataTable(),
                    segment_0 = segment_0,
                    segment_1 = segment_1,
                    reconDate = reconDate.ToShortDateString()
                };

                return cn.Query<int>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
