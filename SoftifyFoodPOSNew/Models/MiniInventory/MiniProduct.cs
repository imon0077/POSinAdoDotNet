using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models.MiniInventory
{
  
        public class MiniProduct
        {
            public int ProductId { get; set; }
            public int SupplierId { get; set; }
            public string ProductName { get; set; }
            public float OPQty { get; set; }
            public float SellPrice { get; set; }
            public int UnitId { get; set; }
            public float CostPrice { get; set; }
            public string Currency { get; set; }
            public string BrandId { get; set; }
            public string Weight { get; set; }
            public string ROL { get; set; }
            public string PartNo { get; set; }

        public static DataSet prcGetDatacombo(int ComId, int ProductId)
            {
                DataSet dsList = new DataSet();
                SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
                try
                {
                    string sqlQuery = "Exec[prcGet_Product] '" + ComId + "',"+ ProductId;
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


