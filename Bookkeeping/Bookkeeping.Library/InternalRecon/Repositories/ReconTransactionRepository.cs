using AccountingLegacy;
using Bookkeeping.Library.InternalRecon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Bookkeeping.Library.InternalRecon.Models;
using MoreLinq;
using System.Xml.Linq;

namespace Bookkeeping.Library.InternalRecon.Repositories
{
    public class ReconTransactionRepository
    {
        private readonly SERVER server;

        public ReconTransactionRepository()
        {
            server = new SERVER("Recon Transaction");
        }

        public IEnumerable<ReconTransactionViewModel> GetReconTransactions(string _segment_0, string _segment_1, DateTime _asOfDate)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "GET",
                    segment_0 = _segment_0,
                    segment_1 = _segment_1,
                    asOfDate = _asOfDate
                };
                return cn.Query<ReconTransactionViewModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<ReconTransactionViewModel> GetReconTransactions(IEnumerable<ReconTransactionModel> _data)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "GET_BY_DOCENTRIES",
                    data = _data.ToDataTable()
                };
                return cn.Query<ReconTransactionViewModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<ReconTransactionModel> InsertTransactions(IEnumerable<ReconTransactionModel> _data, string _userId)
        {
            foreach (var item in _data)
            {
                item.CanceledDate = DateTime.Now;
                item.PostedDate = DateTime.Now;
                item.ReconDate = DateTime.Now;
            }

            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "INSERT",
                    data = _data.ToDataTable(),
                    userId = _userId
                };
                return cn.Query<ReconTransactionModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void UpdateTransactions(int _groupNumber)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "UPDATE",
                    groupNumber = _groupNumber
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void RemoveTransactions(IEnumerable<ReconTransactionModel> _data, string _userId)
        {
            foreach (var item in _data)
            {
                item.CanceledDate = DateTime.Now;
                item.PostedDate = DateTime.Now;
                item.ReconDate = DateTime.Now;
            }

            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "REMOVE",
                    data = _data.ToDataTable(),
                    userId = _userId
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void Log(IEnumerable<ReconLog> _logs, string _userId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "LOG_ACTION",
                    logs = _logs.ToDataTable(),
                    userId = _userId
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<ReconTransactionViewModel> GetForReconTransactions()
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "GET_FOR_RECON"
                };
                return cn.Query<ReconTransactionViewModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<int> GetTransactionRows(IEnumerable<ReconTransactionViewModel> _transactions, string segment_0, string segment_1, DateTime reconDate)
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

        public void Log(int _groupNumber, string _error)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "LOG",
                    groupNumber = _groupNumber,
                    error = _error
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void AutoUpdate()
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "AUTO_UPDATE"
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
