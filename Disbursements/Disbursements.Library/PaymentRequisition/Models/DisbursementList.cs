using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Library.Models;
using SAPbobsCOM;

namespace Disbursements.Library.PaymentRequisition.Models
{
    internal class DisbursementList : Payment
    {
        public int InvDoc { get; set; }
        public string ObjType { get; set; }
        public decimal Outs { get;set;}
        public int WhsCode { get; set;} 
        public int TransId { get; set; }   
        public int OverDueDate { get; set; }
        public decimal WTEAmt { get; set; }
        public string ATCCode { get; set; }
        public int EWTTrans { get; set; }
        public string Segment_1 { get; set; }
        public decimal Advamt { get; set; }
        public int AdvWhs { get; set; }
        public string Remarks { get; set; }
        public string DType { get; set; }
        public int PRDoc { get; set; }
        public string PMode { get; set; }
        public int Bankcode { get; set; }
        public string BankName { get; set; }
        public string Bank { get; set; }
        public string WhsV { get; set; }
        public int Checkno { get; set; }
        public string Stat { get; set; }
        public int OPentry { get; set; }
        public string Voucher { get; set; }
        public string PayTo { get; set; }
        public string Payee { get; set; }
        public string CardAddr { get; set; }
        public string AcctNum { get; set; }
        public string PRCat { get; set; }
        public string PRMean { get; set; }
        public int PONum { get; set; }
        public string CWPayee { get; set; }
        public int PrevOP { get; set; }
        public string Corp { get; set; }
        public decimal AdvEWT { get; set; }



        public int BranchCode { get; set; }
        public int Branch { get; set; }

        public int Linenum { get; set; }
        public int GLAccount { get; set; }
        public string Acctname { get; set; }
        public decimal Amt { get; set; }
        public string Descr { get; set; }

        public int CardCode { get; set; }

        public int SAPDoc { get; set; }

        public string TXGRP { get; set; }
        public int ManualVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal NetVat { get; set; }
        public decimal ATCRate { get; set; }



    }


}
