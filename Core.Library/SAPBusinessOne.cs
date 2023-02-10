using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace AccountingLegacy.Core.Library
{
    public class SAPBusinessOne :IDisposable
    {
        private Company company;

        public Company Company => company;
        public void BeginTran() => company.StartTransaction();
        public void Commit() => company.EndTransaction(BoWfTransOpt.wf_Commit);
        public void Rollback() => company.EndTransaction(BoWfTransOpt.wf_RollBack);

        public JournalEntries JournalEntries => company.GetBusinessObject(BoObjectTypes.oJournalEntries);
        public Payments VendorPayments => company.GetBusinessObject(BoObjectTypes.oVendorPayments);

        public SAPBusinessOne()
        {
            this.company = new Company();
            if (!this.company.Connected)
            {
                this.company.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                this.company.Server = "172.30.1.167";
                this.company.CompanyDB = "HPDI";
                this.company.DbUserName = "sapdb";
                this.company.DbPassword = "sapdb";
                this.company.UserName = "manager";
                this.company.Password = "12345";
                //_company.UseTrusted = true;
                this.company.language = BoSuppLangs.ln_English;
                var result = this.company.Connect();
                if (result != 0) throw new ApplicationException("SAP login failed.");
            }
        }

        public void Dispose()
        {
            if (company.Connected) company.Disconnect();
        }
    }
}
