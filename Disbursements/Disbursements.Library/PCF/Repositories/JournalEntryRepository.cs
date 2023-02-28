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

                using (var sap = new SAPBusinessOne())
                {
                    var entry = sap.JournalEntries;
                    entry.ReferenceDate = data.Header.RefDate;
                    entry.Memo = data.Header.Memo.Trim();
                    entry.Reference = data.Header.Ref1.Trim();
                    entry.Reference2 = data.Header.Ref2.Trim();
                    if(data.Header.Ref3 is not null) entry.Reference3 = data.Header.Ref3.Trim();

                    foreach (var item in data.Details)
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

                    int returnValue = entry.Add();
                    if (returnValue == 0)
                    {
                        var transId = Convert.ToInt32(sap.Company.GetNewObjectKey());
                        //confirm if not need pcfop
                        if (int.Parse(IsJEUpdated(transId)) == 1)
                        {
                          throw new ApplicationException(PcfBuilder.IsJEUpdated());
                        }
                        else {
                            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                            {
                                var storedProc = PcfBuilder.spPcfJE();
                                var parameters = new
                                {
                                    mode = PcfBuilder.spModeJEUpdateTables(),
                                    transId = transId,
                                    pcfOP = data.Header.PCFOP,
                                    pcfDoc = data.Header.PCFDoc,
                                };

                                cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                            }
                        }



                        return transId;
                       

                    }
                    else
                    {

                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
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
