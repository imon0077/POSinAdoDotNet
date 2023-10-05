using System;
using System.Collections;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using System.Linq;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class ClientController : Controller
    {
        public int comId = clsCommon.ComID;
        public static DataSet dsEdit = new DataSet();
        public static DataSet dsListEdit = new DataSet();
        public string GetComboLoad()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec [prcGet_Client] 'Client', " + comId + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public string GetClientList()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = "EXEC [prcGet_ClientList] " + comId + ", 0";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Client model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = prcDataSave(model);
                    if (msg.Contains("saved"))
                    {
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msgModelState = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return Json(msgModelState, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Edit(int id)
        {
            Client model = new Client();
            model.ClientId = id;
            return View(model);
        }

        public string GetClientById(int id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "EXEC [prcGet_ClientList]  " + comId + ", " + id;
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                clsCon = null;
            }
        }

        [HttpPost]
        public ActionResult Edit(Client model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = prcUpdateData(model);
                    if (msg.Contains("updated"))
                    {
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msgModelState = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return Json(msgModelState, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        } //end : Edit
        public string prcDataSaveShortly(Client model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = $"SELECT Cast(isNull(MAX(ClientId),0)+1 AS float)  AS ClientId FROM tblClient_Information where ComId = {Session["ComId"]} ";
                double ClientId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "INSERT INTO tblClient_Information(ComID, ClientId, ClientName, ClientCode, Mobile, shortName, ClientAddress, LUserId, dtEntry ) " +
                        " VALUES (" + comId + ", " + ClientId + ", '" + model.ClientName + "', dbo.fncNewId('CUSTOMER', " + ClientId + ", "+comId+"), '" + model.Mobile + "', '" + model.shortName + "','" + model.ClientAddress + "', " + Session["UserId"] + ", GetDate() )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ClientId, sqlQuery, "Insert", "ClientController", "tblClient_Information"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return ClientId.ToString();

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
        } //end : 

        public string prcDataSave(Client model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "SELECT Cast(isNull(MAX(ClientId),0) + 1 AS float) AS ClientId FROM tblClient_Information";
                double ClientId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "INSERT INTO tblClient_Information(ComID, ClientId, ClientCode,  ClientName,shortName, ClientAddress, AccId,  Phone, Mobile, Email, web, AreaIncharge, OPBalance,  dateOP, IsAllowCredit, CreditLimit, ReferanceId, LUserId, dtEntry) VALUES (" + comId + ", " + ClientId + ",dbo.fncNewId('CUSTOMER', " + ClientId + ", "+Session["ComId"]+"),  '" + model.ClientName + "', '" + model.shortName + "','" + model.ClientAddress + "', 0,  '" + model.Phone + "', '" + model.Mobile + "', '" + model.Email + "', '" + model.Web + "', '" + model.AreaInchargeId + "', '" + model.OPBalance + "',  '" + clsProc.softifyDateFormat(model.dateOP.ToString() ) + "', 0, 0, 0, " + Session["UserId"] + ", GetDate() )" ;
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ClientId, sqlQuery, "Insert", "Client", "tblClient_Information"));
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
        } //end : 

        public string prcUpdateData(Client model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {
                sqlQuery = $"UPDATE tblClient_Information SET ClientName = '{model.ClientName}', shortName = '{model.shortName}', ClientAddress = '{model.ClientAddress}', Phone = '{model.Phone}', Mobile = '{model.Mobile}', Email = '{model.Email}', OPBalance = '{model.OPBalance}', web='{model.Web}', Remarks = '{model.Remarks}', UpdatedById = {Session["UserId"]}, dtUpdate = GetDate() Where ComID = {comId} And ClientId = {model.ClientId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.ClientId, sqlQuery, "Update", "Client", "tblClient_Information"));
                //END : Transaction Log

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
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
        } //end : 

        public string prcDeleteData(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            string sqlQuery = "";
            try
            {
                sqlQuery = $"DELETE tblClient_Information WHERE ComId = {Session["ComId"]} And ClientId = {ItemId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Client", "tblClient_Information"));
                //END : Transaction Log

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
            }
        }
    }
}