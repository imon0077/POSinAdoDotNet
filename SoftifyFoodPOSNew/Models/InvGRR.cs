using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftifyFoodPOSNew.Models
{
    public class InvGRR
    {
        public Int64 GrrId { get; set; }
        public string dtGrr { get; set; }
        public string GrrNo { get; set; }

        [Required(ErrorMessage = "Please, provide supplier name")]
        [Range(1, 99999, ErrorMessage = "Please, select supplier")]
        public int SupplierId { get; set; }

        public int LCId { get; set; }
        public int WHId { get; set; }

        [Required(ErrorMessage ="Please, provide challan number")]
        public string ChallanNo { get; set; }
        public string ReceivedBy { get; set; }
        public string Notes { get; set; }
        public string PayMode { get; set; }
        public int HeadId { get; set; }
        public string dtCheque { get; set; }
        public string ChequeNo { get; set; }
        public float TotalAmount { get; set; }
        public float DueAmount { get; set; }
        public float PaidAmount { get; set; }
        public string ProductName { get; set; }
        public List<InvGRRSub> InvGRRSubList { get; set; }
        public List<InvGrrSerial> GrrSerialList { get; set; }

        public class InvGRRSub
        {
            public Int64 GrrId { get; set; }
            public int ProductId { get; set; }
            public int WeightId { get; set; }
            public string ProductName { get; set; }
            public string ModelNo { get; set; }
            public string Reference { get; set; }
            public float QtyRcv { get; set; }
            public int RowNo { get; set; }
            public bool IsSerial { get; set; }
            public bool IsWarranty { get; set; }
            public string Warranty { get; set; }
            public float Price { get; set; }
            public float SellingPrice { get; set; }

            
            public float Amount { get; set; }
            public string dtValidate { get; set; }
            public string Photo { get; set; }
            public string FileName { get; set; }
            public string OldFileName { get; set; }
        }

        public class InvGrrSerial
        {
            public Int64 ProductId { get; set; }
            public string SerialNo { get; set; }
            public int IsSell { get; set; }
        }

    }
}