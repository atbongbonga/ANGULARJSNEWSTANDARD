using AccountingLegacy;
using AccountingLegacy.Core.Library;
using Disbursements.Library.COPS.ViewModels.JGMAN;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SAPbobsCOM;
using System.Net;

namespace Disbursements.Library.COPS.Repositories
{
    internal class JGMANRepository
    {
        private readonly SERVER server;
        private readonly string userCode;
        public JGMANRepository(string userCode = "")
        {
            server = new SERVER("COPS - JGMAN");
            this.userCode = userCode;
        }

        public void PostPayments(IEnumerable<SummaryView> data)
        {
            foreach (var item in data)
            {
                try
                {
                    var result = GetPaymentView(item);
                    using (var sap = new SAPBusinessOne("172.30.1.167"))
                    {
                        var pay = sap.VendorPayments;
                        pay.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;
                        pay.DocType = BoRcptTypes.rAccount;
                        pay.DocDate = result.Header.DocDate;
                        pay.CardName = result.Header.CardName;
                        pay.TransferAccount = result.Header.AcctCode;
                        pay.DocCurrency = "PHP";
                        pay.JournalRemarks = result.Header.JrnlMemo;
                        pay.Remarks = result.Header.Comments;
                        pay.Reference2 = result.Header.Ref2;
                        pay.UserFields.Fields.Item("U_ChkNum").Value = result.Header.U_ChkNum;
                        pay.UserFields.Fields.Item("U_CardCode").Value = result.Header.U_CardCode;
                        pay.UserFields.Fields.Item("U_BranchCode").Value = result.Header.U_BranchCode;
                        pay.UserFields.Fields.Item("U_HPDVoucherNo").Value = result.Header.U_HPDVoucherNo;

                        foreach (var account in result.Accounts)
                        {
                            pay.AccountPayments.AccountCode = account.AcctCode;
                            pay.AccountPayments.Decription = account.Description;
                            pay.AccountPayments.SumPaid = (double)account.SumApplied;
                            pay.AccountPayments.Add();
                        }

                        if (pay.Add() == 0)
                        {
                            var docNum = Convert.ToInt32(sap.Company.GetNewObjectKey());

                            //CURRENT UPDATES
                            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
                            {
                                cn.Execute("spJGMAN", new
                                {
                                    mode = "POST_PAYMENT",
                                    docNum = docNum,
                                    genId = item.GenId,
                                    branch = item.BrCode,
                                    acctType = item.AcctType,
                                }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                            }

                            //OLD UPDATES
                            using (IDbConnection cn = new SqlConnection(server.SAP_HPCOMMON))
                            {
                                cn.Execute("spOPPost", new
                                {
                                    opnum = docNum,
                                    payee = result.Header.CardName,
                                    chkRmrks = "",
                                    chkprint = "N/A",
                                    EmpID = userCode,
                                    PMStat = "For Collection",
                                    PMDate = result.Header.DocDate,
                                    CAOAres = 0,
                                }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                            }
                        }
                        else
                        {
                            throw new ApplicationException(sap.Company.GetLastErrorDescription());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log error
                }
            }
        }

        private PaymentView GetPaymentView(SummaryView record)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spJGMAN";
                var parameters = new
                {
                    mode = "PAYMENT_TEMPLATE",
                    genId = record.GenId,
                    branch = record.BrCode,
                    acctType = record.AcctType,
                    docDate = record.DocDate,
                    startDate = record.StartDate,
                    endDate = record.EndDate
                };
                using (var result = cn.QueryMultiple(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0))
                {
                    return new PaymentView
                    {
                        Header = result.Read<PaymentHeaderView>().Single(),
                        Accounts = result.Read<PaymentAccountView>()
                    };
                }
            }
        }

        public IEnumerable<SummaryView> GetSummary(string genId, string acctType, bool active)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                return cn.Query<SummaryView>("spJGMAN", new
                {
                    mode = "GET_JGMAN",
                    genId = genId,
                    acctType = acctType,
                    active = active
                }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public IEnumerable<DetailView> GetDetails(string genId, string acctType, string brCode)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                return cn.Query<DetailView>("spJGMAN", new
                {
                    mode = "GET_JGMAN_DETAILS",
                    genId = genId,
                    acctType = acctType,
                    branch = brCode
                }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

    }
}
