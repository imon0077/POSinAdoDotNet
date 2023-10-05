using Softify;
using  SoftifyFoodPOSNew.CustomeFilter;
using  SoftifyFoodPOSNew.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace  SoftifyFoodPOSNew.Controllers
{

    [SessionFilter]
    public class ExpensesHeadController : Controller
    {
        public int ComId = clsCommon.ComID;
        DataSet dsList = new DataSet();

        #region Start : Expenses ===========================================
        List<clsCommon.clsCombo1> Typelist = new List<clsCommon.clsCombo1>();

        // Start : Combo Method
        public void prcLoadCombo()
        {
            Softify.SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery1 = "Exec [prcGet_Expense_Head] '" + ComId + "', 0";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery1);

            Typelist = clsGenerateList.prcColumnOne(dsList.Tables[0]);
            ViewBag.Typelist = Typelist;            
        }
        // End : Combo Method

        // GET: Expenses
        [HttpGet]      
        public ActionResult ExpensesHead()
        {
            prcLoadCombo();
            return View();
        }

        // Post : Expenses
        [HttpPost]
        public ActionResult ExpensesHead(ExpensesHead model)
        {
            try
            {
                int message = 0;
                if (model.HeadId == 0)
                {
                    SaveExpenses(model);
                    message = 1;
                }
                else if (model.HeadId != 0)
                {
                    PrcUpdateDataExpenses(model);
                    message = 2;
                }
                return Json(message);
            }
            catch (Exception ex)
            {
                return Json("Failure:" + ex.Message);
            }
        }


        // Start : list Method 
        public string GetExpenseslist()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsCombo = new DataSet();
            string sqlQuery = "Exec [prcGet_Expense_Head] '" + ComId + "', 0";

            clsCon.softifyFillDatasetUsingSQLCommand(ref dsCombo, sqlQuery);
            DataTable dtData = (DataTable)dsCombo.Tables[1];
            return clsCommon.JsonSerialize(dtData);
        } 
        //  End : list Mehthod 


        // Start :  Save
        public string SaveExpenses(ExpensesHead model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";

                sqlQuery = "SELECT  cast(Isnull(MAX(HeadId),0) + 1 AS float)  AS HeadId  FROM tblExp_Head";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "INSERT INTO tblExp_Head (HeadId, HeadName, HeadType, AccNo, Balance,ComId)Values ('" + NewId + "', '" + model.HeadName + "', '" + model.HeadType + "', '"+ model.AccNo +"','" + model.Balance + "', '" + Session["ComId"] + "')";
                arQuery.Add(sqlQuery);

                // START : Transaction Log
                        arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "ExpensesHead", "tblExp_Head"));
                //END : Transaction Log 

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
        } // End : Save


        public string PrcUpdateDataExpenses(ExpensesHead model)  
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = " ";
                sqlQuery = "SELECT  cast(Isnull(MAX(HeadId),0) + 1 AS float)  AS HeadId  FROM tblExp_Head";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "UPDate tblExp_Head SET  HeadName = '" + model.HeadName + "', HeadType = '" + model.HeadType + "',AccNo = '"+ model.AccNo +"', Balance = '" + model.Balance + "' Where HeadId = '" + model.HeadId + "' ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert( NewId,sqlQuery, "update", "Expenses", "tblExp_Head"));
                //END : Transaction Log 

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
        }
        //end : prcUpdateData


        // prcDeleteDataExpenses
        public string prcDeleteDataExpenses(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();

            try
            {
                var sqlQuery = " ";
                sqlQuery = "SELECT  cast(Isnull(MAX(HeadId),0) + 1 AS float)  AS HeadId  FROM tblExp_Head";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "DELETE FROM tblExp_Head  WHERE HeadId = " + ItemId + " ";


                arQuery.Add(sqlQuery);

                sqlQuery = string.Format("DELETE FROM tblExp_Head WHERE HeadId = " + ItemId + "");
                arQuery.Add(sqlQuery);

                //START: Transaction Log
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Delete", "Expenses", "tblExp_Head"));
                //END: Transaction Log
                
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

        #endregion End : Expenses ========================================
    }  // End  : Controller
} // End : Namespace
