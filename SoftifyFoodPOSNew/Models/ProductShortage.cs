using System.Collections.Generic;

namespace SoftifyFoodPOSNew.Models
{
    public class ProductShortage
    {
        public int ShortageId { get; set; }
        public string dtShortage { get; set; }
        public string Remarks { get; set; }
        public int WHId { get; set; }
        public int TypeId { get; set; }                                            

        public List<ShortageSub> ShortageSubList { get; set; }

        public class ShortageSub
        {           
            public int ProductId { get; set; }
            public float Qty { get; set; }
            public float Rate { get; set; }
            public float Amount { get; set; }
            public int RowNo { get; set; }
        }


    }// END : Model Class
}