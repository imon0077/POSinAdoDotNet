
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models.MiniInventory
{
  
        public class MiniProductMR
        {
            public int MrId { get; set; }
            public DateTime dtMR { get; set; }
            public DateTime dtLoading { get; set; }
            public int BinId { get; set; }
            public string Remarks { get; set; }
            public int SupplierId { get; set; }
            public float CarryingPrice { get; set; }
            public float CurrencyPrice { get; set; }
        public List<MiniProductMRSub> MRSubList { get; set; }
            public class MiniProductMRSub
            {
                public int MrId { get; set; }
                public int ProductId { get; set; }
                public int UnitId { get; set; }
                public int CostPrice { get; set; }
                public int Qty { get; set; }
                public float SellPrice { get; set; }
                public float BrandId { get; set; }
                public float PurPrice { get; set; }

        }

            public static DataSet prcGetDatacombo(int ComId, int ProductId)
            {
                DataSet dsList = new DataSet();
                SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
                try
                {
                    string sqlQuery = "Exec[prcGet_MR] '" + ComId + "',"+ ProductId;
                    clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                    return dsList;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    clsCon = null;
                }
            }

        }



    }


