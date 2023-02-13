using AccountingLegacy.Core.Library;
using AccountingLegacy.Disbursements.Library.Auth;
using AccountingLegacy.Disbursements.Library.COPS.Models;
using AccountingLegacy.Disbursements.Library.COPS.ViewModels;
using AccountingLegacy.Disbursements.Library.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MoreLinq;
using SAPbobsCOM;
using Disbursements.Library.COPS.Models;

namespace AccountingLegacy.Disbursements.Library.COPS.Repositories
{
    internal class AgencyRepository
    {
        private readonly SERVER server;
        public AgencyRepository()
        {
            server = new SERVER("Agencies Importation");
        }

        public IEnumerable<AgenciesImportedView> GetAgencies(string genId, string bankType, string brCode)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAgenciesImported";
                var parameters = new { 
                    mode = "GET_AGENCIES", 
                    genId = genId, 
                    bankType = bankType, 
                    branch = brCode 
                };
                return cn.Query<AgenciesImportedView>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void PostAgencies(IEnumerable<AgenciesImportedModel> agencies)
        {

            using (var sap = new SAPBusinessOne())
            {

                try
                {
                    var payment = sap.VendorPayments;
                    var jrnlEntry = sap.JournalEntries;

                    sap.BeginTran();

                    //POST CA
                


                    //POST OA

                    //POST JE

                    //UPDATE DOCNUMs

                    sap.Commit();
                }
                catch (Exception ex)
                {
                    sap.Rollback();
                    throw ex;
                }
            }

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAgenciesImported";
                var parameters = new
                {
                    mode = "POST_AGENCIES",
                    data = agencies.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void RemoveAgencies(IEnumerable<AgenciesImportedModel> agencies)
        {
            using(IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAgenciesImported";
                var parameters = new
                {
                    mode = "REMOVE_AGENCIES",
                    data = agencies.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void SaveAgencies(IEnumerable<AgenciesImportedModel> agencies)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAgenciesImported";
                var parameters = new {
                    mode = "SAVE_AGENCIES",
                    data = agencies.ToDataTable()
                };
                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void AddExcelSetup(IEnumerable<AgenciesExcelSetupModel> setup)
        {

        }

        public void UpdateExcelSetup(AgenciesExcelSetupModel setup)
        {

        }

        public void RemoveExcelSetup(IEnumerable<AgenciesExcelSetupModel> setup)
        {

        }

        public IEnumerable<AgenciesExcelSetupModel> GetExcelSetup()
        {
            throw new NotImplementedException();
        }
    }
}
