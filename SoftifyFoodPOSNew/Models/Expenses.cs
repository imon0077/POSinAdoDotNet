using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Expenses
    {
        [Key]
        [Display(Name = "ID ")]
        public long ExpId { get; set; }

        [Required(ErrorMessage ="Please, provide expense date")]
        public string dtExp { get; set; }

        public string dtFrom { get; set; }
        public string dtTo { get; set; }

        public string ExpNo { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public string ExpType { get; set; }
        public long HeadId { get; set; }
        public string Description { get; set; }
        public string AccNo { get; set; }
        public float Total { get; set; }
        public List<ExpensesSub> GateExpensesSubList { get; set; }

        public class ExpensesSub
        {
            public int HeadId { get; set; }
            public int LedgerId { get; set; }
            public float Amount { get; set; }
            public string Remarks { get; set; }
            public int RowNo { get; set; }

        }

    }
}