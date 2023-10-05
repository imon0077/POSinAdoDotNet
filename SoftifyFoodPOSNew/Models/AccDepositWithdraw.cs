using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class AccDepositWithdraw
    {
        [Key]
        public int TrnId { get; set; }
        public string dtTrn { get; set; }
        public string TrnNo { get; set; }
        
        
        [Required(ErrorMessage = "Please, provide amount")]
        [Display(Name = "Amount")]
        public int Amount { get; set; }
        
        public string PayMode { get; set; }

        public string TrnType { get; set; }
        public int FromHeadId { get; set; }
        public int ToHeadId { get; set; }
        
        public int BankId { get; set; }
        public string ChequeNo { get; set; }
        public string dtCheque { get; set; }
        public bool isCleared { get; set; }
        public bool isPosted { get; set; }
        public string Remarks { get; set; }

    }
}