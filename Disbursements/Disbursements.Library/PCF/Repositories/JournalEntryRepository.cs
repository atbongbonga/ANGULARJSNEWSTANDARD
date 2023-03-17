using AccountingLegacy;
using AccountingLegacy.Core.Library;
using Core.Library.Models;
using Disbursements.Library.PCF.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SAPbobsCOM;
using Disbursements.Library.PCF.Helpers;
using Disbursements.Library.PCF.Models;
using System.Reflection.PortableExecutable;
using System.Numerics;
using System.Security.Principal;
using MoreLinq;

namespace Disbursements.Library.PCF.Repositories
{
    internal class JournalEntryRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public JournalEntryRepository(string empCode = "")
        {
            server = new SERVER(PcfBuilder.PCFSERVER());
            this.empCode = empCode;
        }
        public int PostJrnlEntry(JrnlEntryView data)
        {
            try
            {

                
                var docEntry = UpdateData(data);
                var jrnlEntry = GetTemplate(docEntry);

                using (var sap = new SAPBusinessOne())
                {
                    sap.BeginTran();
                    var entry = sap.JournalEntries;
                    entry.ReferenceDate = jrnlEntry.Header.DocDate;
                    entry.Memo = jrnlEntry.Header.Memo.Trim();
                    entry.Reference = jrnlEntry.Header.Ref1.Trim();
                    entry.Reference2 = jrnlEntry.Header.Ref2.Trim();
                    entry.UserFields.Fields.Item("U_FTDocNo").Value = docEntry.ToString();
                    if(jrnlEntry.Header.Ref3 is not null) entry.Reference3 = jrnlEntry.Header.Ref3.Trim();

                    foreach (var item in jrnlEntry.Details)
                    {
                        entry.Lines.AccountCode = item.Account;
                        entry.Lines.Debit = Convert.ToDouble(item.Debit);
                        entry.Lines.Credit = Convert.ToDouble(item.Credit);
                        entry.Lines.LineMemo = item.LineMemo;
                        entry.Lines.ShortName = item.ShortName;
                        entry.Lines.Reference1 = item.Ref1;
                        entry.Lines.Reference2 = item.Ref2;
                        entry.Lines.Add();
                    }
                    var returnValue = entry.Add();

                    if (returnValue == 0)
                    {
                        var transId = Convert.ToInt32(sap.Company.GetNewObjectKey());

                        using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                        {
                            var storedProc = "spPCFPosting";
                            var parameters = new
                            {
                                mode = "POST_JE",
                                transId = transId,
                                pcfOP = jrnlEntry.Header.PCFOP,
                                pcfDoc = jrnlEntry.Header.Ref2.Trim(),
                                empCode = empCode,
                                docEntry = docEntry
                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        }
                        using (IDbConnection cn = new SqlConnection(server.EMS_HPCOMMON))
                        {
                            var storedProc = PcfBuilder.spPcfJE1051();
                            var parameters = new
                            {
                                mode = PcfBuilder.spModeJEUpdateTables(),
                                transId = transId,
                                pcfOP = jrnlEntry.Header.PCFOP,
                                pcfDoc = jrnlEntry.Header.Ref2.Trim(),

                            };

                            cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        }
                        sap.Commit();
                        return transId;
                    }
                    else
                    {
                        var err = sap.Company.GetLastErrorDescription();
                        sap.Rollback();
                        throw new ApplicationException(err);
                    }

                }
            }
            catch (Exception ex)
            {
               
                LogError(new PCFErrorLogs
                {
                    Module = "PCF POST JE",
                    ErrorMsg = ex.GetBaseException().Message
                });
                
                throw;
            }
        }

        private int UpdateData(JrnlEntryView jrnlEntry)
        {
            var output = new JrnlEntryView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                List<JournalEntryHeaderView> headerTable = new List<JournalEntryHeaderView>();

                headerTable.Add(jrnlEntry.Header);

                return cn.ExecuteScalar<int>(
                    "spPCFPosting",
                    new
                    {
                        mode = "UPDATE_JE",
                        header = headerTable.ToDataTable(),
                        details = jrnlEntry.Details.ToDataTable(),
                        //docEntry = jrnlEntry.Header.Docentry
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
               
            }
        }

        private JrnlEntryView GetTemplate(int docEntry)
        {

            var output = new JrnlEntryView();
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {

                using (var multi = cn.QueryMultiple
                (
                    "spPCFPosting",
                    new
                    {
                        mode = "GET_JE_TEMPLATE",
                        docEntry = docEntry,
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0)
                )
                {
                    output.Header =  multi.Read<JournalEntryHeaderView>().Single();
                    output.Details = multi.Read<JournalEntrDetailView>();
                    return output;
                }
            }

    
        }
        private List<int> GetDocentriesByIp(string ipaddress)
        {
            using (IDbConnection cn = new SqlConnection(server.EMS_HPCOMMON))
            {

                return cn.Query<int>(
                    "spPCFPosting",
                    new
                    {
                        mode = "UPDATE_JE",
                        ipaddress
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();

            }

        }



        public string GetAcctCodeByFormatCode(string acctcode)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                return cn.QueryFirstOrDefault<string>(PcfBuilder.GetAcctCodeByFormatCode(acctcode));
            }
        }


        public string IsJEUpdated(int transid)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
            {
                return cn.QueryFirstOrDefault<string>(PcfBuilder.IsJEUpdated( transid));
            }
        }



        private void LogError(PCFErrorLogs log)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                cn.Execute(
                    "spPCFJEError",
                    new
                    {
                        mode = "INSERT",
                        module = log.Module,
                        message = log.ErrorMsg,
                        docEntry = log.DocEntry,
                        remarks = log.Remarks,
                        empCode = this.empCode
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }






    }
}
