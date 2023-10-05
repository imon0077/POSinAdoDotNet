using Softify;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace SoftifyFoodPOSNew.Models
{
    public class ReceiptPaymentSingle
    {
        //RecvPayId, dtRecv, RecvNo, RecvPayType, Remarks, ComId, LUserId, PcName
        public int RecvPayId { get; set; }
        [Display(Name = "DocNo ")]
        public string RecvNo { get; set; }
        public int ClientId { get; set; }
        public int SupplierId { get; set; }
        public DateTime dtRecv { get; set; }
        public string RecvPayType { get; set; }
        public string Remarks { get; set; }
        public string PayMode { get; set; }

        public float DueAmount { get; set; }

        public float Amount { get; set; }
        public int HeadId { get; set; }
        public string ChequeNo { get; set; }

    }

}