using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using Microsoft.Reporting.WebForms;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class ProductSerialController : Controller
    {
        // GET: ProductSerial
        public int comId = clsCommon.ComID;
        public static DataSet dsEdit = new DataSet();
        DataSet dsList = new DataSet();
      
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Stock()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public string GetProductAuto_Serial(int id = 0, string value="")
        {
            DataSet dsData = new DataSet();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = $"Exec [prcGetInv_ItemSearchAuto] {comId}, {id}, '{value}' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsData, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsData);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                clsProc = null;
                dsData = null;
            }
        }

        public string GetProductInfo(int id=0, int ProductId =0)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsCombo = new DataSet();
            string sqlQuery = "";
            try
            {
                if (id > 0)
                {
                    //Id =0 : Product info else serial info
                    sqlQuery = $"EXEC prcGetProductInfo_Search {comId}, {id}, {ProductId} ";
                }
                else
                {
                    sqlQuery = $"EXEC prcGetProductInfo_Search {comId}, {id}, {ProductId} ";
                }
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsCombo, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsCombo);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsCombo = null;
            }

            
        }

        public string GetProductList()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGetSerialInfo " + comId + ",'Info'";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }

        public string GetSerialLoad(int ProductId = 0)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGetSerialInfo " + comId + ",'','','" + ProductId + "'";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }

        public string prcDataSave(ProductSerial model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                for (int i = 0; i < model.ProductSubList.Count; i++)
                {
                    sqlQuery = "Update tblSal_Product Set StockQty = '" + model.ProductSubList[i].CurrentStock + "' Where ProductId ='" + model.ProductSubList[i].ProductId + "'And ComId = '" + comId + "'";
                    arQuery.Add(sqlQuery);
                    arQuery.Add(clsCommon.TransLogInsert(model.ProductSubList[i].ProductId, sqlQuery, "Insert", "ProductSerial", "tblSal_Product"));
                }

                if (model.GateProductSerial != null)
                {
                    for (int i = 0; i < model.GateProductSerial.Count; i++)
                    {
                        if (model.GateProductSerial[i].IsSelect == true)  // Update Serial 
                        {
                            sqlQuery = "Update tblSal_Product_Serial Set InvoiceId = 0 , IsSell='0' Where SerialId='" + model.GateProductSerial[i].SerialId + "'";
                            arQuery.Add(sqlQuery);

                            //arQuery.Add(clsCommon.TransLogInsert(model.ProductSubList[i].ProductId, sqlQuery, "Update", "InvPOSContrller", "tblSal_Product_Serial"));
                        }
                    }
                }
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
            //} //end : prcDataSave

        }


    }
}