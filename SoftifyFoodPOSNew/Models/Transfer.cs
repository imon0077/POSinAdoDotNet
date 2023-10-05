using System.Collections.Generic;

namespace SoftifyFoodPOSNew.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public string TransferNo { get; set; }
        
        public string dtTransfar { get; set; }
        public string ProductName { get; set; }

        public int WHFrom { get; set; }
        public int WHTo { get; set; }
        public int TypeId { get; set; }
        public string Remarks { get; set; }
        public string dtEntry { get; set; }
        public List<TransferSub>TransferSubList { get; set; }

        public class TransferSub
        {
            public int ProductId { get; set; }
            public float Qty { get; set; }
            public float UnitPrice { get; set; }
            public float Amount { get; set; }
            public int RowNo { get; set; }
        }


    }// END : Model Class
}