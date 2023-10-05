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
    public class MiscellaneousController : Controller
    {
        DataSet dsList = new DataSet();

        public static List<clsCommon.clsCombo2> CategoryList = new List<clsCommon.clsCombo2>();
        public static List<clsCommon.clsCombo2> BinWHList = new List<clsCommon.clsCombo2>();


        #region LC Area
        public ActionResult AddLC()
        {
            return View();
        }
        public string GetLCList()
        {
            dsList = Miscellaneous.prcGetDataLC();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }


        [HttpPost]
        public ActionResult AddLC(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.LCId > 0)
                {
                    msg = prcDataUpdateLC(model);
                }
                else
                {
                    msg = prcDataSaveLC(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveLC(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = $"Select Cast(Isnull(MAX(LCId),0) + 1 AS float) As NewId From tblLc Where ComId = {Session["ComId"]}";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = $"Insert Into tblLc (ComId, LCId, LCNo, PCName,LUserId) Values ({Session["ComId"]}, {NewId}, '{clsProc.softifyAvoidSingleQuote(model.LCNo)}', '{clsProc.softifyPCName()}', {Session["UserId"]} )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblLc"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdateLC(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = $"Update tblLc Set LCNo = '{clsProc.softifyAvoidSingleQuote(model.LCNo)}' Where ComId = {Session["ComId"]} And LCId = {model.LCId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.BrandId, sqlQuery, "Update", "Miscellaneous", "tblLc"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteDataLC(int itemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = $"Delete tblLc Where ComId = {Session["ComId"]}  And LCId = {itemId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(itemId, sqlQuery, "Delete", "Miscellaneous", "tblLc"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Brand Arrea

        #region Weight Area
        public ActionResult AddWeight()
        {
            return View();
        }
        public string GetWeightList()
        {
            dsList = Miscellaneous.GetWeightList();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }


        [HttpPost]
        public ActionResult AddWeight(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.WeightId > 0)
                {
                    msg = prcDataUpdateWeight(model);
                }
                else
                {
                    msg = prcDataSaveWeight(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveWeight(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = $"Select Cast(Isnull(MAX(WeightId),0) + 1 AS float) As NewId From tblWeight Where ComId = {Session["ComId"]}";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = $"Insert Into tblWeight (ComId, WeightId, Weight, PCName,LUserId) Values ({Session["ComId"]}, {NewId}, '{clsProc.softifyAvoidSingleQuote(model.Weight)}', '{clsProc.softifyPCName()}', {Session["UserId"]} )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblLc"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdateWeight(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = $"Update tblWeight Set Weight = '{clsProc.softifyAvoidSingleQuote(model.Weight)}' Where ComId = {Session["ComId"]} And WeightId = {model.WeightId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.BrandId, sqlQuery, "Update", "Miscellaneous", "tblWeight"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteDataWeight(int itemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = $"Delete tblWeight Where ComId = {Session["ComId"]}  And WeightId = {itemId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(itemId, sqlQuery, "Delete", "Miscellaneous", "tblWeight"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Brand Arrea


        #region Brand Arrea
        public ActionResult AddBrand()
        {
            return View();
        }
        public string GetBrandList()
        {
            dsList = Miscellaneous.prcGetDataBrand();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }


        [HttpPost]
        public ActionResult AddBrand(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.BrandId > 0)
                {
                    msg = prcDataUpdateBrand(model);
                }
                else
                {
                    msg = prcDataSaveBrand(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveBrand(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = "Select Cast(Isnull(MAX(BrandId),0) + 1 AS float) As NewId From tblBrand Where ComId = " + Session["ComId"] + " ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblBrand (ComId, BrandId, aId, BrandName, PCName,LUserId) "
                          + " Values (" + Session["ComId"] + ", " + NewId + "," + NewId + ", '" + clsProc.softifyAvoidSingleQuote(model.BrandName) + "', '" +
                          clsProc.softifyPCName() + "'," + Session["UserId"] + ")";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblBrand"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdateBrand(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = "Update tblBrand Set BrandName = '" + clsProc.softifyAvoidSingleQuote(model.BrandName) + "' Where ComId = " + 
                            Session["ComId"] + " And BrandId = " + model.BrandId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.BrandId, sqlQuery, "Update", "Miscellaneous", "tblBrand"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteDataBrand(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = "Delete tblBrand Where ComId = " + Session["ComId"] + " And BrandId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Miscellaneous", "tblBrand"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Brand Arrea

        #region Size
        public ActionResult AddSize()
        {
            return View();
        }
        public string GetSizeList()
        {
            dsList = Miscellaneous.prcGetDataSize();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }


        [HttpPost]
        public ActionResult AddSize(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.SizeId > 0)
                {
                    msg = prcDataUpdateSize(model);
                }
                else
                {
                    msg = prcDataSaveSize(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveSize(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            { 
                sqlQuery = "Select Cast(Isnull(MAX(SizeId),0) + 1 AS float) As NewId From tblSize Where ComId = " + Session["ComId"] + " ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblSize (ComId, SizeId, aId, SizeName, PCName,LUserId) "
                          + " Values (" + Session["ComId"] + ", " + NewId + "," + NewId + ", '" + clsProc.softifyAvoidSingleQuote(model.SizeName) + "', '" +
                          clsProc.softifyPCName() + "'," + Session["UserId"] + ")";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblSize"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdateSize(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = "Update tblSize Set SizeName = '" + clsProc.softifyAvoidSingleQuote(model.SizeName) + "' Where ComId = " +
                            Session["ComId"] + " And SizeId = " + model.SizeId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.SizeId, sqlQuery, "Update", "Miscellaneous", "tblSize"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteDataSize(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = "Delete tblSize Where ComId = " + Session["ComId"] + " And SizeId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Miscellaneous", "tblSize"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Size Arrea

        #region Unit 
        public ActionResult AddUnit()
        {
            return View();
        }
        public string GetUnitList()
        {
            dsList = Miscellaneous.prcGetDataUnit();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }


        [HttpPost]
        public ActionResult AddUnit(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.unitId > 0)
                {
                    msg = prcDataUpdateUnit(model);
                }
                else
                {
                    msg = prcDataSaveUnit(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveUnit(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = "Select Cast(Isnull(MAX(unitId),0) + 1 AS float) As NewId From tblunit Where ComId = " + Session["ComId"] + " ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblunit (ComId, unitId, unitName, PCName,LUserId) "
                          + " Values (" + Session["ComId"] + ", " + NewId + ", '" + clsProc.softifyAvoidSingleQuote(model.unitName) + "', '" +
                          clsProc.softifyPCName() + "'," + Session["UserId"] + ")";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblunit"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveUnit

        public string prcDataUpdateUnit(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = "Update tblunit Set unitName = '" + clsProc.softifyAvoidSingleQuote(model.unitName) + "' Where ComId = " +
                            Session["ComId"] + " And unitId = " + model.unitId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.unitId, sqlQuery, "Update", "Miscellaneous", "tblunit"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateUnit

        public string prcDeleteDataUnit(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = "Delete tblunit Where ComId = " + Session["ComId"] + " And unitId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Miscellaneous", "tblunit"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Unit Arrea

        #region Category
        public ActionResult AddCategory()
        {
            return View();
        }
        public string GetCategoryList()
        {
            dsList = Miscellaneous.prcGetDataCategory();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }


        [HttpPost]
        public ActionResult AddCategory(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.prodCatId > 0)
                {
                    msg = prcDataUpdateCategory(model);
                }
                else
                {
                    msg = prcDataSaveCategory(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveCategory(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = "Select Cast(Isnull(MAX(prodCatId),0) + 1 AS float) As NewId From tblCategory Where ComId = " + Session["ComId"] + " ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblCategory (ComId, prodCatId, aId, prodCatName, PCName,LUserId) "
                          + " Values (" + Session["ComId"] + ", " + NewId + "," + NewId + ", '" + clsProc.softifyAvoidSingleQuote(model.prodCatName) + "', '" +
                          clsProc.softifyPCName() + "'," + Session["UserId"] + ")";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblCategory"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdateCategory(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = "Update tblCategory Set prodCatName = '" + clsProc.softifyAvoidSingleQuote(model.prodCatName) + "' Where ComId = " +
                            Session["ComId"] + " And prodCatId = " + model.prodCatId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.prodCatId, sqlQuery, "Update", "Miscellaneous", "tblCategory"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteDataCategory(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = "Delete tblCategory Where ComId = " + Session["ComId"] + " And prodCatId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Miscellaneous", "tblCategory"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Category Arrea

        #region Sub Category
        public ActionResult AddSubCategory()
        {
            prcLoadCategory();
            return View();
        }
        public string GetSubCategoryList()
        {
            dsList = Miscellaneous.prcGetDataSubCategory();
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public void prcLoadCategory()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Select prodCatId, prodCatName From tblCategory Where ComId = " + Session["ComId"] + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                CategoryList = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            }
            catch (Exception ex) { throw (ex); }
            finally { sqlQuery = null; clsCon = null; dsList.Dispose(); }

        }

        public JsonResult GetCategoryCombo()
        {
            return Json(CategoryList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddSubCategory(Miscellaneous model)
        {
            try
            {
                string msg = "";
                if (model.prodSCatId > 0)
                {
                    msg = prcDataUpdateSubCategory(model);
                }
                else
                {
                    msg = prcDataSaveSubCategory(model);
                }
                if (msg.Contains("saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else if (msg.Contains("updated"))
                {
                    return Json("2", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failure" + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string prcDataSaveSubCategory(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = "Select Cast(Isnull(MAX(prodSCatId),0) + 1 AS float) As NewId From tblSubCategory Where ComId = " + Session["ComId"] + " ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = "Insert Into tblSubCategory (ComId, prodSCatId, aId, prodSCatName, prodCatId, PCName,LUserId) "
                          + " Values (" + Session["ComId"] + ", " + NewId + "," + NewId + ", '" + clsProc.softifyAvoidSingleQuote(model.prodSCatName) + "', '" + 
                          model.prodCatIdSub + "','" + clsProc.softifyPCName() + "'," + Session["UserId"] + ")";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Miscellaneous", "tblSubCategory"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdateSubCategory(Miscellaneous model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = "Update tblSubCategory Set prodSCatName = '" + clsProc.softifyAvoidSingleQuote(model.prodSCatName) + "',  prodCatId = '" +
                            model.prodCatIdSub + "' Where ComId = " +Session["ComId"] + " And prodSCatId = " + model.prodSCatId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.SizeId, sqlQuery, "Update", "Miscellaneous", "tblSubCategory"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteDataSubCategory(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = "Delete tblSubCategory Where ComId = " + Session["ComId"] + " And prodSCatId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Miscellaneous", "tblSubCategory"));
                //END : Transaction Log 

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
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion Sub Category Arrea


    }
}