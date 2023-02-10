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
        private readonly string SAP_DISBURSEMENTS_ = string.Empty;
        private readonly string EMS_HPCOMMON_ = string.Empty;
        public SERVER(string ApplicationName)
        {
            appName = ApplicationName;
            SAP_HPCOMMON_ = $"Data Source=172.30.0.17;Initial Catalog=HPCOMMON;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
            SAP_DISBURSEMENTS_ = $"Data Source=172.30.0.17;Initial Catalog=DISBURSEMENTS;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
            EMS_HPCOMMON_ = $"Data Source=192.171.10.51;Initial Catalog=HPCOMMON;Integrated Security=False;UID=sapdb;PWD=sapdb; Application Name={ appName }";
        }

        public string SAP_HPCOMMON => SAP_HPCOMMON_;
        public string EMS_HPCOMMON => EMS_HPCOMMON_;
        public string SAP_DISBURSEMENTS => SAP_DISBURSEMENTS_;
    }
}
