using Softify;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace SoftifyFoodPOSNew.Models
{
    public class ReceiptPayment
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
        public List<ReceiptPaymentSub> ReceiptPaymentSubList { get; set; }

        public class ReceiptPaymentSub
        {
            //RecvPayId, TranType, ClientId, Amount, PayMode, BankId, ChequeNo, Remarks
            public int RecvPayId { get; set; }

            public string PayMode { get; set; }

            public int ClientId { get; set; }

            public int SupplierId { get; set; }

            public float DueAmount { get; set; }

            public float Amount { get; set; }
            public float Discount { get; set; }
            public int HeadId { get; set; }
            public string ChequeNo { get; set; }
            public string Remarks { get; set; }
            public string ChequeDate { get; set; }

        }

        public static DataSet prcGetData(int id, int comId)
        {
            DataSet ds = new DataSet();

            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyWebSecurity", true);

            try
            {
                string sqlQuery = "Exec prcGet_Production_ProfileRecv '" + comId + "', '" + id + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref ds, sqlQuery);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}