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

        public GLSetupDetailsModel GetLineDetails(int _docEntry, int _lineId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_LINE_DETAIL",
                    docEntry = _docEntry,
                    line_id = _lineId
                };

                return cn.QuerySingle<GLSetupDetailsModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void RemoveDetail(int _docEntry, int _lineId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "REMOVE_DETAIL",
                    docEntry = _docEntry,
                    line_id = _lineId
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void UpdateDetail(GLSetupDetailsModel _detail)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "UPDATE_DETAIL",
                    docEntry = _detail.DocEntry,
                    line_id = _detail.Line_ID,
                    isRequired = _detail.IsRequired,
                    detailOperator = _detail.Operator,
                    isColumn = _detail.IsColumn
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        //Properties
        public IEnumerable<GLSetupDetailPropertiesModel> GetProperties(int _docEntry, int _lineId)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_PROPERTIES",
                    docEntry = _docEntry,
                    line_id = _lineId
                };
                return cn.Query<GLSetupDetailPropertiesModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<GLSetupDetailPropertiesModel> GetLineProperties(int _docEntry, int _lineId, int _groupNumber)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "GET_LINE_PROPERTIES",
                    docEntry = _docEntry,
                    line_id = _lineId,
                    groupNumber = _groupNumber
                };
                return cn.Query<GLSetupDetailPropertiesModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void RemoveProperty(int _docEntry, int _lineId, int _groupNumber)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "REMOVE_PROPERTY",
                    docEntry = _docEntry,
                    line_id = _lineId,
                    groupNumber = _groupNumber
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void UpdateProperty(IEnumerable<GLSetupDetailPropertiesModel> _properties)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_BOOKKEEPING))
            {
                var storedProc = "spInternalReconGLSetup";
                var parameter = new
                {
                    mode = "UPDATE_PROPERTIES",
                    detailProperties = _properties.ToDataTable()
                };

                cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        //Posting
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
                        parameter.mode = "POST_DETAILS";
                        parameter.details = _setup.Details.ToDataTable();

                        cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        //insert detail properties
                        parameter.details = new List<GLSetupDetailsModel>().ToDataTable();
                        parameter.mode = "POST_PROPERTIES";
                        parameter.details = _setup.DetailsProperties.ToDataTable();

                        var dataDetailProperties = cn.Query<GLSetupDetailPropertiesModel>(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        // insert logs
                        var logs = new List<GLSetupLogsModel>();
                        foreach (var detail in _setup.Details)
                        {
                            var properties = dataDetailProperties
                                .Where(x => x.DocEntry == detail.DocEntry
                                    && x.Line_ID == detail.Line_ID)
                                .Select(x => new GLSetupLogsModel
                                {
                                    DocEntry = detail.DocEntry,
                                    Line_ID = detail.Line_ID,
                                    Status = true, //default active when created
                                    IsRequired = detail.IsRequired,
                                    Operator = detail.Operator,
                                    IsColumn = detail.IsColumn,
                                    Value = x.Value,
                                    Number = x.Number,
                                    PropertyId = x.Id,
                                    ModifiedBy = _setup.Header.CreatedBy,
                                    UserIP = _setup.UserIP
                                });

                            logs.AddRange(properties);
                        }

                        //changed SP to separate concerns
                        storedProc = "spInternalReconGLSetupLogs";
                        parameter.detailProperties = new List<GLSetupDetailPropertiesModel>().ToDataTable();
                        parameter.mode = "INSERT";
                        parameter.data = logs.ToDataTable();

                        cn.Execute(storedProc, parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
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
    }
}
