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

        public void UpdateTransactions(IEnumerable<ReconTransactionModel> _data, string _userId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "UPDATE",
                    data = _data.ToDataTable(),
                    userId = _userId
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

        public void InsertLogs(IEnumerable<ReconLog> _logs, string _userId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconTransaction";
                var parameter = new
                {
                    mode = "INSERT_LOGS",
                    logs = _logs.ToDataTable(),
                    userId = _userId
                };
                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
