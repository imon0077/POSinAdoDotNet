using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftifyFoodPOSNew.Models
{
    public class ReceiveReturn
    {
        //ComId, OrderSheetId, OutletName, dtOrderSheet, LuserId, aId, wId, dtEntry tbl_OrderSheet_Main
        public int RcvReturnId { get; set; }
    
        public DateTime dtRcvReturn { get; set; }  
        
        public List<OReceiveReturnSub> ReceiveReturnSubList { get; set; }
        public class OReceiveReturnSub
        {
            //OrderSheetId, ProductName, Code, [Order], Delivery, CatId, RowNo, tbl_OrderSheet_Sub
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string Code { get; set; }
            public float Qty{get;set;}
            public float TP{get;set; }
            public float MRP{get;set; }
            public int CatId { get; set; }
            public string prodCatName { get; set; }
            public string BarCode { get; set; }
            public string AliceCode { get; set; }

        }
    }
}