using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class SalesCollection
    {
        [Key]
        public int CollectionId { get; set; }
        public string dtCollection { get; set; }
        public string CollectionNo { get; set; }

        [Required(ErrorMessage = "Please, provide client")]
        [Display(Name = "Client Name:")]
        public int ClientId { get; set; }
        public string ClientAddress { get; set; }

        public int? InvoiceId { get; set; }


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