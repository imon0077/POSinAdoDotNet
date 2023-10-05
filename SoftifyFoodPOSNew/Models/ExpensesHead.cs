using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class ExpensesHead
    {
        public int HeadId { get; set; }
        public int LedgerId { get; set; }
        public string HeadName { get; set; }
        public string HeadType{get;set;}
        public string AccNo { get; set; }
        public float Balance { get; set; }
    }
} 