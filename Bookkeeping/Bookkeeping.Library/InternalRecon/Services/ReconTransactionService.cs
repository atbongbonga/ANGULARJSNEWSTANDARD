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
    public class ReconTransactionService
    {
        private readonly ReconTransactionRepository _repository;
        public string? _userId;

        public ReconTransactionService(string userId = "")
        {
            _repository = new ReconTransactionRepository();
            _userId = userId.Trim();
        }

        public IEnumerable<ReconTransactionViewModel> GetTransactions(string segment_0, string segment_1)
        {
            if (string.IsNullOrEmpty(segment_0)) throw new ApplicationException("Segment_0 cannot be null or empty");
            if (string.IsNullOrEmpty(segment_1)) throw new ApplicationException("Segment_1 cannot be null or empty");

            return _repository.GetReconTransactions(segment_0, segment_1);
        }

        public IEnumerable<ReconTransactionModel> InsertTransations(IEnumerable<ReconTransactionModel> data)
        {
            if (data is null) throw new ApplicationException("No data found");
            if (data.Sum(x => x.Balance) is not decimal.Zero) throw new ApplicationException("Unbalanced transactions.");

            var postedTransactions = _repository.InsertTransactions(data, _userId!);

            if (postedTransactions is null) throw new ApplicationException("Problem in posting data.");
            else
            {
                var logs = postedTransactions.Select(x => new ReconLog
                {
                    DocEntry = x.DocEntry,
                    EmpCode = _userId,
                    DocDate = DateTime.Now,
                    ActionTaken = "INSERT"
                });

                _repository.Log(logs, _userId!);
            }

            return postedTransactions;
        }

        public void UpdateTransactions(int groupNumber)
        {
            _repository.UpdateTransactions(groupNumber);
        }

        public void RemoveTransactions(IEnumerable<ReconTransactionModel> data)
        {
            if (data is null) throw new ApplicationException("No data found");

            _repository.RemoveTransactions(data, _userId!);

            var logs = data.Select(x => new ReconLog
            {
                DocEntry = x.DocEntry,
                EmpCode = _userId,
                DocDate = DateTime.Now,
                ActionTaken = "REMOVE"
            });

            _repository.Log(logs, _userId!);
        }

        public IEnumerable<ReconTransactionViewModel> GetForRecon()
        {
            return _repository.GetForReconTransactions();
        }

        public IEnumerable<int> GetTransactionRows(IEnumerable<ReconTransactionViewModel> _transactions, string segment_0, string segment_1, DateTime reconDate)
        {
            return _repository.GetTransactionRows(_transactions, segment_0, segment_1, reconDate);
        }

        public void Log(int _groupNumber, string _error = "")
        {
            if (_groupNumber.Equals(default)) throw new ArgumentException("Group number cannot be zero.");

            _repository.Log(_groupNumber, _error);
        }

        public void AutoUpdate()
        {
            _repository.AutoUpdate();
        }
    }
}
