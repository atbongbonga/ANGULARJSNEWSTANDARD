using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using PR.api.Models.RequestList


namespace PR.api.Models
{
    public class RequestHeader : RequestList
    {
        public int ReqNo { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
        public string LabCoreReqNo { get; set; }
        public string Payee { get; set; }
        public int Code { get; set; }
        public string DepositoryBank { get; set; }
        public int AccountNo { get; set; }  
        public string AccountName { get; set; }
        public string Remarks { get; set; }
        public string MOP { get; set; }
        public string Category { get; set; }
        public string PaymentMeans { get; set; }
        public DateOnly DatePosted { get; set; }
        public DateOnly DateNeeded { get; set; }
        public string ReqStatus { get; set; }
        public string OPNo { get; set; }

    }



    



}
