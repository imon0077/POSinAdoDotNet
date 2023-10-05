using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;


namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class CategoryController : Controller
    {
        public int ComId = clsCommon.ComID;
        DataSet dsList = new DataSet();


        List<clsCommon.clsCombo2> CategoryList = new List<clsCommon.clsCombo2>();
        //List<clsCommon.clsCombo2> ColorList = new List<clsCommon.clsCombo2>();
        //List<clsCommon.clsCombo2> SizeList = new List<clsCommon.clsCombo2>();
        List<clsCommon.clsCombo2> WHList = new List<clsCommon.clsCombo2>();

        public static List<Category> CategoryLoad = new List<Category>();
        public static List<Category> SubCategoryLoad = new List<Category>();
        public void prcLoadCategoryCombo()
        {
            dsList = Category.prcGetDatacombo("Category", ComId);
            CategoryList = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            ViewBag.CategoryComboList = CategoryList;

        }
        public void prcLoadWareHouseCombo()
        {
            dsList = Category.prcGetDatacombo("WareHouse", ComId);
            WHList = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            ViewBag.WHList = WHList;

        }

        public ActionResult BrandUnit()
        {
            return View();
        }

        public JsonResult GetCategoryData(int tempId)
        {
            CategoryLoad = Category.prcGetData(ComId);
            return Json(CategoryLoad.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubCategoryData(int tempId)
        {
            dsList = Category.prcGetDataCategory(ComId);
            SubCategoryLoad = Category.prcSetDataSubCategory(dsList);
            return Json(SubCategoryLoad.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryCombo(int nullId)
        {
            prcLoadCategoryCombo();
            return Json(CategoryList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBrandData()
        {
            dsList = Category.prcGetDataBrandUnit(ComId);
            var BrandList = Category.prcSetDataBrand(dsList);
            return Json(BrandList.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUnitData()
        {
            dsList = Category.prcGetDataBrandUnit(ComId);
            var UnitList = Category.prcSetDataUnit(dsList);
            return Json(UnitList.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetColorData()
        {
            dsList = Category.prcGetDataColorSize(ComId);
            var ColorList = Category.prcSetDataColor(dsList);
            return Json(ColorList.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSizeData()
        {
            dsList = Category.prcGetDataColorSize(ComId);
            var SizeList = Category.prcSetDataSize(dsList);
            return Json(SizeList.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWareHouseCombo()
        {
            prcLoadWareHouseCombo();
            return Json(WHList, JsonRequestBehavior.AllowGet);
        }

        public string GetWareHouseList()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsCombo = new DataSet();
            string sqlQuery = "EXEC [prcGet_WareHouseBin] " + ComId;
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsCombo, sqlQuery);
            DataTable dtData = (DataTable)dsCombo.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public string GetWareHouseBinList()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsCombo = new DataSet();
            string sqlQuery = "EXEC [prcGet_WareHouseBin] "+ComId;
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsCombo, sqlQuery);
            DataTable dtData = (DataTable)dsCombo.Tables[1];
            return clsCommon.JsonSerialize(dtData);
        }
        public string prcDataSaveBrand(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(BrandId),0) + 1 AS float)  AS BrandId  FROM tblCat_Brand";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblCat_Brand ( BrandId, BrandName,PCName,LUserId,aId, ComID) "
                               + " Values (" + NewId + ", '" + model.BrandName + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ",  " + ComId + ")";

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateBrand(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Brand set BrandName='" + model.BrandName + "' Where BrandId= '" + model.BrandId + "' ";

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData

        public string prcDataSaveUnit(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(unitId),0) + 1 AS float)  AS unitId  FROM tblCat_unit";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblCat_unit ( unitId, unitName,UnitNameShort,PCName,LUserId,aId, ComID) "
                               + " Values (" + NewId + ", '" + model.UnitName + "','" + model.UnitName + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ",  " + ComId + ")";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateUnit(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_unit set unitName='" + model.UnitName + "',UnitNameShort='" + model.UnitName + "'  Where unitId= '" + model.UnitId + "' ";

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData
        [HttpPost]
        public ActionResult BrandUnit(Category model)
        {
            try
            {
                string msg = "";
                if (model.BrandName == null)
                {
                    if (model.UnitId == 0)
                    {
                        msg = prcDataSaveUnit(model);
                    }
                    else if (model.UnitId != 0)
                    {
                        msg = prcDataUpdateUnit(model);
                    }
                }
                else
                {
                    if (model.BrandId == 0)
                    {
                        msg = prcDataSaveBrand(model);
                    }
                    else if (model.BrandId != 0)
                    {
                        msg = prcDataUpdateBrand(model);
                    }
                }

                return Json(msg, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ColorSize()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ColorSize(Category model)
        {
            try
            {
                string msg = "";
                if (model.ColorName == null)
                {
                    if (model.SizeId == 0)
                    {
                        msg = prcDataSaveSize(model);
                    }
                    else if (model.SizeId != 0)
                    {
                        msg = prcDataUpdateSize(model);
                    }
                }
                else
                {
                    if (model.ColorId == 0)
                    {
                        msg = prcDataSaveColor(model);
                    }
                    else if (model.ColorId != 0)
                    {
                        msg = prcDataUpdateColor(model);
                    }
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Category model)
        {
            try
            {
                string msg = "";
                if (model.PcatName == null)
                {

                    if (model.SubCatId == 0)
                    {
                        msg = prcDataSaveSubCat(model);
                    }
                    else if (model.SubCatId != 0)
                    {
                        msg = prcDataUpdateSubCat(model);
                    }
                }
                else
                {
                    if (model.PcatId == 0)
                    {
                        prcDataSaveCat(model);
                        msg = "1";
                    }
                    else if (model.PcatId != 0)
                    {
                        prcDataUpdateCat(model);
                        msg = "2";
                    }
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult WareHouseBin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult WareHouseBin(Category.WareHouseBin model)
        {
            try
            {
                string msg = "";
                if (model.WHName == null)
                {

                    if (model.BinId == 0)
                    {
                        msg = prcDataSaveWareHouseBin(model);
                    }
                    else if (model.BinId != 0)
                    {
                        msg = prcDataUpdateWareHouseBin(model);
                    }
                }
                else
                {
                    if (model.WHID == 0)
                    {
                        prcDataSaveWareHouse(model);
                        msg = "1";
                    }
                    else if (model.WHID != 0)
                    {
                        prcDataUpdateWareHouse(model);
                        msg = "2";
                    }
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveCat(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(prodCatId),0) + 1 AS float)  AS prodCatId  FROM  tblCat_Prod_Category";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into  tblCat_Prod_Category ( prodCatId, prodCatName,PCName,LUserId,aId,ComID) "
                               + " Values (" + NewId + ", '" + model.PcatName + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ",'" + ComId + "')";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "Successfully Save.";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateCat(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Prod_Category set prodCatName='" + model.PcatName + "', aId=" + model.PcatId + " Where prodCatId = " + model.PcatId;

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData



        public string prcDataSaveWareHouse(Category.WareHouseBin model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(WHID),0) + 1 AS float)  AS WHID  FROM  tblCat_Warehouse";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into  tblCat_Warehouse (ComId, WHID, WHCode, WHName, WHNameShort, ParentId, ParentCode, WHType,PCName,LUserId, aId) "
                           + " Values ('" + ComId + "'," + NewId + ", '" + model.WHCode + "','" + model.WHName + "','" + model.WHName + "',1,1,'','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ")";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateWareHouse(Category.WareHouseBin model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Warehouse set WHName='" + model.WHName + "', WHNameShort='"+model.WHName + "',ParentId='1', ParentCode='1', WHType='',aId=" + model.WHID + " Where WHId = " + model.WHID+"";

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData



        public string prcDataSaveWareHouseBin(Category.WareHouseBin model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(BinId),0) + 1 AS float)  AS BinId  FROM  tblCat_Warehouse_Bin";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into  tblCat_Warehouse_Bin (ComId, BinId, BinNo, FloorNo, RoomNo, RackNo, CellNo, BinDetails, WHId, aId, PCName, LUserId) "
                           + " Values ('" + ComId + "'," + NewId + ", '" + (model.FloorNo+"-"+model.RoomNo+"-"+model.RackNo + "-"+model.CellNo).ToString() + "','" + model.FloorNo + "','" + model.RoomNo + "','" + model.RackNo + "','" + model.CellNo + "','" + model.BinDetails + "','" + model.WHID + "','" + NewId + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ")";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateWareHouseBin(Category.WareHouseBin model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Warehouse_Bin set BinNo='" + (model.FloorNo + "-" + model.RoomNo + "-" + model.RackNo + "-" + model.CellNo).ToString() + "', FloorNo='" + model.FloorNo + "', RoomNo='" + model.RoomNo + "', RackNo='" + model.RackNo + "', CellNo='" + model.CellNo + "', BinDetails='" + model.BinDetails + "', WHId='" + model.WHID + "', aId='" + model.BinId + "' Where BinId = " + model.BinId + "";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData
        public string prcDataSaveColor(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(ColorId),0) + 1 AS float)  AS subCatId  FROM  tblCat_Color";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblCat_Color ( colorId, colorName,ColorCode,PCName, LUserId,aId) "
                               + " Values (" + NewId + ", '" + model.ColorName + "', '" + model.ColorCode + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ")";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateColor(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Color set colorName='" + model.ColorName + "' Where colorId= '" + model.ColorId + "' ";

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData

        public string prcDataSaveSize(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(sizeId),0) + 1 AS float)  AS subCatId  FROM tblCat_Size";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblCat_Size ( sizeId, sizeName,PCName,LUserId,aId) "
                               + " Values (" + NewId + ", '" + model.SizeName + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ")";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateSize(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Size set sizeName='" + model.SizeName + "' Where sizeId= '" + model.SizeId + "' ";

                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData




        public string prcDataSaveSubCat(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(prodSCatId),0) + 1 AS float)  AS subCatId  FROM tblCat_Prod_SubCategory";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblCat_Prod_SubCategory ( prodSCatId, prodCatId, prodSCatName,PurchaseType,PCName,LUserId,aId, ComID) "
                               + " Values (" + NewId + ", '" + model.CatId + "', '" + model.SubCatName + "','','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ", " + NewId + ",  " + ComId + ")";
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Insert', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcDataSave


        //To Update 
        public string prcDataUpdateSubCat(Category model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Update tblCat_Prod_SubCategory set prodSCatName='" + model.SubCatName + "', PurchaseType='',prodCatId='" + model.CatId + "' Where prodSCatId= '" + model.SubCatId + "' ";

                arQuery.Add(sqlQuery);


                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Update', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);


                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "2";
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

            finally
            {
                clsCon = null;
            }
        } //end : prcUpdateData



        public string prcDeleteDataSize(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();

            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Size " +
                                         " WHERE SizeId = " + ItemId + "");
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "1";
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
        //end : prcDeleteData

        public string prcDeleteDataColor(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();

            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Color " +
                                         " WHERE ColorId = " + ItemId + "");
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";

                arQuery.Add(Trans2);


                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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
        //end : prcDeleteData


        public string prcDeleteDataCat(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();

            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Prod_Category " +
                                         " WHERE prodCatId = " + ItemId + "");
                arQuery.Add(sqlQuery);


                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";

                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
            finally
            {
                clsCon = null;
            }
        }
        //end : prcDeleteData

        public string prcDeleteDataSubCat(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();

            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Prod_SubCategory " +
                                         " WHERE prodSCatId = " + ItemId + "");

                arQuery.Add(sqlQuery);


                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";

                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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
        //end : prcDeleteData

        public string prcDeleteDataUnit(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();

            ArrayList arQuery = new ArrayList();
            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Unit " +
                                         " WHERE UnitId = " + ItemId + "");
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";

                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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
        //end : prcDeleteData

        public string prcDeleteDataWareHouse(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Warehouse " +
                                         " WHERE WHID = " + ItemId + "");
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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
        //end : prcDeleteData

        public string prcDeleteDataWareHouseBin(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Warehouse_Bin " +
                                         " WHERE BinId = " + ItemId + "");
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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

        public string prcDeleteDataBrand(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            try
            {
                var sqlQuery = "";

                sqlQuery = string.Format("DELETE FROM tblCat_Brand " +
                                         " WHERE BrandId = " + ItemId + "");
                arQuery.Add(sqlQuery);

                string Trans2 = "Insert Into tblUser_Trans_Log (LUserId, formName, tranStatement, tranType, PcName) Values ('" + Session["UserId"] + "', 'CategoryController', '" + sqlQuery.Replace("'", "|") + "', 'Delete', '" + clsProc.softifyPCName() + "')";
                arQuery.Add(Trans2);

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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