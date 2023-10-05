using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class PurchaseReturn
    {
        //ReturnId, ReturnNo, dtReturn, CustId, ReturnType, BrandName, ProductType, Remarks, CreatedBy, CreatedDate, LastUpdatedBy, UId, ComId, PCName
        [Key]
        [Display(Name = "ID ")]
        public long ReturnId { get; set; }
        public DateTime dtReturn { get; set; }
        public string ReturnNo { get; set; }
        public int GrrId { get; set; }
        public int SupplierId { get; set; }
        public string ReturnType { get; set; }
        public float Total { get; set; }
        public string Remarks { get; set; }
        public List<PurchaseReturnSub> GatePurchaseReturnSubList { get; set; }
        public List<PurchaseReturnSerial> GatePurchaseReturnSerial { get; set; }
        public List<PurchaseSerial> PurchaseSerialList { get; set; }


        public class PurchaseReturnSub
        {

            //ReturnId, PrdId, PrdSubId, ColorId, Qty, UnitId, Rate, Amount, RowNo, wId
            public long ProductId { get; set; }
            public float Qty { get; set; }
            public int DemoQty { get; set; }
            public int OldPrd { get; set; }
            public float ReturnQty { get; set; }
            public float Rate { get; set; }
            public float Amount { get; set; }


        }
        public class PurchaseReturnSerial
        {
            public Int64 GrrId { get; set; }
            public Int64 SerialId { get; set; }
            public int BinId { get; set; }
            public bool IsSelect { get; set; }


        }

        public class PurchaseSerial
        {
            public Int64 GrrId { get; set; }
            public Int64 SerialId { get; set; }
            public int BinId { get; set; }
            public bool IsSelect { get; set; }

        }

    }
}