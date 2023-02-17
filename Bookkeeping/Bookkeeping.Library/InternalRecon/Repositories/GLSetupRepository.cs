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

        //Header
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

        public int PostHeader(GLSetupHeaderModel _header)
        {
            var headers = new List<GLSetupHeaderModel> { _header };

            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "POST_HEADER",
                    headers = headers.ToDataTable()
                };

                return cn.ExecuteScalar<int>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        //Details
        public IEnumerable<GLSetupDetailsModel> GetDetails(int _docEntry)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_DETAILS",
                    docEntry = _docEntry
                };
                return cn.Query<GLSetupDetailsModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<GLSetupDetailsModel> GetLineDetails(int _docEntry, int _lineId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_LINE_DETAILS",
                    docEntry = _docEntry,
                    line_id = _lineId
                };

                return cn.QuerySingle<GLSetupDetailsModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void RemoveDetails(IEnumerable<GLSetupDetailsModel> _details)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "REMOVE_DETAILS",
                    details = _details.ToDataTable()
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void UpdateDetails(IEnumerable<GLSetupDetailsModel> _details)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "UPDATE_DETAILS",
                    details = _details.ToDataTable()
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<GLSetupDetailsModel> PostDetails(IEnumerable<GLSetupDetailsModel> _details, int _postedDocEntry)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "POST_DETAILS",
                    details = _details.ToDataTable(),
                    docEntry = _postedDocEntry
                };

                return cn.Query<GLSetupDetailsModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        //Logs
        public void InsertLogs(IEnumerable<GLSetupLogsModel> _logs)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetupLogs";
                var parameter = new
                {
                    data = _logs.ToDataTable()
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }
    }
}
