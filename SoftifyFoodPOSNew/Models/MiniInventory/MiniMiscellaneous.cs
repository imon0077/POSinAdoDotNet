using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models.MiniInventory
{
    public class MiniMiscellaneous
    {
        //Color Area
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorPrice { get; set; }
        //End of color Area

        //Brand Area
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        //End of brand Area

        //Size Area
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        //End of Size Area

        //Unit Area
        public int unitId { get; set; }
        public string unitName { get; set; }
        //End of Unit Area

        //warehouse Area 
        public int WHId { get; set; }
        public string WHName { get; set; }
        public string WHCode { get; set; }
        public string WHNameShort { get; set; }
        //End of warehouse Area

        //warehouse Area ComId
        public int BinId { get; set; }
        public string BinNo { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
        public string RackNo { get; set; }
        public string CellNo { get; set; }
        public string BinDetails { get; set; }
        public int BinWHId { get; set; }
        public string WarehouseName { get; set; }
        //End of warehouse Area

        //Category Area 
        public int prodCatId { get; set; }
        public string prodCatName { get; set; }

        //End of Category Area

        //Sub Category Area 
        public int prodSCatId { get; set; }
        public string prodSCatName { get; set; }
        public int prodCatIdSub { get; set; }
        //End of Sub Category Area

 


        public static DataSet prcGetDataUnit()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
            try
            {
                string sqlQuery = "Exec SoftifyStockManagement.dbo.prcGetUnit " + HttpContext.Current.Session["ComId"] + ", 0 ";
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

        public static DataSet prcGetDatawareHouse()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
            try
            {
                string sqlQuery = "Exec SoftifyStockManagement.dbo.prcGetWarehouse " + HttpContext.Current.Session["ComId"] + ", 0 ";
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
        // Brand List
        public static DataSet prcGetDataBrand()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
            try
            {
                string sqlQuery = "Exec SoftifyStockManagement.dbo.prcGetBrand " + HttpContext.Current.Session["ComId"] + ", 0 ";
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
        public static DataSet prcGetDataBin()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
            try
            {
                string sqlQuery = "Exec SoftifyStockManagement.dbo.prcGetBin " + HttpContext.Current.Session["ComId"] + ", 0 ";
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