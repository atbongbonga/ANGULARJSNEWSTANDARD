using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy
{
    public class SERVER
    {
        private readonly string appName;
        private readonly string SAP_HPCOMMON_ = string.Empty;
        private readonly string SAP_HPCOMMON_TEST_ = string.Empty;
        private readonly string SAP_DISBURSEMENTS_ = string.Empty;
        private readonly string SAP_PF_ = string.Empty;
        private readonly string SAP_PF2_ = string.Empty;
        private readonly string EMS_HPCOMMON_ = string.Empty;
        private readonly string SAP_BOOKKEEPING_ = string.Empty;
        private readonly string SAP_BUSINESSONE_ = string.Empty;
        public SERVER(string ApplicationName)
        {
            appName = ApplicationName;
            SAP_HPCOMMON_ = $"Data Source=172.30.0.17;Initial Catalog=HPCOMMON;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
            SAP_HPCOMMON_TEST_ = $"Data Source=172.30.1.167;Initial Catalog=HPCOMMON;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={appName}";
            SAP_DISBURSEMENTS_ = $"Data Source=172.30.0.17;Initial Catalog=DISBURSEMENTS;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
            EMS_HPCOMMON_ = $"Data Source=192.171.10.51;Initial Catalog=HPCOMMON;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
            SAP_BOOKKEEPING_ = $"Data Source=172.30.0.17;Initial Catalog=BOOKKEEPING;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={appName}";
            SAP_PF_ = $"Data Source=172.30.0.17;Initial Catalog=PF;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
            SAP_PF2_ = $"Data Source=172.30.0.17;Initial Catalog=PF2;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
        }

        public string SAP_HPCOMMON => SAP_HPCOMMON_;
        public string SAP_HPCOMMON_TEST => SAP_HPCOMMON_TEST_;
        public string SAP_PF => SAP_PF_;
        public string SAP_PF2 => SAP_PF2_;
        public string EMS_HPCOMMON => EMS_HPCOMMON_;
        public string SAP_DISBURSEMENTS => SAP_DISBURSEMENTS_;
        public string SAP_BOOKKEEPING => SAP_BOOKKEEPING_;
        public string SAP_BUSINESSONE => SAP_BUSINESSONE_;

    }
}
