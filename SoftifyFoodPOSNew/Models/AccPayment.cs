using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class AccPayment
    {
        [Key]
        public int PaymentId { get; set; }
        public string dtPayment { get; set; }
        public string PaymentNo { get; set; }

        [Required(ErrorMessage = "Please, provide supplier name")]
        [Display(Name = "Supplier Name")]
        public int SupplierId { get; set; }
        public int GrrId { get; set; }
        public string SupplierAddress { get; set; }


        [Required(ErrorMessage = "Please, provide amount")]
        [Display(Name = "Amount")]
        public int Amount { get; set; }
        public int Discount { get; set; }

        [Required(ErrorMessage = "Please, provide pay mode")]
        [Display(Name = "Pay Mode:")]
        public string PayMode { get; set; }
        public int BankId { get; set; }
        public string ChequeNo { get; set; }
        public string dtCheque { get; set; }
        public bool isCleared { get; set; }
        public bool isPosted { get; set; }
        public string Remarks { get; set; }

    }
}