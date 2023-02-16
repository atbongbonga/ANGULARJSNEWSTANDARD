using AccountingLegacy;
using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.ViewModels;
using Dapper;
using MoreLinq;
using System.Data;
using System.Data.SqlClient;

namespace Bookkeeping.Library.InternalRecon.Repositories
{
    internal class GLSetupRepository
    {
        private readonly SERVER server;
        public GLSetupRepository()
        {
            server = new SERVER("GL Setup");
        }

        #region HEADER CRUD

        public IEnumerable<GLSetupHeaderViewModel> GetHeaders()
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_HEADERS"
                };
                return cn.Query<GLSetupHeaderViewModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public GLSetupHeaderViewModel GetHeaderByDocentry(int _docEntry)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storeProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_HEADER_BY_DOCENTRY",
                    docEntry = _docEntry
                };
                return cn.QuerySingle<GLSetupHeaderViewModel>(storeProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void RemoveHeader(int _docEntry, string _modifiedBy)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storeProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "REMOVE_HEADER",
                    docEntry = _docEntry,
                    modifiedBy = _modifiedBy
                };
                cn.Execute(storeProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void UpdateHeader(GLSetupHeaderModel _header)
        {
            var headers = new List<GLSetupHeaderModel>
            {
                _header
            };

            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "UPDATE_HEADER",
                    header = headers.ToDataTable()
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public int PostSetup(GLSetupView _setup)
        {
            var headers = new List<GLSetupHeaderModel>
            {
                _setup.Header
            };

            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var transaction = cn.BeginTransaction(0);

                try
                {
                    var storedProc = "spInternalReconGLSetup";
                    var parameter = new PostGLSetupParameterHelper
                    {
                        mode = "POST_HEADER",
                        header = headers.ToDataTable()
                    };

                    _setup.Header.DocEntry = cn.ExecuteScalar<int>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    if (_setup.Header.DocEntry == 0) throw new ApplicationException("Couldn't create header.");
                    else
                    {
                        //insert details
                        parameter.header = new List<GLSetupHeaderModel>().ToDataTable();
                        parameter.mode = "POST_DETAIL";
                        parameter.details = _setup.Details.ToDataTable();

                        cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        //insert detail properties

                        // insert logs
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }

            return _setup.Header.DocEntry;
        }

        #endregion

    }
}
