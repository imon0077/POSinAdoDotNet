using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class DamagesController : Controller
    {
        public int comId = clsCommon.ComID;
        public static DataSet dsList = new DataSet();
        public static DataSet dsEdit = new DataSet();

        public ActionResult Index()
        {
            return View();
        }
        public string GetDamageList(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "EXEC prcGetProduct_Damage_List " + comId + ", '" + dtFrom + "', '" + dtTo + "' ";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Damages model)
        {
            try
            {
                int msg = 0;
                if (model.DamageId == 0)
                {
                    prcDataSave(model);
                    msg = 1;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public string GetComboLoad(int ClientId = 0)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGet_PurchaseReturn " + comId + ",'" + ClientId + "'";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }
        public string GetGrrCombo(int Id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGet_PurchaseReturn " + comId + ",'" + Id + "',GrrCombo";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }

        public string GetDamagesCombo(int GrrId = 0)
        {
            dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGet_Damagesproduct " + comId + ",'" + GrrId + "',GrrDamagesCombo";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }

        public ActionResult Edit(int Id = 0)

        {
            try
            {
                dsEdit = new DataSet();
                SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
                string sqlQuery = "";
                sqlQuery = "Exec [prcGet_DamageList] " + comId + ",'','','" + Id + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsEdit, sqlQuery);

                if (Id == 0)
                {
                    return RedirectToAction("Index");
                }
                GetComboLoad();
                return View();
            }

            catch (Exception ex)
            {
                return RedirectToAction("Index").Danger("!! Failure: " + ex.Message);
            }
        }
        public string GetEditdata()
        {
            return clsCommon.JsonSerializeDataSet(dsEdit);
        }
        [HttpPost]
        public ActionResult Edit(Damages model)
        {
            try
            {
                var message = prcUpdateData(model);
                return Json(message);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);

            }

        }

        public string prcDataSave(Damages model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = $"SELECT cast(Isnull(MAX(DamageId),0) + 1 AS float)  AS NewId FROM tblSal_Product_Damage_Main Where ComId = {comId} ";
                double DamageId = clsCon.softifyCountingDataDouble(sqlQuery);

                // TOP (200) ComId, DamageId, WHId, DamageNo, dtDamage, SupplierId, GrrId, TypeId, Remarks, dtEntry, LUserId, UpdatedById, dtUpdate, BranchId


                // Generate New Id for transaction                
                sqlQuery = "Insert Into tblSal_Product_Damage_Main(ComId, DamageId,  DamageNo, dtDamage, SupplierId, GrrId, TypeId, Remarks, Total, LUserId)" +
                         " Values ('" + comId + "', '" + DamageId + "', dbo.fncNewId('Damage', '" + DamageId + "', '" + comId + "'),'" + model.dtDamage+ "','" + model.SupplierId + "','" + model.GrrId + "','" + model.TypeId + "' ,'" + model.Remarks + "','" + model.Total + "','" + comId + "')";
                arQuery.Add(sqlQuery);

               // START: Transaction Log
                arQuery.Add(clsCommon.TransLogInsert(DamageId, sqlQuery, "Insert", "Damages", "tblSal_Product_Damage_Main"));
               // END: Transaction Log

                if (model.DamagesSubList != null && model.DamagesSubList.Count > 0)
                {
                    for (int i = 0; i < model.DamagesSubList.Count; i++)
                    {                     
                        
                        int RowNo = i + 1;
                        sqlQuery = "INSERT INTO tblSal_Product_Damage_Sub (ComId, DamageId, ProductId, Qty, Rate, Amount, RowNo) Values ('" + comId + "','" + DamageId + "','" + model.DamagesSubList[i].ProductId + "','" + model.DamagesSubList[i].Qty + "','" + model.DamagesSubList[i].Rate + "','" + model.DamagesSubList[i].Amount + "','" + RowNo + "')";
                        arQuery.Add(sqlQuery);

                        arQuery.Add(clsCommon.TransLogInsert(DamageId, sqlQuery, "Insert", "Damages", "tblSal_Product_Damage_Sub"));
                    }
                }

               // Update stock
                sqlQuery = $"Exec prcStockUpdateByDamages {comId}, {DamageId}, 0 ";
                arQuery.Add(sqlQuery);

                /* START : Transaction Log */
                arQuery.Add(clsCommon.TransLogInsert(DamageId, sqlQuery, "StockUpdateDamages", "Damages", "tblSal_Product"));
                /* END : Transaction Log */


                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                return "Successfully Save.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        } //end : prcDataSave



    public string prcUpdateData(Damages model)
    {
        ArrayList arQuery = new ArrayList();
        softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
        try
        {
            var sqlQuery = "";
            //Adjust stock
            sqlQuery = $"Exec prcStockUpdateByDamages {comId}, {model.DamageId}, 0  ";
            arQuery.Add(sqlQuery);
            // Adjusted stock

            sqlQuery = " DELETE FROM tblSal_Product_Damage_Sub  WHERE DamageId = " + model.DamageId + " ";
            arQuery.Add(sqlQuery);

            sqlQuery = "SELECT cast(Isnull(MAX(DamageId),0) + 1 AS float)  AS NewId FROM tblSal_Product_Damage_Main";
            double DamageId = clsCon.softifyCountingDataDouble(sqlQuery);

            sqlQuery = "UPDATE tblSal_Product_Damage_Main SET ComId='" + comId + "', SupplierId='" + model.SupplierId + "',GrrId='" + model.GrrId + "',TypeId='" + model.TypeId + "',dtDamage = '" + model.dtDamage+ "',Total = '" + model.Total + "',PCName='" + clsProc.softifyPCName() + "' WHERE DamageId = " + model.DamageId + "";
            arQuery.Add(sqlQuery);

            // START: Transaction Log
            arQuery.Add(clsCommon.TransLogInsert(DamageId, sqlQuery, "UPDATE", "Damages", "tblSal_Product_Damage_Main"));
            //  END: Transaction Log

          

            //Adjust stock
            sqlQuery = $"Exec prcStockUpdateByDamages {comId}, {model.DamageId}, 5 ";
            arQuery.Add(sqlQuery);

                /* START : Transaction Log */
                arQuery.Add(clsCommon.TransLogInsert(DamageId, sqlQuery, "StockUpdateDamages", "Damages", "tblSal_Product"));
                /* END : Transaction Log */

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
            return "Successfully Updated.";
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

    public string prcDeleteData(int ItemId)
    {
        SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
        ArrayList arQuery = new ArrayList();
        softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
        try
        {
            var sqlQuery = "";

                //Update stock 
                sqlQuery = $"Exec prcStockUpdateByDamages 2, {ItemId}, 1 ";
                arQuery.Add(sqlQuery);
                sqlQuery = "DELETE FROM  tblSal_Product_Damage_Main WHERE DamageId = " + ItemId + " ";
            arQuery.Add(sqlQuery);

            // START: Transaction Log
            arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Damages", "tblSal_Product_Damage_Main"));
            //END: Transaction Log

            clsCon.softifyDataSaveUsingSqlCommend(arQuery);
            return "Data Deleted Successfully.";
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
