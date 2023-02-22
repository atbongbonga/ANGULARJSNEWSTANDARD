using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.Repositories;
using Bookkeeping.Library.InternalRecon.ViewModels;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.InternalRecon.Services
{
    internal class GLSetupService
    {
        private readonly GLSetupRepository _repository;
        public string _userId;

        public GLSetupService(string userId)
        {
            if (string.IsNullOrEmpty(_userId)) throw new ApplicationException("Cannot access service, employee id is required.");

            _repository = new GLSetupRepository();
            _userId = userId;
        }

        public void RemoveHeader(int _docEntry)
        {
            if (_docEntry.Equals(default)) throw new ApplicationException("Header not found.");

            _repository.RemoveHeader(_docEntry, _userId);   
        }

        public void RemoveDetails(IEnumerable<GLSetupDetailsModel> _details)
        {
            if (_details == null) throw new ApplicationException("Data not found.");
            if (_details.Any(x => x.Id.Equals(default))) throw new ApplicationException("Detail not found.");
            if (_details.Any(x => !x.IsActive)) throw new ApplicationException("Detail already removed.");

            _repository.RemoveDetails(_details);
        }

        public void UpdateHeader(GLSetupHeaderModel _header)
        {
            if (_header == null) throw new ApplicationException("Data not found.");
            if (string.IsNullOrEmpty(_header.Segment_0)) throw new ApplicationException("GL not found.");

            _repository.UpdateHeader(_header);  
        }

        public void UpdateDetails(IEnumerable<GLSetupDetailsModel> _details)
        {
            if (_details == null) throw new ApplicationException("Data not found.");
            if (_details.Any(x => x.Id.Equals(default))) throw new ApplicationException("Detail not found.");
            if (_details.Any(x => !x.IsActive)) throw new ApplicationException("Cannot update removed detail.");

            _repository.UpdateDetails(_details);
        }

        public int PostSetup (GLSetupView _setup)
        {
            if (_setup == null || _setup.Header == null) throw new ApplicationException("Data not found.");
            if (!_setup.Header.DocEntry.Equals(default)) throw new ApplicationException("Setup already exists.");
            if (string.IsNullOrEmpty(_setup.Header.Segment_0)) throw new ApplicationException("GL not found.");

            _setup.Header.DocEntry = _repository.PostHeader(_setup.Header);

            if (!_setup.Header.DocEntry.Equals(default)) throw new ApplicationException("Error in creating header.");
            else
            {
                var details = PostDetails(_setup.Details, _setup.Header.DocEntry);
                /**/
                var logs = details.Select(x => new GLSetupLogsModel
                {
                    DocEntry = x.DocEntry,
                    Line_ID = x.Line_ID,
                    Status = true, //default active if inserted
                    IsRequired = x.IsRequired,
                    Operator = x.Operator,
                    IsColumn = x.IsColumn,
                    Value = x.Value,
                    Number = x.SequenceNumber,
                    PropertyId = x.Id,
                    ModifiedBy = _userId,
                    //add userIP if applicable
                });

                _repository.InsertLogs(logs);
            }

            return _setup.Header.DocEntry;
        }

        public IEnumerable<GLSetupDetailsModel> PostDetails(IEnumerable<GLSetupDetailsModel> _details, int _postedDocEntry)
        {
            if (_details == null) throw new ApplicationException("Data not found.");
            if (_postedDocEntry.Equals(default)) throw new ApplicationException("Error in posting header."); 
            if (_details.Any(x => !x.Id.Equals(default))) throw new ApplicationException("Detail already exists.");

            return _repository.PostDetails(_details, _postedDocEntry);
        }

        public IEnumerable<GLSetupHeaderModel> GetHeaders()
        {
            return _repository.GetHeaders();
        }

        public IEnumerable<GLSetupDetailsModel> GetDetails(int _docEntry)
        {
            return _repository.GetDetails(_docEntry);
        }

        public IEnumerable<GLSetupDetailsModel> GetLineDetails(int _docEntry, int _lineId)
        {
            return _repository.GetLineDetails(_docEntry, _lineId);
        }

        public GLSetupHeaderViewModel GetHeaderByDocentry(int _docEntry)
        {
            return _repository.GetHeaderByDocentry(_docEntry);
        }
    }
}
