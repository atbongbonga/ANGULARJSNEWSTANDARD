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
using AccountingLegacy.Disbursements.Library.COPS.Models;
using AccountingLegacy.Disbursements.Library.COPS.Repositories;
using AccountingLegacy.Disbursements.Library.COPS.ViewModels;

namespace AccountingLegacy.Disbursements.Library.COPS.Repositories
{
    internal class OPRepository
    {
        
        private readonly SERVER server;
        public OPRepository()
        {
            server = new SERVER("OP Posting");
        }

        public void PostOP(OPPostingView op) {
            try
            {
                CommonQryRepository common = new CommonQryRepository();
                using (var sap = new SAPBusinessOne())
                {
                    try
                    {
                        
                        if (sap.Company.InTransaction) 
                        {
                            sap.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        
                        SAPbobsCOM.Payments oPay = (SAPbobsCOM.Payments)(sap.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments));

                        oPay.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
                        oPay.Address = op.OPPostHdr.Addr;
                        oPay.JournalRemarks = op.OPPostHdr.JERemarks;
                        oPay.DocDate = op.OPPostHdr.DocDate;

                        if (op.OPPostHdr.DocType == "S")
                        {
                            oPay.CardCode = op.OPPostHdr.CardCode;
                            oPay.CardName = op.OPPostHdr.CardName;
                            oPay.CardCode = op.OPPostHdr.CardCode;
                        }
                        else {
                            oPay.CardName = op.OPPostHdr.CardName;
                            oPay.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                        }

                        oPay.DocCurrency = "PHP";
                        oPay.Remarks = op.OPPostHdr.Remarks;
                        oPay.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;

                        oPay.TaxDate = op.OPPostHdr.DocDate;

                        if (op.OPPostHdr.ChkNum != null) { oPay.UserFields.Fields.Item("U_ChkNum").Value = op.OPPostHdr.ChkNum; }
                        if (op.OPPostHdr.U_CardCode != null) { oPay.UserFields.Fields.Item("U_CardCode").Value = op.OPPostHdr.U_CardCode; }
                        if (op.OPPostHdr.BranchCode != null) { oPay.UserFields.Fields.Item("U_BranchCode").Value = op.OPPostHdr.BranchCode; }

                        oPay.UserFields.Fields.Item("U_HPDVoucherNo").Value = common.GetVoucherNum(op.OPPostHdr.BranchCode, op.OPPostHdr.DocDate);
                        if (op.OPPostHdr.Ref2 != null) { oPay.Reference2 = op.OPPostHdr.Ref2; }


                        if (op.OPPostHdr.DocType == "S") {

                            #region Supplier

                            if (op.OPPostHdr.PMode.ToUpper() == "BANK TRANSFER")
                            {
                                oPay.TransferAccount = common.GetAccountCode(op.OPPostHdr.Bank + op.OPPostHdr.WhsCode + "000");
                                oPay.TransferSum = op.OPPostHdr.Total;
                                oPay.TransferDate = op.OPPostHdr.DueDate;
                                oPay.PrimaryFormItems.PaymentMeans = SAPbobsCOM.PaymentMeansTypeEnum.pmtBankTransfer;
                            }
                            else {
                                
                                oPay.Checks.Branch = op.OPPostHdr.WhsCode;
                                oPay.Checks.AccounttNum = op.OPPostHdr.WhsCode;
                                oPay.Checks.DueDate = op.OPPostHdr.DocDueDate;
                                oPay.Checks.CountryCode = "PH";

                                oPay.Checks.BankCode = op.OPPostHdr.Bank;
                                oPay.Checks.ManualCheck = SAPbobsCOM.BoYesNoEnum.tNO;
                                oPay.Checks.CheckAccount = common.GetAccountCode(op.OPPostHdr.Bank + op.OPPostHdr.WhsCode + "000");
                                oPay.Checks.CheckSum = op.OPPostHdr.Total;

                            }

                            string _BankID = "";
                            string _CheckAccount = "";
                            for (var ii = 0; ii < op.Supp.Count; ii++) {

                                double _App = op.Supp[ii].App;
                                double _EWTAmt = op.Supp[ii].EWT;
                                double _Net = op.Supp[ii].App - op.Supp[ii].EWT;

                                if (_App + _EWTAmt != 0) {

                                    if (op.Supp[ii].GlAcct != op.OPPostHdr.WhsCode) {
                                        _BankID = "231" + op.OPPostHdr.WhsCode.Substring(op.OPPostHdr.WhsCode.Length - 2);
                                        _CheckAccount = common.GetGLByBankCodeAndBranch(_BankID, op.Supp[ii].GlAcct);
                                    }   
                                }

                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
