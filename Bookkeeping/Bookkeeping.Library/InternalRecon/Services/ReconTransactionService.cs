using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.Repositories;
using Bookkeeping.Library.InternalRecon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Services
{
    internal class ReconTransactionService
    {
        private readonly ReconTransactionRepository _repository;
        public string _userId;

        public ReconTransactionService(string userId)
        {
            if (string.IsNullOrEmpty(_userId)) throw new ApplicationException("Cannot access service, employee id is required.");

            _repository = new ReconTransactionRepository();
            _userId = userId.Trim();
        }

        public IEnumerable<ReconTransactionViewModel> GetTransactions(int transactionType)
        {
            if (transactionType.Equals(default)) throw new ArgumentException("Transaction type cannot be zero.");

            return _repository.GetReconTransactions(transactionType);
        }

        public IEnumerable<ReconTransactionViewModel> GetTransactions(string segment_0, string segment_1)
        {
            if (string.IsNullOrEmpty(segment_0)) throw new ApplicationException("Segment_0 cannot be null or empty");
            if (string.IsNullOrEmpty(segment_1)) throw new ApplicationException("Segment_1 cannot be null or empty");

            return _repository.GetReconTransactions(segment_0, segment_1);
        }

        public void InsertTransations(IEnumerable<ReconTransactionModel> data)
        {
            if (data is null) throw new ApplicationException("No data found");
            if (data.Sum(x => x.ReconAmount) is not decimal.Zero) throw new ApplicationException("Unbalanced transactions.");

            var postedTransaction = _repository.InsertTransactions(data, _userId);

            if (postedTransaction is null) throw new ApplicationException("Problem in posting data.");
            else
            {
                var logs = postedTransaction.Select(x => new ReconLog
                {
                    DocEntry = x.DocEntry,
                    EmpCode = _userId,
                    DocDate = DateTime.Now,
                    ActionTaken = "INSERT"
                });

                _repository.InsertLogs(logs, _userId);
            }
        }

        public void UpdateTransactions(IEnumerable<ReconTransactionModel> data)
        {
            if (data is null) throw new ApplicationException("No data found");

            var postedData = _repository.GetReconTransactions(data);

            if (postedData is null) throw new ApplicationException("No existing data.");
            else if (data.Any(x => !postedData.Any(y => y.DocEntry == x.DocEntry))) throw new ApplicationException("No existing data.");

            _repository.UpdateTransactions(data, _userId);

            var logs = postedData.Select(x => new ReconLog
            {
                DocEntry = x.DocEntry,
                EmpCode = _userId,
                DocDate = DateTime.Now,
                ActionTaken = "UPDATE"
            });

            _repository.InsertLogs(logs, _userId);
        }

        public void RemoveTransactions(IEnumerable<ReconTransactionModel> data)
        {
            if (data is null) throw new ApplicationException("No data found");

            var postedData = _repository.GetReconTransactions(data);

            if (postedData is null) throw new ApplicationException("No existing data.");
            else if (data.Any(x => !postedData.Any(y => y.DocEntry == x.DocEntry))) throw new ApplicationException("No existing data.");

            _repository.RemoveTransactions(data, _userId);

            var logs = postedData.Select(x => new ReconLog
            {
                DocEntry = x.DocEntry,
                EmpCode = _userId,
                DocDate = DateTime.Now,
                ActionTaken = "REMOVE"
            });

            _repository.InsertLogs(logs, _userId);
        }
    }
}
