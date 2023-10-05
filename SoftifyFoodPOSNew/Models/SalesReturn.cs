using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class SalesReturn
    {
        [Key]
        [Display(Name = "Id ")]
        public long ReturnId { get; set; }
        public string dtReturn { get; set; }
        public string ReturnNo { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string Reason { get; set; }
        public string ReturnType { get; set; }
        public float Total { get; set; }
        public string Remarks { get; set; }

        public List<SalesReturnSub> GateSalesReturnSubList { get; set; }

        public class SalesReturnSub
        {
            public long ProductId { get; set; }
            public int SerialId { get; set; }
            public string PrdModel { get; set; }
            public float Qty { get; set; }
            public int OldQty { get; set; }
            public float ReturnQty { get; set; }
            public float Rate { get; set; }
            public float Amount { get; set; }
        }
    }
}