using AccountingLegacy.Core.Library;
using AccountingLegacy.Disbursements.Library.Auth;
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
using System.Reflection.Emit;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Diagnostics;
using AccountingLegacy;
using AccountingLegacy.Disbursements.Library.PCF.Models;
using AccountingLegacy.Disbursements.Library.PCF.ViewModels;
using AccountingLegacy.Disbursements.Library.COPS.Repositories;
using System.Data.SqlTypes;
using System.Security.Principal;
using System.Net;
using System.Globalization;
using System.Transactions;
using System.Reflection;

namespace AccountingLegacy.Disbursements.Library.PCF.Repositories
{
    internal class PCFOPRepository
    {

        private readonly SERVER server;
        public PCFOPRepository()
        {
            server = new SERVER("PCF OP Posting");
        }

        public string CheckIfPosted(PCFOPView model)
        {
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_PCFOPValidation";
                var parameters = new
                {
                    Mode = "CheckIfPosted",
                    CardName = model.Header.Payee,
                    DocDate = model.Header.DocDate,
                    Comments = model.Header.Remarks,
                    ChkNo = model.Header.ChkNum,
                    WhsCode = model.Header.BranchCode,
                    Amount = model.Header.Total,
                    PType = model.Header.PType,

                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string CheckLineIfPosted(string Status , string AcctCode , string WhsCode) {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_PCFOPValidation";
                var parameters = new
                {
                    Mode = "CheckLineIfPosted",
                    Status = Status,
                    AcctCode = AcctCode,
                    WhsCode = WhsCode

                };

                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string CheckBankDetail(int OPNum , string BankCode) {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_PCFOPValidation";
                var parameters = new
                {
                    Mode = "CheckBankDetail",
                    OPNum = OPNum,
                    Bank = BankCode

                };
                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string CheckGLIfExist(string AcctCode)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_PCFOPValidation";
                var parameters = new
                {
                    Mode = "CheckGLIfExist",
                    AcctCode = AcctCode

                };
                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string CheckGLIfExistByCodeWhs(string Bank , string WhsCode)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_PCFOPValidation";
                var parameters = new
                {
                    Mode = "CheckGLIfExist",
                    Bank = Bank,
                    WhsCode = WhsCode

                };
                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public string CheckBankOwner(string BankCode)
        {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_PCFOPValidation";
                var parameters = new
                {
                    Mode = "CheckBankOwner",
                    Bank = BankCode

                };
                return cn.QuerySingle<string>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

        public void OPChangesLogs(string UserID) {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var oTransaction = cn.BeginTransaction();
                try
                {
                    var storedProc = "spAL_PCFOPLogs";
                    var parameters = new
                    {
                        Mode = "OPChangeLogs",
                        UserID = UserID

                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    oTransaction.Commit();
                }
                catch (Exception)
                {
                    oTransaction.Rollback();
                    throw;
                }

                
            }
        }

        public void OPPostVPM4Update(int OPNum , List<PCFPayDetail> model , string UserID) {

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {

                var oTransaction = cn.BeginTransaction();
                try
                {
                    var storedProc = "spOPPost";
                    var parameters = new
                    {
                        opnum = OPNum,
                        payee = "",
                        chkRmrks = "N/A",
                        chkprint = "",
                        EmpID = UserID
                    };
                    cn.Execute(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    int line = 0;
                    foreach (var item in model)
                    {

                        storedProc = "spAL_PCFOPLogs";
                        var pcfparam = new
                        {
                            Mode = "InsertVPM4New",
                            OPEntry = OPNum,
                            Line = line,
                            AcctCode = item.AcctCode,
                            AcctName = item.AcctName,
                            Amt = item.Amt,
                            VAT = item.VAT,
                            MnlVAT = item.MnlVAT,
                            NetVAT = item.NetVAT,
                            Descrip = item.Descr,
                            WhsCode = item.WhsCode,
                            ATCCode = item.ATCCode,
                            WTax = item.WTax,
                            ATCRate = item.ATCRate,
                            TaxGrp = item.TaxGrp,
                        };
                        cn.Execute(storedProc, pcfparam, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                        line += 1;
                    }

                    oTransaction.Commit();
                }
                catch (Exception)
                {
                    oTransaction.Rollback();
                    throw;
                }
             
            }
        }

        public int UpsertPCFDetails(PCFOPView model) {

            using (IDbConnection cn = new SqlConnection(server.EMS_HPCOMMON))
            {
                var oTransction = cn.BeginTransaction();
                try
                {
                    var storedProc = "spAL_PCFOPPosting";
                    var parameters = new
                    {
                        Mode = "InsertOVPM",
                        DocDate = model.Header.DocDate,
                        WhsCode = model.Header.WhsCode,
                        PayTo = model.Header.Addr,
                        Remarks = model.Header.Remarks,
                        BankName = model.Header.BankName,
                        BankCode = model.Header.Bank,
                        Vendors = model.Header.Payee,
                        Stat = "Posted",
                        WhsV = model.Header.BranchCode,
                        RType = "R",
                        SapEntry = model.Header.OPNum,
                        Custodian = model.Header.Custodian,
                        PostBy = model.Header.PostBy,
                        TotalAmt = model.Header.Total,
                        SalesInvDate = "01/01/1900",
                        SalesInvDateReceive = "01/01/1900",
                        Checknum = model.Header.ChkNum == null ? SqlString.Null : model.Header.ChkNum,
                        Si = SqlString.Null,
                        VFrom = "01/01/1900",
                        VTo = "01/01/1900"

                    };
                    int _OPEntry = Convert.ToInt32(cn.ExecuteScalar(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0));


                    foreach (var item in model.Detail) {

                        storedProc = "spAL_PCFOPPosting";
                        var pcfparam = new
                        {
                            Mode = "UpdatePCF",
                            DocNum = item.DocNum,
                            OPEntry = _OPEntry,
                            DocEntry = item.DocEntry
                        };
                        cn.Execute(storedProc, pcfparam, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    }

                    oTransction.Commit();
                    return _OPEntry;
                }
                catch (Exception)
                {
                    oTransction.Rollback();
                    throw;
                }
                
             }
        
        }
        public int PostPCFOP(PCFOPView model) {

            using (var sap = new SAPBusinessOne()) {

                if (sap.Company.InTransaction) sap.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                long LRet;
                sap.Company.StartTransaction();
                SAPbobsCOM.Payments oPay = (SAPbobsCOM.Payments)(sap.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments));
                try
                {
                    CommonQryRepository common = new CommonQryRepository();
                    oPay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                    oPay.CardCode = "";
                    oPay.CardName = model.Header.Payee;
                    oPay.Address = model.Header.Addr;
                    oPay.JournalRemarks = model.Header.Payee;
                    oPay.DocDate = model.Header.DocDate;
                    oPay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                    oPay.DocCurrency = "PHP";
                    oPay.Remarks = model.Header.Remarks;
                    oPay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    oPay.TaxDate = model.Header.DocDate;
                    if (model.Header.ChkNum != null) { oPay.UserFields.Fields.Item("U_ChkNum").Value = model.Header.ChkNum; }
                    if (model.Header.BranchCode != null) { oPay.UserFields.Fields.Item("U_BranchCode").Value = model.Header.BranchCode; }
                    oPay.UserFields.Fields.Item("U_HPDVoucherNo").Value = common.GetVoucherNum(model.Header.BranchCode, model.Header.DocDate);

                    oPay.Checks.Branch = model.Header.WhsCode;
                    oPay.Checks.AccounttNum = model.Header.WhsCode;
                    oPay.Checks.DueDate = model.Header.DocDate;
                    oPay.Checks.CountryCode = "PH";
                    oPay.Checks.BankCode = model.Header.Bank;
                    oPay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                    oPay.Checks.CheckAccount = common.GetAccountCode(model.Header.Bank + model.Header.WhsCode + "000");
                    oPay.Checks.CheckSum = model.Header.Total - (model.Detail.Where(x => !string.IsNullOrEmpty(x.ATCCode) && x.WTax != 0).Select(x => x.WTax).DefaultIfEmpty().Sum());
                    oPay.Checks.Add();

                    string BankOwner = CheckBankOwner(model.Header.Bank); //Check if has existing bank owner , raise error if not setup found.
                    int i = 0;
                    int ln = 0;
                    foreach (var item in model.Detail) {

                        item.AcctCode = item.AcctCode + item.WhsCode + "000";
                        if (item.Amt != 0)
                        {
                            if (ln != 0) { oPay.AccountPayments.Add(); }

                            CheckGLIfExist(item.AcctCode);
                            oPay.AccountPayments.AccountCode = common.GetAccountCode(item.AcctCode);//sap.GetAcct(item.AcctCode);
                            oPay.AccountPayments.SumPaid = item.Amt;
                            if (item.Descr.Length > 250)
                            {
                                item.Descr = item.Descr.Substring(0, 250);
                            }
                            oPay.AccountPayments.Decription = item.Descr;
                            oPay.AccountPayments.UserFields.Fields.Item("U_DocLine").Value = i.ToString();
                            ln += 1;

                            // Post Total EWT
                            if (!string.IsNullOrEmpty(item.ATCCode) && item.WTax != 0)
                            {
                                if (ln != 0) { oPay.AccountPayments.Add(); }
                                oPay.AccountPayments.AccountCode = common.GetAccountCode("25200" + item.WhsCode + "000");
                                oPay.AccountPayments.SumPaid = -item.WTax;
                                oPay.AccountPayments.Decription = "WTAX - " + item.ATCCode;
                              
                                ln += 1;
                            }

                            string DueTo = "";
                            string Advances = "";

                            if (item.WhsCode != BankOwner) {

                                CheckGLIfExistByCodeWhs(model.Header.Bank, item.WhsCode);
                                DueTo = item.WhsCode;
                                Advances = BankOwner;

                                if (DueTo != "")
                                {

                                    //Advances GL Account
                                    CheckGLIfExist("132" + item.WhsCode.Substring(item.WhsCode.Length - 2) + BankOwner + "000");
                                    string AdvanceAcct = common.GetAccountCode("132" + item.WhsCode.Substring(item.WhsCode.Length - 2) + BankOwner + "000");

                                    oPay.Checks.Branch = model.Header.WhsCode;
                                    oPay.Checks.AccounttNum = model.Header.WhsCode;
                                    oPay.Checks.DueDate = model.Header.DocDate;
                                    oPay.Checks.CountryCode = "PH";
                                    oPay.Checks.BankCode = "132" + item.WhsCode.Substring(item.WhsCode.Length - 2);
                                    oPay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                                    oPay.Checks.CheckAccount = AdvanceAcct;
                                    oPay.Checks.CheckSum = -(item.Amt - (string.IsNullOrEmpty(item.ATCCode) ? 0 : item.WTax));
                                    oPay.Checks.Add();

                                    //DueTo GL Account
                                    CheckGLIfExist("231" + BankOwner.Substring(BankOwner.Length - 2) + item.WhsCode + "000");
                                    string DueToAcct = common.GetAccountCode("231" + BankOwner.Substring(BankOwner.Length - 2) + item.WhsCode + "000");

                                    oPay.Checks.Branch = model.Header.WhsCode;
                                    oPay.Checks.AccounttNum = model.Header.WhsCode;
                                    oPay.Checks.DueDate = model.Header.DocDate;
                                    oPay.Checks.CountryCode = "PH";
                                    oPay.Checks.BankCode = "231" + BankOwner.Substring(BankOwner.Length - 2);
                                    oPay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                                    oPay.Checks.CheckAccount = DueToAcct;
                                    oPay.Checks.CheckSum = item.Amt - (!string.IsNullOrEmpty(item.ATCCode) ? item.WTax : 0);
                                    oPay.Checks.Add();

                                }
                            }

                        }

                        i += 1;
                    }
                    LRet = oPay.Add();

                    if (LRet == 0)
                    {
                        if (sap.Company.InTransaction) { sap.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit); }
                        string opnum = sap.Company.GetNewObjectKey();

                        OPPostVPM4Update(Convert.ToInt32(opnum), model.Detail , model.Header.PostBy); // Execute stored proc. for table update
                        return Convert.ToInt32(opnum);
                    }
                    else {
                        if (sap.Company.InTransaction) { sap.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }

                }
                catch (Exception)
                {
                   throw;
                }
            }
            
        }

        public int UpatePCFOP(PCFView model)
        {
            try
            {
                using (var sap = new SAPBusinessOne())
                {
                    long LRet;
                    SAPbobsCOM.Payments oPay = (SAPbobsCOM.Payments)(sap.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments));
                    oPay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;

                    if (oPay.GetByKey(model.Header.OPNum) == false)
                    {
                        throw new ApplicationException("OPentry not found in SAP");
                    }
                    oPay.Reference2 = model.Header.DocEntry.ToString();
                    oPay.UserFields.Fields.Item("U_CardCode").Value = "PCF-" + model.Header.DocEntry.ToString();
                    LRet = oPay.Update();
                    if (LRet == 0)
                    {
                        return Convert.ToInt32(model.Header.OPNum);
                    }
                    else {
                        if (sap.Company.InTransaction) { sap.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
                        throw new ApplicationException(sap.Company.GetLastErrorDescription());
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
          
        }

    }
}
