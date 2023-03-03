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

namespace Disbursements.Library.PCF.Repositories
{
    internal class JournalEntryRepository
    {
        private readonly SERVER server;
        private readonly string empCode;
        public JournalEntryRepository(string empCode = "")
        {
            server = new SERVER("PCF JE");
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
                    entry.Reference3 = data.Header.Ref3.Trim();

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
                        return transId;
                        //using (IDbConnection cn = new SqlConnection(server.SAP_PF))
                        //{
                        //    var storedProc = "spProfFees";
                        //    var parameters = new
                        //    {
                        //        mode = "POST_CA",
                        //        docEntry = docEntry,
                        //        transId = transId,
                        //        empCode = empCode,
                        //    };

                        //    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        //}

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

  
        public string GettAcctCodeByFormatCode(string acctcode)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                return cn.QueryFirstOrDefault<string>(PcfBuilder.GetAcctCodeByFormatCode(acctcode));
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
