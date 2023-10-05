using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class Damages
    {
        [Key]
       

        public int DamageId { get; set; }
        public string DamageNo{ get; set; }
        [Required(ErrorMessage ="Please, provide damage date.")]
        public string dtDamage { get; set; }
        [Required(ErrorMessage = "Please, provide warehouse name")]
        public int WHId { get; set; }
        public int TypeId { get; set; }        
        public int GrrId { get; set; }
        public float Total { get; set; }
        public int SupplierId { get; set; }
        public string Remarks { get; set; }
        public List<DamagesSub> DamagesSubList { get; set; }

        public class DamagesSub
        {           
            public int ProductId { get; set; }
            public float Qty { get; set; }
            public float Rate { get; set; }
            public float Amount { get; set; }
        }


    }// END : Model Class
}