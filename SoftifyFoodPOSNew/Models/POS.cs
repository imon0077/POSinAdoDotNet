using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class POS
    {
        [Key]
        public Int64 InvoiceId { get; set; }
        public string dtInvoice { get; set; }
        public int ClientId { get; set; }
        public int WHId { get; set; }
        public string ItemType { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public float TotalAmount { get; set; } 
        public float Discount { get; set; } 
        public float Tax { get; set; } 
        public float Shipping { get; set; } 
        public float NetPayable { get; set; } 
        public float Paid { get; set; } 
        public float Percentage { get; set; }
        public float Due { get; set; }
        public string Phone { get; set; }
   

        public List<InvoiceSub> InvoiceSubList { get; set; }
        public List<InvoiceSerial> InvoiceSerialList { get; set; }

        public class InvoiceSub
        {
            public Int64 InvoiceId { get; set; }
            public Int64 WHId { get; set; }

            public Int64 ProductId { get; set; }
            public string PrdModel { get; set; }
            public Int64 GrrId { get; set; }
            public double StockQty { get; set; } 
            public float Qty { get; set; }
            public float SellingPrice { get; set; }  
            public float PrdDiscount { get; set; }  
            public float Amount { get; set; }  
            public int IsWarranty { get; set; }  
            public float warranty { get; set; }
            public int RowNo { get; set; }
        }

        public class InvoiceSerial
        {
            public Int64 InvoiceId { get; set; }
            public int ProductId { get; set; }
            public Int64 SerialId { get; set; }
            
        }


    }// END : Model Class
}