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
    public class ExpensesController : Controller
    {
        public int comId = clsCommon.ComID;
        public static DataSet dsEdit = new DataSet();
        List<clsCommon.clsCombo2> HeadList = new List<clsCommon.clsCombo2>();
        List<clsCommon.clsCombo1> TypeList = new List<clsCommon.clsCombo1>();

        public string GetComboLoad()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec prcGet_Expenses " + comId + ",0 ";
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

        //Expense list
        public string GetExpensesList(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = "EXEC prcGet_ExpensesList " + comId + ", '" + dtFrom + "', '" + dtTo + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
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

        public string GetAccNo(int Id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = "EXEC prcGet_Expenses " + comId + ", '" + Id + "','AccBalance'";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
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
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Expenses model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = prcDataSave(model);
                    if (msg.Contains("saved"))
                        return Json("1", JsonRequestBehavior.AllowGet);
                    else
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
        public ActionResult ApproveRequisition()
        {
            return View("ApproveRequisition");
        }

        public ActionResult Edit(int Id = 0)
        {
            Expenses model = new Expenses();
            model.ExpId = Id;
            return View(model);
        }

        public string GetExpneseById(int Id = 0)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec [prcGet_Expenses] " + comId + ",'" + Id + "' ";
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


        [HttpPost]
        public ActionResult Edit(Expenses model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = prcUpdateData(model);
                    if (msg.Contains("updated"))
                        return Json("1", JsonRequestBehavior.AllowGet);
                    else
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

        public string prcDataSave(Expenses model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                model.ChequeDate = (string.IsNullOrEmpty(model.ChequeNo)) ? "" : model.ChequeDate;

                sqlQuery = $"SELECT Cast(isNull(MAX(ExpId),0) + 1 AS float) AS NewId FROM tblExpense_Main Where ComId = {Session["ComId"]} ";
                double ExpId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "Insert Into tblExpense_Main(ComId, ExpId, ExpNo, dtExp, ExpType, HeadId, ChequeNo, ChequeDate, Description, Total, LUserId, PCName, dtEntry) Values ('" + comId + "', '" + ExpId + "', dbo.fncNewId('EXP', '" + ExpId + "', "+Session["ComId"]+"), '" + clsProc.softifyDateFormat(model.dtExp.ToString()) + "','" + model.ExpType + "','" + model.HeadId + "','" + model.ChequeNo + "','" + model.ChequeDate + "','" + model.Description + "','" + model.Total + "', '" + Session["UserId"] + "','" + clsProc.softifyPCName() + "', GetDate() )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ExpId, sqlQuery, "Insert", "Expenses", "tblExpense_Main"));
                //END : Transaction Log 

                if (model.GateExpensesSubList != null && model.GateExpensesSubList.Count > 0)
                {
                    for (int i = 0; i < model.GateExpensesSubList.Count; i++)
                    {                    
                        sqlQuery = "INSERT INTO tblExpense_Sub (ComId, ExpId, HeadId, Amount, Remarks, RowNo) Values (" + Session["ComId"]+",'" + ExpId + "','" + model.GateExpensesSubList[i].HeadId + "', '" + model.GateExpensesSubList[i].Amount + "','" + model.GateExpensesSubList[i].Remarks + "', '" + model.GateExpensesSubList[i].RowNo + "')";
                        arQuery.Add(sqlQuery);

                        arQuery.Add(clsCommon.TransLogInsert(ExpId, sqlQuery, "Insert", "ExpensesController", "tblExpense_Sub"));
                    }
                }

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            finally
            {
                clsCon = null;
                arQuery = null;
                clsProc = null;
            }
        } //end : prcDataSave

        public string prcUpdateData(Expenses model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            var sqlQuery = "";
            try
            {                
                sqlQuery = $"DELETE FROM tblExpense_Sub WHERE ComId = {Session["ComId"]} And ExpId = {model.ExpId} ";
                arQuery.Add(sqlQuery);

                sqlQuery = $"UPDATE tblExpense_Main SET dtExp = '{clsProc.softifyDateFormat(model.dtExp.ToString())}', ExpType = '{model.ExpType}', HeadId= {model.HeadId}, ChequeNo = '{model.ChequeNo}', ChequeDate = '{model.ChequeDate}', Description='{model.Description}', Total='{model.Total}', UpdatedById='{Session["UserId"]}', dtUpdate = GetDate() WHERE ComId ={Session["ComId"]} And ExpId ={model.ExpId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.ExpId, sqlQuery, "UPDATE", "Expenses", "tblExpense_Main"));
                //END : Transaction Log 

                if (model.GateExpensesSubList != null && model.GateExpensesSubList.Count > 0)
                {
                    for (int i = 0; i < model.GateExpensesSubList.Count; i++)
                    {
                        sqlQuery = $"INSERT INTO tblExpense_Sub (ComId, Expid, HeadId, Amount, Remarks, RowNo) Values ({Session["ComId"]}, {model.ExpId}, {model.GateExpensesSubList[i].HeadId}, '{model.GateExpensesSubList[i].Amount}','{clsProc.softifyAvoidSingleQuote(model.GateExpensesSubList[i].Remarks)}', {model.GateExpensesSubList[i].RowNo})";
                        arQuery.Add(sqlQuery);

                        //START : Transaction Log 
                        arQuery.Add(clsCommon.TransLogInsert(model.ExpId, sqlQuery, "Insert", "Expenses", "tblExpense_Sub"));
                        //END : Transaction Log 
                    }
                }
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
                arQuery = null;
                clsProc = null;
            }
        } //end : prcUpdateData

        public string prcDeleteData(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            var sqlQuery = "";
            try
            {                
                sqlQuery = "DELETE FROM tblExpense_Main WHERE ComId = "+Session["ComId"]+" And ExpId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "ExpensesController", "tblExpense_Main"));
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
                arQuery = null;
                clsProc = null;
            }
        }
        
    }
}