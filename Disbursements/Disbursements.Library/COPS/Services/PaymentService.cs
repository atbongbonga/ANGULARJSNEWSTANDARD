using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disbursements.Library.COPS;
using Disbursements.Library.COPS.Repositories;
using Disbursements.Library.COPS.ViewModels;
using static System.Formats.Asn1.AsnWriter;

namespace Disbursements.Library.COPS.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository repo;
        private readonly string userCode;
        public PaymentService(string userCode = "")
        {
            this.userCode = userCode;
            repo = new PaymentRepository(this.userCode);
        }
        public void PostPayment(PaymentView payment)
        {
            if (payment is null || payment.Header is null) throw new ApplicationException("Data not found.");
            if (payment.Header.DocType.Equals("S") && (payment.Invoices is null || !payment.Invoices.Any())) throw new ApplicationException("Invoice(s) not found.");
            if (payment.Header.DocType.Equals("A") && (payment.Accounts is null || !payment.Accounts.Any())) throw new ApplicationException("Invoice(s) not found.");

            if (string.IsNullOrEmpty(payment.Header.DocType)) throw new ApplicationException("Document type(Supplier/Account) is required.");
            if (string.IsNullOrEmpty(payment.Header.WhsCode)) throw new ApplicationException("Payment branch is required.");
            if (payment.Header.DocDate.Year < DateTime.Now.Year - 1) throw new ApplicationException("Invalid document date.");
            if (payment.Header.DueDate is not null)
            {
                if ((Convert.ToDateTime(payment.Header.DueDate).Year) < DateTime.Now.Year - 1) throw new ApplicationException("Invalid due date.");
            }
            if (payment.Header.DocDueDate.Year < DateTime.Now.Year - 1) throw new ApplicationException("Invalid document due date.");
            if (string.IsNullOrEmpty(payment.Header.PMode)) throw new ApplicationException("Payment mode is required.");
            if (payment.Header.DocType.Equals("S") && string.IsNullOrEmpty(payment.Header.CardCode)) throw new ApplicationException("Supplier code is required.");
            //if (payment.Header.DocType.Equals("A") && string.IsNullOrEmpty(payment.Header.U_CardCode)) throw new ApplicationException("Supplier code is required.");
            if (string.IsNullOrEmpty(payment.Header.BankCode)) throw new ApplicationException("Bank code is required.");
            if (string.IsNullOrEmpty(payment.Header.Comments)) throw new ApplicationException("Remarks is required.");
            if (payment.Header.DocTotal <= 0) throw new ApplicationException("Invalid document total.");
            if (payment.Header.DocType.Equals("A") && payment.Accounts.Any(x => x.AcctCode.Substring(0, 5).Equals("25200"))) throw new ApplicationException("EWT payment is not allowed.");

            if (payment.Header.DocType.Equals("A"))
            {
                foreach (var item in payment.Accounts)
                {
                    if (item.EWT is not decimal.Zero && string.IsNullOrEmpty(item.ATC)) throw new ApplicationException($"Invalid ATC at line: {item.LineId}.");
                    if (item.EWT is not decimal.Zero && string.IsNullOrEmpty(item.TaxGroup)) throw new ApplicationException($"Invalid Tax Group at line: {item.LineId}.");
                }
            }

            repo.PostPayment(payment);
        }

        public void UpdatePayment(PaymentHeaderView payment)
        {
            repo.UpdatePayment(payment);   
        }
        
        public void CancelPayment(int docNum) => repo.CancelPayment(docNum);
    }
}
