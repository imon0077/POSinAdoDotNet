using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftifyFoodPOSNew.Models
{
    public class Transaction
    {
        public int GrrId { get; set; }
        public string TransType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime unPostDate { get; set; }
        public string TransStatus { get; set; }
        public string LedgerStatus { get; set; }


        public List<TransactionSub> TransactionSubList { get; set; }

        public class TransactionSub
        {
           
            public int TransId { get; set; }


        }



    }
}