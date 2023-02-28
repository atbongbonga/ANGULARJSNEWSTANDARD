using AccountingLegacy.Core.Library;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using MoreLinq;
using AccountingLegacy;
using AccountingLegacy.Disbursements.Library.PCF.Models;
using AccountingLegacy.Disbursements.Library.PCF.ViewModels;
using System.Data.SqlTypes;
using Core.Library.Models;

namespace AccountingLegacy.Disbursements.Library.PCF.Repositories
{
    internal class PCFOPRepository
    {

        private readonly SERVER server;
        public PCFOPRepository()
        {
            server = new SERVER("PCF OP Posting");
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

                    PCFHeaderPostTemplate Header = GetPostTemplateHeader(model);
                 
                    oPay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                    oPay.CardCode = "";
                    oPay.CardName = Header.Payee;
                    oPay.Address = Header.Addr;
                    oPay.JournalRemarks = Header.Payee;
                    oPay.DocDate = Header.DocDate;
                    oPay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                    oPay.DocCurrency = "PHP";
                    oPay.Remarks = Header.Remarks;
                    oPay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    oPay.TaxDate = Header.DocDate;
                    oPay.UserFields.Fields.Item("U_ChkNum").Value = Header.ChkNum;
                    oPay.UserFields.Fields.Item("U_BranchCode").Value = Header.BranchCode;
                    oPay.UserFields.Fields.Item("U_HPDVoucherNo").Value = Header.VoucherNo;

                    oPay.Checks.Branch = Header.WhsCode;
                    oPay.Checks.AccounttNum = Header.WhsCode;
                    oPay.Checks.DueDate = Header.DocDate;
                    oPay.Checks.CountryCode = "PH";
                    oPay.Checks.BankCode = Header.Bank;
                    oPay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                    oPay.Checks.CheckAccount = Header.CheckAccount;
                    oPay.Checks.CheckSum = Convert.ToDouble(Header.SummApplied);
                    oPay.Checks.Add();

                    int i = 0;
                    foreach (var item in model.Detail) {
                        // Get Template for Posting
                        PCFCheckAcct CheckAcct = GetPostTemplateDetail(model.Header, item);

                        foreach (var accountitem in CheckAcct.Accounts) {

                            oPay.AccountPayments.AccountCode = accountitem.AcctCode;
                            oPay.AccountPayments.SumPaid = Convert.ToDouble(accountitem.SumApplied);
                            oPay.AccountPayments.Decription = accountitem.Description;
                            oPay.AccountPayments.UserFields.Fields.Item("U_DocLine").Value = i.ToString();
                        }

                        foreach (var checkitem in CheckAcct.Checks) {

                            oPay.Checks.Branch = checkitem.Branch;
                            oPay.Checks.AccounttNum = checkitem.Branch;
                            oPay.Checks.DueDate = checkitem.DueDate;
                            oPay.Checks.CountryCode = checkitem.CountryCode;
                            oPay.Checks.BankCode = checkitem.BankCode;
                            oPay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                            oPay.Checks.CheckAccount = checkitem.CheckAccount;
                            oPay.Checks.CheckSum = Convert.ToDouble(checkitem.Amount);
                            oPay.Checks.Add();
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

        public PCFCheckAcct GetPostTemplateDetail(PCFPayHeader Header , PCFPayDetail Detail) {
          
            PCFCheckAcct model = new PCFCheckAcct();
            List<PCFInputsDetail> detail = new List<PCFInputsDetail>();
         
            detail.Add(new PCFInputsDetail
            {
                SAP = Detail.SAP,
                AcctCode = Detail.AcctCode ,
                WhsCode = Detail.WhsCode , 
                ATCCode = Detail.ATCCode ,
                WTax = Detail.WTax ,
                Amt = Detail.Amt ,
                Descr = Detail.Descr
            });
            
            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var reader = cn.QueryMultiple("spAL_GetPCFOPPostTemplate", 
                             param: new 
                             { Mode = "GetPostTemplateDetail",
                               Bank = Header.Bank,
                               PCFOPDetail = detail.ToDataTable()
                             }, commandType: CommandType.StoredProcedure);
                model.Accounts = reader.Read<PaymentAccount>().ToList();
                model.Checks = reader.Read<PCFPaymentChecks>().ToList();
            }
            return model;
        }
        public PCFHeaderPostTemplate GetPostTemplateHeader(PCFOPView model) {

            List<PCFInputsHeader> Input = new List<PCFInputsHeader>();

            Input.Add(new PCFInputsHeader
            {
                Payee = model.Header.Payee,
                Addr = model.Header.Addr,
                Remarks = model.Header.Remarks,
                DocDate = model.Header.DocDate,
                ChkNum = model.Header.ChkNum,
                BranchCode = model.Header.BranchCode,
                WhsCode = model.Header.WhsCode ,
                Bank = model.Header.Bank,
                Total = Convert.ToDecimal(model.Header.Total),
                TaxAmount = Convert.ToDecimal(model.Detail.Where(x => !string.IsNullOrEmpty(x.ATCCode) && x.WTax != 0).Select(x => x.WTax).DefaultIfEmpty().Sum()),
                PostBy = model.Header.PostBy

            });

            using (IDbConnection cn = new SqlConnection(server.SAP_DISBURSEMENTS))
            {
                var storedProc = "spAL_GetPCFOPPostTemplate";
                var parameters = new
                {
                    mode = "GetPostTemplateHeader",
                    CardName = model.Header.Payee,
                    DocDate = model.Header.DocDate,
                    Comments = model.Header.Remarks,
                    ChkNo = model.Header.ChkNum,
                    WhsCode = model.Header.BranchCode,
                    Amount = model.Header.Total,
                    @PCFOPHeader = Input.ToDataTable()
                };
                return cn.QuerySingle<PCFHeaderPostTemplate>(storedProc, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
            }
        }

    }
}
