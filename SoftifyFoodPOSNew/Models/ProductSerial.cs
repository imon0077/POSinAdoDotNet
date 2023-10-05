using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftifyFoodPOSNew.Models
{
    public class ProductSerial
    {
        //       A.SerialId, SerialNo as Serial, WarrantyPeriod as warranty,C.ProductId,c.ProductName,C.ProductCode,D.BrandId,D.BrandName,E.prodCatId,E.prodCatName,
        //F.prodSCatId,F.prodSCatName,C.SellingPrice,C.CostPrice
        //G.InvoiceId,G.InvoiceNo,G.dtInvoice,I.ClientId,I.ClientName,G.TotalAmount,G.NetPayable,G.Paid,G.Due,H.Qty,H.SellingPrice
        public Int64 SerialId { get; set; }
        public string Serial { get; set; }
        public string warranty { get; set; }
        public Int64 ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Int64 BrandId { get; set; }
        public string BrandName { get; set; }
        public Int64 prodCatId { get; set; }
        public string prodCatName { get; set; }
        public Int64 prodSCatId { get; set; }
        public string prodSCatName { get; set; }
        public float SellingPrice { get; set; }
        public float CostPrice { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime dtInvoice { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public float TotalAmount { get; set; }
        public float NetPayable { get; set; }
        public float Paid { get; set; }
        public float Due { get; set; }
        public int Qty { get; set; }
        public float SoldPrice { get; set; }
        //J.GrrId,J.GrrNo,Convert(varchar, J.dtGrr,106) as dtGrr,L.SupplierId,L.SupplierName,J.ChallanNo,J.Receiver
        public int GrrId { get; set; }
        public string GrrNo { get; set; }
        public DateTime dtGrr { get; set; }
        public string SupplierName { get; set; }
        public string ChallanNo { get; set; }
        public string Receiver { get; set; }

        //M.ReturnNo,M.ReturnType,I.ClientId,I.ClientName,C.ProductId,C.ProductName,N.Qty,N.ReturnQty,N.Rate,N.Amount,M.Total
        public int ReturnNo { get; set; }
        public string ReturnType { get; set; }
        public string SalQty { get; set; }
        public string ReturnQty { get; set; }
        public string Amount { get; set; }
        public string Total { get; set; }

        public DateTime dtEntry { get; set; }
        public List<ProductSub> ProductSubList { get; set; }
        public List<UpdateSerial> GateProductSerial { get; set; }
        public class ProductSub
        {
            public Int64 ProductId { get; set; }
            public Int64 ProductSubId { get; set; }

            public int ProductSizeId { get; set; }

            public string Thickness { get; set; }
            public string ProductWeight { get; set; }

            public int ThicknessId { get; set; }
            public int RowId { get; set; }
            public int CurrentStock { get; set; }
        }
        public class UpdateSerial
        {
            public Int64 InvoiceId { get; set; }
            public Int64 SerialId { get; set; }
            public int BinId { get; set; }
            public bool IsSelect { get; set; }
        }

    }
}