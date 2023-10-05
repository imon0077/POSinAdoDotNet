using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftifyFoodPOSNew.Models;
using System.Data;
using Softify; 


namespace SoftifyFoodPOSNew.Controllers
{
    public class LoginController : Controller
    {
        public int AdminLoginType { get; private set; }
        private string root;
        //
        // GET: /Login/
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["DisplayName"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult Login(Login model)
        {
            
            model = Models.Login.prcGetData(model.UserName, model.Password);
            if (model.UserId != 0)
            {
                Session["DisplayName"] = model.DisplayName;
                Session["UserId"] = model.UserId;
                Session["ComId"] = model.ComId;             
               
                Session["InvoicePrintSize"] = model.InvoicePrintSize;
                Session["BranchId"] = model.BranchId;
                Session["EmpPicLocation"] = model.EmpPicLocation;
                Session["Dashboard"] = model.Dashboard;
                clsCommon.ComID = model.ComId;
                clsCommon.UserID = model.UserId;
                prcLoadMenu(model.UserId.ToString());

                return RedirectToAction("Index", "Dashboard");
                //return RedirectToAction(clsCommon.ActionNameP, clsCommon.ControllerNameP);
            }
            else
            {
                model.Password = "";
                model.UserName = "";
                ViewBag.Message = "Invalid user name or password.";
            }
            return View();
        }
        
        public void prcLoadMenu(string UserId)
        {
            DataSet dsList = new DataSet();
            Login login = new Login();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            root = GetType().Namespace.Split('.')[0];
            try
            {
                string strQuery = "Exec prcGetMenuSecurity '" + Session["ComId"] + "', '" + UserId + "', 'SoftifyFoodPOSNew' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, strQuery);
                dsList.Tables[0].TableName = "menu";
                Session["Menu"] = dsList.Tables["menu"];
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                dsList = null;
                clsProc = null;
                clsCon = null;
            }
        }
        public ActionResult Logout()
        {
            Session["DisplayName"] = null;
            Session["Menu"] = null;
             
          return RedirectToRoute("Acsol");
        }


        // Start : MenuRefresh
        public ActionResult MenuRefresh()
        {
            if (Session["DisplayName"] != null)
            {
                DataSet dsList = new DataSet(); 
                SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
                softifyInterfaceHelper clsProc = new softifyInterfaceHelper(); 
                try
                {
                    string strQuery = "Exec prcGetMenuSecurity '" + Session["UserId"] + "', 'SoftifyFoodPOSNew' ";
                    clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, strQuery);
                    dsList.Tables[0].TableName = "menu";
                    Session["Menu"] = dsList.Tables["menu"];
                    
                    return RedirectToAction("Index", "Dashboard");
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    dsList = null;
                    clsProc = null;
                    clsCon = null;
                }                
            }

            return RedirectToRoute("Acsol");
        }
        // End : MenuRefresh



    }  // End : Controller
} // End : namespace