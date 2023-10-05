
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models.MiniInventory
{

    public class MiniProductTransfer
    {
        public int TransId { get; set; }
        public DateTime dtTrans { get; set; }
        public int FromBinId { get; set; }
        public int ToBinId { get; set; }
        public string Remarks { get; set; }
        public List<MiniProductTransferSub> TransferSubList { get; set; }
        public class MiniProductTransferSub
        {
            public int TransId { get; set; }
            public int SupplierId { get; set; }
            public int ProductId { get; set; }
            public int UnitId { get; set; }
            public int Qty { get; set; }
            public float Price { get; set; }

        }

 
}
}


