using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Category
    {
        public int PcatId { get; set; }

        [Required(ErrorMessage = "Please Provide Category Name")]
        public string PcatName { get; set; }
        public int CatId { get; set; } // this cat Id use for sub category CatId

        public string CatIds { get; set; } // this cat Id use for sub category CatId

        [Required(ErrorMessage = "Please Provide Category Name")]
        public string CatName { get; set; } // this cat Name use for sub category Cat Name
        public int SubCatId { get; set; }
        public string SubCatName { get; set; }
        public string ProdCatName { get; set; }


        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorPrice { get; set; }
        public string ColorCode { get; set; }

        public int SizeId { get; set; }
        public string SizeName { get; set; }

        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool IsSalesBrand { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }

        public class WareHouseBin
        {
            public int WHID { get; set; }
            public string WHCode { get; set; }
            public string WHName { get; set; }
            public string WHNameShort { get; set; }
            public string ParentId { get; set; }
            public string ParentCode { get; set; }
            public string WHType { get; set; }

            public int BinId { get; set; }
            public string BinNo { get; set; }
            public string FloorNo { get; set; }
            public string RoomNo { get; set; }
            public string RackNo { get; set; }
            public string CellNo { get; set; }
            public string BinDetails { get; set; }
        }

        
        private void prcSetData(IDataReader reader)
        {
            PcatId = Convert.ToInt32(reader["prodCatId"].ToString());
            PcatName = reader["prodCatName"].ToString();
        }

        public static List<Category> prcGetData(int ComId)
        {
            IDataReader reader = null;
            List<Category> list = new List<Category>();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC prcGet_Category '" + ComId + "', 0 ";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    Category Item = new Category();
                    Item.prcSetData(reader);
                    list.Add(Item);
                }
                return list;
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


        public static DataSet prcGetDataBrandUnit(int comId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC prcGet_BrandUnit " + comId + ", " + 0 + "";

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
        public static List<Category> prcSetDataBrand(DataSet dsList)
        {
            List<Category> BrandList = new List<Category>();
            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
            {
                Category BrandInfo = new Category();

                BrandInfo.BrandId = Convert.ToInt32(dsList.Tables[0].Rows[i]["BrandId"].ToString());
                BrandInfo.BrandName = dsList.Tables[0].Rows[i]["BrandName"].ToString();
                BrandList.Add(BrandInfo);

            }
            return BrandList;
        }

        public static List<Category> prcSetDataUnit(DataSet dsList)
        {
            List<Category> UnitList = new List<Category>();

            for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
            {
                Category UnitInfo = new Category();

                UnitInfo.UnitId = Convert.ToInt32(dsList.Tables[1].Rows[i]["UnitId"].ToString());
                UnitInfo.UnitName = dsList.Tables[1].Rows[i]["UnitName"].ToString();

                UnitList.Add(UnitInfo);
            }
            return UnitList;
        }
        public static DataSet prcGetDataCategory(int comId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC prcGet_Category " + comId + ", " + 0 +"";

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
        //end DataSet prcGetData\


        public static DataSet prcGetDataColorSize(int comId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC prcGet_ColorSize " + comId + "";

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
        //end DataSet prcGetData\

        public static DataSet prcGetDatacombo(string Flag, int id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetCombo '" + Flag + "', " + id + " ";

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

        //public static void prcSetDataSub(string flag, DataSet dsList)
        //{
        //      ColorList=new List<Category>();
        //      SizeList=new List<Category>();
          
        //    if (flag == "Edit")
        //    {
        //        for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
        //        {
        //            Category SubDocInfo = new Category();

        //           SubDocInfo.SubCatId = Convert.ToInt32(dsList.Tables[1].Rows[i]["prodSCatId"].ToString());
        //           SubDocInfo.CatId = Convert.ToInt32(dsList.Tables[1].Rows[i]["prodCatId"].ToString());
        //           SubDocInfo.ProdCatName = dsList.Tables[1].Rows[i]["ProdCatName"].ToString();
        //           SubDocInfo.SubCatName = dsList.Tables[1].Rows[i]["prodSCatName"].ToString();

        //            ColorList.Add(SubDocInfo);
        //        }
        //    }

        //    if (flag == "ColorSize")
        //    {
        //        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
        //        {
        //            Category ColorInfo = new Category();

        //            ColorInfo.ColorId = Convert.ToInt32(dsList.Tables[0].Rows[i]["ColorId"].ToString());
        //            ColorInfo.ColorName = dsList.Tables[0].Rows[i]["ColorName"].ToString();

        //            ColorList.Add(ColorInfo);
                  
        //        }


        //        for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
        //        {
        //            Category SizeInfo = new Category();

        //            SizeInfo.SizeId = Convert.ToInt32(dsList.Tables[1].Rows[i]["sizeId"].ToString());
        //            SizeInfo.SizeName = dsList.Tables[1].Rows[i]["sizeName"].ToString();

        //            SizeList.Add(SizeInfo);
        //        }
        //    }
          
        //}


        public static List<Category> prcSetDataColor(DataSet dsList)
        {
            List<Category>ColorList = new List<Category>();
            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
            {
                Category ColorInfo = new Category();

                ColorInfo.ColorId = Convert.ToInt32(dsList.Tables[0].Rows[i]["ColorId"].ToString());
                ColorInfo.ColorName = dsList.Tables[0].Rows[i]["ColorName"].ToString();
                ColorList.Add(ColorInfo);

            }
            return ColorList;
        }

        public static List<Category> prcSetDataSize(DataSet dsList)
        {
            List<Category> SizeList = new List<Category>();

            for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
            {
                Category SizeInfo = new Category();

                SizeInfo.SizeId = Convert.ToInt32(dsList.Tables[1].Rows[i]["sizeId"].ToString());
                SizeInfo.SizeName = dsList.Tables[1].Rows[i]["sizeName"].ToString();

                SizeList.Add(SizeInfo);
            }
            return SizeList;
        }


        public static List<Category> prcSetDataSubCategory(DataSet dsList)
        {
            List<Category> SubCategoryList = new List<Category>();

            for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
            {
                Category SubDocInfo = new Category();

                SubDocInfo.SubCatId = Convert.ToInt32(dsList.Tables[1].Rows[i]["prodSCatId"].ToString());
                SubDocInfo.CatId = Convert.ToInt32(dsList.Tables[1].Rows[i]["prodCatId"].ToString());
                SubDocInfo.CatIds = dsList.Tables[1].Rows[i]["prodCatId"].ToString();
                SubDocInfo.ProdCatName = dsList.Tables[1].Rows[i]["ProdCatName"].ToString();
                SubDocInfo.SubCatName = dsList.Tables[1].Rows[i]["prodSCatName"].ToString();

                SubCategoryList.Add(SubDocInfo);
            }
            return SubCategoryList;
        }


    }
}