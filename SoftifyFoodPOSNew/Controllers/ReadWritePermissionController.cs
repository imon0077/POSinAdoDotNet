using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Collections;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter; 
using SoftifyFoodPOSNew.Models;
 
namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class ReadWritePermissionController : Controller
    {
        public int comId = clsCommon.ComID;
        /*Procedures : prcGetPermission_User & prcLoadMenuSave */
        public ActionResult Create()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
             
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, "Exec prcGetPermission_User " + Session["ComId"] + ", " + 0 + ", '" + 0 + "'");
             
            List<clsCommon.clsCombo2> Users = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            ViewBag.UsersList = Users;
             
            return View();
        }
        

        public string GetData(string Id = "")
        {
            DataSet ds = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection( true);
            string sqlQuery = "Exec prcGetPermission_User " + Session["ComId"] + ", " + Session["UserId"] + ", '" + Id + "', '', 'SoftifyFoodPOSNew'  ";
            clsCon.softifyFillDatasetUsingSQLCommand(ref ds, sqlQuery);
            return clsCommon.JsonSerializeDataSet(ds);
        }

         
        public string prcMenuPermissionGrid(MenuPermission model, int userId)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "SELECT  cast(Isnull(MAX(UserMenuId),0) AS float)  AS NewId FROM tblLogin_User_Menu_Main WHERE Userid = " + userId + " AND ComId = '" + Session["ComId"] + "' ";
                double OldId = clsCon.softifyCountingDataDouble(sqlQuery);

                if (OldId > 0)
                {
                    sqlQuery = " Delete  tblLogin_User_Menu_sub Where UserMenuId = " + OldId + " AND ComId = '" + Session["ComId"] + "' ";
                    arQuery.Add(sqlQuery);

                    /* START : Transaction Log */
                    arQuery.Add(clsCommon.TransLogInsert(OldId, sqlQuery, "Insert", "MenuPermissionControler", "tblLogin_User_Menu_Main"));
                    /* END : Transaction Log */

                    for (int i = 0; i < model.MPMenuList.Count; i++)
                    {                        
                        sqlQuery = " Insert Into tblLogin_User_Menu_Sub (UserMenuId, MenuId, aId, IsUpdate, IsDelete, IsMaster, ComId)  " +
                                   " Values (" + OldId + ", " + model.MPMenuList[i].MenuId.ToString() + ", " + i + ", '"+ Convert.ToInt16(model.MPMenuList[i].IsUpdate) + "', '" + Convert.ToInt16(model.MPMenuList[i].IsDelete) + "', '" + Convert.ToInt16(model.MPMenuList[i].IsMaster) + "', '" + Session["ComId"] + "')";
                        arQuery.Add(sqlQuery);

                        /* START : Transaction Log */
                        arQuery.Add(clsCommon.TransLogInsert(OldId, sqlQuery, "Insert", "MenuPermissionControler", "tblLogin_User_Menu_Sub"));
                        /* END : Transaction Log */
                    }

                    sqlQuery = "Exec prcLoadMenuSave " + OldId + ", 'SoftifyFoodPOSNew','"+Session["ComId"]+"'";
                    arQuery.Add(sqlQuery);
                }
                else
                {
                    sqlQuery = "SELECT  cast(Isnull(MAX(UserMenuId),0) + 1 AS float)  AS NewId FROM tblLogin_User_Menu_Main WHERE ComId = '" + Session["ComId"] + "' ";
                    double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                    sqlQuery = " Insert Into tblLogin_User_Menu_Main (ComId, UserMenuId, UserId, dtDate, LUserId, PCName, ProjectN)  " +
                               " Values ('" + Session["ComId"] + "', " + NewId + ", " + userId + ",'" + clsProc.softifyDateFormat(System.DateTime.Now.ToString()) + "', '" + Session["UserId"] + "', '" + clsProc.softifyPCName() + "', 'SoftifyFoodPOSNew')";
                    arQuery.Add(sqlQuery);

                    /* START : Transaction Log */
                    arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "MenuPermissionControler", "tblLogin_User_Menu_Main"));
                    /* END : Transaction Log */

                    for (int i = 0; i < model.MPMenuList.Count; i++)
                    {
                        sqlQuery = " Insert Into tblLogin_User_Menu_Sub (UserMenuId, MenuId, aId, IsUpdate, IsDelete, IsMaster, ComId)  " +
                                   " Values (" + NewId + ", " + model.MPMenuList[i].MenuId.ToString() + ", " + i + ", '" + Convert.ToInt16(model.MPMenuList[i].IsUpdate) + "', '" + Convert.ToInt16(model.MPMenuList[i].IsDelete) + "','" + Convert.ToInt16(model.MPMenuList[i].IsMaster) + "', '" + Session["ComId"] + "' )";
                        arQuery.Add(sqlQuery);

                        /* START : Transaction Log */
                        arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "MenuPermissionControler", "tblLogin_User_Menu_Sub"));
                        /* END : Transaction Log */                     
                    }
                    sqlQuery = "Exec prcLoadMenuSave " + NewId + ", 'SoftifyFoodPOSNew' ,'" + Session["ComId"] + "' ";
                    arQuery.Add(sqlQuery);
                }

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
         
    } //end : Controller
}