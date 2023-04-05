using AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories;
using Disbursements.Library.PaymentRequisition.Models;

namespace Disbursements.Library.PaymentRequisition.Services
{
    public class PaymentService
    {
        private PaymentRepository repo = new PaymentRepository();
        private readonly string userCode;
        public PaymentService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new AccountingLegacy.Disbursements.Library.PaymentRequisition.Repositories.PaymentRepository(userCode);
        }
        public void PostPayment(PaymentView Model)
        {
            try
            {
                if (Model is null || Model.Header is null) throw new ApplicationException("Data not found.");
                if (Model.Header.U_BranchCode.Substring(Model.Header.U_BranchCode.Length - 2) != Model.Header.WhsCode.Substring(Model.Header.WhsCode.Length - 2)) {
                    throw new ApplicationException("Please check Whscode and Whscode voucher");
                }

                if (Model.Header.BankCode == "") throw new ApplicationException("Bank is required...");
                if (Model.Header.WhsCode == "") throw new ApplicationException("Destination branch is Required...");
                if (Model.Header.Comments == "") throw new ApplicationException("Remarks is Required...");

                if (Model.Header.DocTotal > 200000 && Model.Header.CheckPrint != "Manual Check" && Model.Header.Bank == "UBP") 
                throw new ApplicationException("Transaction with 200,000 and above must be Manual Check");


                if (Model.Header.PayOnAccount) {
                    if (Model.Header.ControlAccount.Substring(5, 3) != Model.Header.WhsCode) {
                        throw new ApplicationException("Payment on account must have the same branch.");
                    }

                    if (Model.Header.ATC == "" && Model.Header.DocTotal == 0) {
                        throw new ApplicationException("ATC in header is required.");
                    }
                }

                if (Model.Header.AcctType == "CA") {
                    if (Model.Header.CheckPrint == "") throw new ApplicationException("Please select check print mode.");
                    if (Model.Header.CheckPrint.ToUpper() == "MANUAL CHECK" && Model.Header.CheckRemarks == "") throw new ApplicationException("Please indicated reason for Manual Check.");
                }

                repo.PostPayment(Model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message.ToString());
            }

        }


    }
}
