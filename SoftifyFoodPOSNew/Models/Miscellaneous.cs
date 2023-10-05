using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Miscellaneous
    {
        //LC Area
        public int LCId { get; set; }
        public string LCNo { get; set; }

        //Weight Area
        public int WeightId { get; set; }
        public string Weight { get; set; }
        //End of brand Area

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
       
        //Category Area 
        public int prodCatId { get; set; }
        public string prodCatName { get; set; }

        //End of Category Area

        //Sub Category Area 
        public int prodSCatId { get; set; }
        public string prodSCatName { get; set; }
        public int prodCatIdSub { get; set; }
        //End of Sub Category Area

        public static DataSet prcGetDataColor()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetColor " + HttpContext.Current.Session["ComId"] + ", 0 ";
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

        // LC List
        public static DataSet prcGetDataLC()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = $"Exec prcGetLC {HttpContext.Current.Session["ComId"]}, 0 ";
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

        // Weight List
        public static DataSet GetWeightList()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = $"Exec prcGetWeight {HttpContext.Current.Session["ComId"]}, 0 ";
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
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetBrand " + HttpContext.Current.Session["ComId"] + ", 0 ";
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

        public static DataSet prcGetDataSize()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetSize " + HttpContext.Current.Session["ComId"] + ", 0 ";
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


        public static DataSet prcGetDataUnit()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetUnit " + HttpContext.Current.Session["ComId"] + ", 0 ";
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
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetWarehouse " + HttpContext.Current.Session["ComId"] + ", 0 ";
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
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetBin " + HttpContext.Current.Session["ComId"] + ", 0 ";
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

        public static DataSet prcGetDataCategory()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetCategory " + HttpContext.Current.Session["ComId"] + ", 0 ";
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

        public static DataSet prcGetDataSubCategory()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetSubCategory " + HttpContext.Current.Session["ComId"] + ", 0 ";
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