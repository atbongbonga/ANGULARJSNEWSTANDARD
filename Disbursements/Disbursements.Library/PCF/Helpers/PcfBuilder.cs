using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Disbursements.Library.PCF.Helpers
{
    public static class PcfBuilder
    {
        public static string GetAcctCodeByFormatCode(string formatcode)
        {
            return $@"SELECT AcctCode FROM hpdi.dbo.OACT WHERE FormatCode = '{formatcode}'";
        }
        public static string ShortNameCheck() {
            return "21100";
        }
        public static string IsJEUpdated(int transid)
        {
            //SAPEntry ={ pcfop}
            return $@"SELECT ISNULL((SELECT TOP 1 1  FROM pcfmon WHERE   TransID={transid}),0)";
        }

        public static string IsJEUpdated()
        {
            return "JE was already updated";
        }

        public static string PCFSERVER()
        {
            return "PCF JE";

        }
        public static string spPcfJE()
        {
            return "spPcfJE";
        }
        public static string spPcfJE1051()
        {
            return "spPcfJELegacy";
        }
        public static string spModeJEUpdateTables()
        {
            return "POST_JE_UpdateTables";
        }




    }
}
