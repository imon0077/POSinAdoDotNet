using System;
using System.Collections;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;


namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class WarehouseController : Controller
    {
        DataSet dsList = new DataSet();


        #region Warehouse
        public ActionResult AddWarehouse()
        {
            return View();
        }
        public string GetWarehouseList()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = $"Exec prcGetWarehouse {Session["ComId"]}, 0 ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
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


        [HttpPost]
        public ActionResult AddWarehouse(Warehouse model)
        {
            try
            {
                string msg = "";
                if (model.WHId > 0)
                {
                    msg = prcDataUpdate(model);
                }
                else
                {
                    msg = prcDataSave(model);
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

        public string prcDataSave(Warehouse model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = $"Select Cast(Isnull(MAX(WHId),0) + 1 AS float) As NewId From tblCat_Warehouse Where ComId = {Session["ComId"]} ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data
                sqlQuery = $"Insert Into tblCat_Warehouse (ComId, WHId, WHName, LUserId) values ({Session["ComId"]}, {NewId}, '{model.WHName}', {Session["UserId"]} )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Warehouse", "tblCat_Warehouse"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSaveBrand

        public string prcDataUpdate(Warehouse model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                //Update color data
                sqlQuery = $"Update tblCat_Warehouse Set WHName = '{clsProc.softifyAvoidSingleQuote(model.WHName)}' Where ComId = {Session["ComId"]}  And WHId = {model.WHId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.WHId, sqlQuery, "Update", "Warehouse", "tblCat_Warehouse"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataUpdateBrand

        public string prcDeleteData(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = $"Delete tblCat_Warehouse Where ComId = {Session["ComId"]} And WHId = {ItemId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Warehouse", "tblCat_Warehouse"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        }

    # endregion Warehouse Arrea


    }
}