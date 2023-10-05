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
    public class PurchaseReturnController : Controller
    {
        public int comId = clsCommon.ComID;
        public static DataSet dsList = new DataSet();
        public static DataSet dsEdit = new DataSet();

        public ActionResult Index()
        {
            return View();
        }
        public string GetReturnList(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "EXEC prcGet_PurchaseReturnList " + comId + ", '" + dtFrom + "', '" + dtTo + "' ";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            DataTable dtData = (DataTable)dsList.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(PurchaseReturn model)
        {
            try
            {
                int msg = 0;
                if (model.ReturnId == 0)
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

        public string GetSupplierCombo(int GrrId = 0)
        {
            dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGet_PurchaseReturn " + comId + ",'" + GrrId + "',GrrReturnCombo";
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
                sqlQuery = "Exec [prcGet_PurchaseReturn] " + comId + ",'" + Id + "' ";
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
        public ActionResult Edit(PurchaseReturn model)
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

        public string prcDataSave(PurchaseReturn model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = $"SELECT cast(Isnull(MAX(ReturnId),0) + 1 AS float)  AS NewId FROM tblPurchase_Return_Main Where ComId = {comId} ";
                double ReturnId = clsCon.softifyCountingDataDouble(sqlQuery);                

                sqlQuery = "Insert Into tblPurchase_Return_Main(ReturnId,ReturnNo,dtReturn,SupplierId,GrrId,ReturnType,Total,Remarks,PCName,ComId)" +
                         " Values ('" + ReturnId + "', dbo.fncNewId('GrrReturn', '" + ReturnId + "', '"+comId+"'),'" + model.dtReturn.ToString("dd-MMM-yyyy") + "','" + model.SupplierId + "','" + model.GrrId + "','" + model.ReturnType + "','" + model.Total + "','" + model.Remarks + "','" + clsProc.softifyPCName() + "','" + comId + "')";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "Insert", "PurchaseReturn", "tblPurchase_Return_Main"));
                //END : Transaction Log 

                if (model.GatePurchaseReturnSubList != null && model.GatePurchaseReturnSubList.Count > 0)
                {
                    for (int i = 0; i < model.GatePurchaseReturnSubList.Count; i++)
                    {
                     
                        int RowNo = i + 1;
                        sqlQuery = "INSERT INTO tblPurchase_Return_Sub (ComId,ReturnId, ProductId, Qty, ReturnQty, Rate, Amount, RowNo) Values ('" + comId + "','" + ReturnId + "','" + model.GatePurchaseReturnSubList[i].ProductId + "','" + model.GatePurchaseReturnSubList[i].Qty + "','" + model.GatePurchaseReturnSubList[i].ReturnQty + "','" + model.GatePurchaseReturnSubList[i].Rate + "','" + model.GatePurchaseReturnSubList[i].Amount + "','" + RowNo + "')";
                        arQuery.Add(sqlQuery);

                        arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "Insert", "PurchaseReturn", "tblPurchase_Return_Sub"));
                    }
                }

                //Update stock 
                sqlQuery = $"Exec prcStockUpdateByPurchaseReturn {comId}, {ReturnId}, 0 ";
                arQuery.Add(sqlQuery);

                /* START : Transaction Log */
                arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "StockUpdatePurRetrn", "PurchaseReturn", "tblPurchase_Return_Sub"));
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
            //} //end : prcDataSave

        }

        public string prcUpdateData(PurchaseReturn model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                //Adjust stock 
                sqlQuery = $"Exec prcStockUpdateByPurchaseReturn {comId}, {model.ReturnId}, 1  ";
                arQuery.Add(sqlQuery); 
                // Adjusted stock
               
                sqlQuery = " DELETE FROM tblPurchase_Return_Sub  WHERE ReturnId = " + model.ReturnId + " ";
                arQuery.Add(sqlQuery);

                sqlQuery = "SELECT cast(Isnull(MAX(ReturnId),0) + 1 AS float)  AS NewId FROM tblPurchase_Return_Main";
                double ReturnId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "UPDATE tblPurchase_Return_Main SET ComId='"+comId+"', SupplierId='" + model.SupplierId + "',GrrId='" + model.GrrId + "',ReturnType='" + model.ReturnType + "',dtReturn = '" + model.dtReturn.ToString("dd-MMM-yyyy") + "',Total = '" + model.Total + "',PCName='" + clsProc.softifyPCName() + "' WHERE ReturnId = " + model.ReturnId + "";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "UPDATE", "PurchaseReturn", "tblPurchase_Return_Main"));
                //END : Transaction Log 

                #region Sub

                for (int i = 0; i < model.GatePurchaseReturnSubList.Count; i++)
                {                  
                  
                    arQuery.Add(clsCommon.TransLogInsert(model.ReturnId, sqlQuery, "Update", "PurchaseReturnController", "tblPurchase_Return_Sub"));

                    int RowNo = i + 1;
                    sqlQuery = "INSERT INTO tblPurchase_Return_Sub (ComId,ReturnId, ProductId, Qty, ReturnQty, Rate, Amount, RowNo)" +
                                    " Values ('"+comId+"','" + model.ReturnId + "','" + model.GatePurchaseReturnSubList[i].ProductId + "','" + model.GatePurchaseReturnSubList[i].Qty + "','" + model.GatePurchaseReturnSubList[i].ReturnQty + "','" + model.GatePurchaseReturnSubList[i].Rate + "','" + model.GatePurchaseReturnSubList[i].Amount + "','" + RowNo + "')";
                    arQuery.Add(sqlQuery);

                    arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "Insert", "PurchaseReturn", "tblPurchase_Return_Sub"));
                }
                #endregion Sub

                //Adjust stock 
                sqlQuery = $"Exec prcStockUpdateByPurchaseReturn {comId}, {model.ReturnId}, 0 ";
                arQuery.Add(sqlQuery);

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

        public string prcDeleteData(int ItemId,int SupplierId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            try
            {
                var sqlQuery = "";
                //Update stock 
                sqlQuery = $"Exec prcStockUpdateByPurchaseReturn 2, {ItemId}, 1 ";
                arQuery.Add(sqlQuery);

                sqlQuery = "DELETE FROM  tblPurchase_Return_Main WHERE ReturnId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "PurchaseReturn", "tblPurchase_Return_Sub"));
                //END : Transaction Log 

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