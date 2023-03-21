using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.Repositories;
using Core.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Services
{
    public class AutoReconService
    {
        private readonly AutoReconRepository _repository;
        public string? _userId;

        public AutoReconService(string userId = "")
        {
            _repository = new AutoReconRepository();
            _userId = userId;
        }

        public IEnumerable<AutoReconTransactionModel> GetTransactions(int _transactionType)
        {
            return _repository.GetTransactions(_transactionType);
        }

        public void Update(int _transactionType, int _groupNumber, DateTime _syncedDate)
        {
            if (_transactionType.Equals(default)) throw new ArgumentException("Transaction type cannot be zero.");
            if (_groupNumber.Equals(default)) throw new ArgumentException("Group number cannot be zero.");
            if (_syncedDate.Equals(default)) throw new ArgumentException("Synced date is required.");

            _repository.Update(_transactionType, _groupNumber, _syncedDate);
        }
        
        public void Log(AutoReconTransactionModel _transaction, string _error = "")
        {
            if (_transaction.TransactionType.Equals(default)) throw new ArgumentException("Transaction type cannot be zero.");
            if (_transaction.GroupNumber.Equals(default)) throw new ArgumentException("Group number cannot be zero.");
            if (_transaction.SyncedDate.Equals(default)) throw new ArgumentException("Synced date is required.");
            if (_transaction.ReconNum.Equals(default)) throw new ArgumentException("Group number cannot be zero.");

            _repository.Log(_transaction, _error);
        }

        public IEnumerable<int> GetTransactionRows(IEnumerable<AutoReconTransactionModel> _transactions, string segment_0, string segment_1, DateTime reconDate)
        {
            return _repository.GetTransactionRows(_transactions, segment_0, segment_1, reconDate);
        }
    }
}
