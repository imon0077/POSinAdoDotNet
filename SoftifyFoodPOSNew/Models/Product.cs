using System;
using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class Product
    {
        [Key]
        public Int64 ProductId { get; set; }
        public string ProductCode { get; set; } 
        
        [Required(ErrorMessage ="Please, provide product name")]        
        public string ProductName { get; set; }         
        public int ProductUnitId { get; set; }  
        public int CountryId { get; set; }
        public int WeightId { get; set; }
        public string Model { get; set; }
        
        public string ProductNameB { get; set; }  
        public float StockQty { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductSCategoryId { get; set; }
        public int BrandId { get; set; }
        public double ROL { get; set; }
        public double ROQ { get; set; }
        public float CostPrice { get; set; }
        public float SellingPrice { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }

        public string ProductImage { get; set; }
        public bool Isvalidity { get; set; }
        public double Warranty { get; set; }
        public bool IsSerial { get; set; }


    } // END : Model Class
}