using System;
using System.Collections.Generic;
using System.Linq;
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
          

    }
}
