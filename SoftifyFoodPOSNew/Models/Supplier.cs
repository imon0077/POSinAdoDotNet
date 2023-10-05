using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Supplier
    {
        public int supplierId { get; set; }
        public string SupplierCode { get; set; }

        [Required(ErrorMessage = "Please Provide Supplier Name")]
        public string supplierName { get; set; }

        [Required(ErrorMessage = "Please Provide Address")]
        public string SupplierAddress { get; set; }

        public string SupplierPhone { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierEmail { get; set; }
        public string ContactName { get; set; }
        public float OpeningBalance { get; set; }

        public string ContactPhone { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int AccId { get; set; }
        public int ClientId { get; set; }
        public bool isClient { get; set; }

    }
}