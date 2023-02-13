using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLegacy.PF.Library.Auth
{
    public class EmployeeModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string WhsCode { get; set; }
        public string Dept { get; set; }
        public string Section { get; set; }
        public string UType { get; set; }
        public string FullName { get; set; }
        public string FName { get; set; }
        public string Position { get; set; }
        public int SecCode { get; set; }
        public int DeptCode { get; set; }
    }
}
