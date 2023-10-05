using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.Models;
using SoftifyFoodPOSNew.CustomeFilter;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class SupplierController : Controller
    {
        public int ComId = clsCommon.ComID;
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Supplier model)
        {
            try
            {
                string msg = "";
                if (model.supplierId == 0)
                {
                    msg = prcDataSave(model);
                }
                else if (model.supplierId != 0)
                {
                    msg = prcUpdateData(model);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
        public string GetComboLoad()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGet_Supplier " + ComId + ",0 ";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }
        public string GetSupplierList()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsCombo = new DataSet();
            string sqlQuery = "Exec prcGet_Supplier  '"+ ComId + "', 0 ";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsCombo, sqlQuery);
            DataTable dtData = (DataTable)dsCombo.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public string prcDataSave(Supplier model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                int isClient;
                isClient = model.isClient == true ? 1 : 0;

                sqlQuery = "Select Cast(Isnull(MAX(supplierId),0) + 1 AS float)  AS NewId From tblCat_Supplier Where ComId = "+Session["ComId"]+" ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                //Insert Data  
                sqlQuery = "Insert Into tblCat_Supplier(ComId,SupplierId, SupplierCode, SupplierName, SupplierAddress, SupplierPhone, SupplierMobile, SupplierEmail, ContactName, ContactPhone,OpeningBalance,isClient, ClientId, accId, LuserId) "
                               + " Values (" + Session["ComId"] + "," + NewId + ",  dbo.fncNewId('SUPPLIER', " + NewId + ", " + ComId + "),'" + clsProc.softifyAvoidSingleQuote(model.supplierName) + "','" +
                               clsProc.softifyAvoidSingleQuote(model.SupplierAddress) + "', '" + model.SupplierPhone + "',  '" + model.SupplierMobile + "','" +
                               model.SupplierEmail + "','','','" + model.OpeningBalance + "','"+ isClient + "','"+ model.ClientId +"', 0, '" + Session["UserId"] + "')";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert( NewId, sqlQuery, "Insert", "Supplier", "tblCat_Supplier"));
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
            }
        } //end : prcDataSave


        //To Update 
        public string prcUpdateData(Supplier model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                int isClient;
                isClient = model.isClient == true ? 1 : 0;
                if (isClient == 0)
                    model.ClientId = 0;

                sqlQuery = " Update tblCat_Supplier Set supplierName = '" + clsProc.softifyAvoidSingleQuote(model.supplierName) + "', SupplierAddress='" +
                            clsProc.softifyAvoidSingleQuote(model.SupplierAddress)+ "', SupplierPhone = '" + 
                            clsProc.softifyAvoidSingleQuote(model.SupplierPhone) + "', SupplierMobile='" + 
                            clsProc.softifyAvoidSingleQuote(model.SupplierMobile) + "', SupplierEmail = '" + 
                            clsProc.softifyAvoidSingleQuote(model.SupplierEmail)+ "', CountryId = '" +
                            model.CountryId + "',OpeningBalance = '"+ model.OpeningBalance +"',isClient = '"+ isClient + "',ClientId = '"+model.ClientId+"' Where ComId = " + Session["ComId"]+ " And supplierId = " + model.supplierId;

                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.supplierId, sqlQuery, "Update", "Supplier", "tblCat_Supplier"));
                //END : Transaction Log 

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

        public string prcDeleteData(int ItemId)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                sqlQuery = "Delete tblCat_Supplier Where ComId = "+Session["ComId"]+" And supplierId = "+ItemId +" ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Supplier", "tblCat_Supplier"));
                //END : Transaction Log 

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
    }
}