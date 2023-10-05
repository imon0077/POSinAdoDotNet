using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using SoftifyFoodPOSNew.Models;
using Softify;
using System.Net;
using System.Collections;
using SoftifyFoodPOSNew.CustomeFilter; 

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class LoginMenuController : Controller
    {
        DataSet dsList = new DataSet();
        List<clsCommon.clsCombo2> Menus = new List<clsCommon.clsCombo2>();
        private string root;

        public ActionResult Index()
        {
            if (Session["DisplayName"] == null)
            {
                return RedirectToRoute("Acsol"); ;
            }
            root = GetType().Namespace.Split('.')[0];
            prcLoadCombo();
            var list = LoginMenu.prcGetData();
            return View(list.ToList());
        }

        public ActionResult Details(Int32 Id)
        {
            if (Session["DisplayName"] == null)
            {
                return RedirectToRoute("Acsol"); ;
            }
            if (Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LoginMenu model = LoginMenu.prcGetData(Id);
            DataSet dsList = new DataSet();
            dsList = LoginMenu.prcGetData("", Id.ToString());
            ViewBag.LoginCategory = dsList.Tables[0].Rows[0];

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public ActionResult Create()
        {
            if (Session["DisplayName"] == null)
            {
                return RedirectToRoute("Acsol"); 
            }

            prcLoadCombo();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoginMenu model)
        {
           
            if (ModelState.IsValid)
            {
                prcDataSave(model);

                return RedirectToAction("Create").Success("Data Updated Successfully");
            }
            prcLoadCombo();
            ViewBag.Parent = model.ParentId;
            return View(model);
        }

        public ActionResult Edit(Int32 id)
        {
            if (Session["DisplayName"] == null)
            {
                return RedirectToRoute("Acsol"); ;
            }
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            prcLoadCombo();
            LoginMenu model = new LoginMenu();
            model = LoginMenu.prcGetData(id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoginMenu model)
        {
            if (ModelState.IsValid)
            {
                prcUpdateData(model);

                return RedirectToAction("Index").Success("Data Updated Successfully");
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Department/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        } //end : Delete
        

        #region porcdures

        public void prcLoadCombo()
        {
            dsList = LoginMenu.prcGetData("Config", "0");
            Menus = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            ViewBag.Parents = Menus;
        }

        public string prcDataSave(LoginMenu model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            root = GetType().Namespace.Split('.')[0];
            try
            {
                var sqlQuery = "Select cast(Isnull(MAX(MenuId),0) + 1 as float)  As CatId From tblLogin_Menu";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                int isParent = 0;
                if (model.IsParent == true)
                {
                    isParent = 1;
                    model.MenuController = "#";
                    model.MenuLink = "#";
                }

                int isInactive = 0;
                if (model.IsInactive == true) isInactive = 1;

                sqlQuery = "Insert Into tblLogin_Menu(MenuId, MenuName, MenuController, MenuLink, ParentId, IsParent, IsInactive, LUserId, PCName,MenuIcon, ProjectName)  Values ( " +
                        NewId + ", '" + model.MenuName + "', '" + model.MenuController + "', '" + model.MenuLink + "', '" +
                        model.ParentId + "', '" + isParent + "', '" + isInactive + "', '" + Session["UserId"] + "', '" +
                        clsProc.softifyPCNameWeb(Request.UserHostAddress) + "', '" + model.MenuIcon + "', 'SoftifyFoodPOSNew' )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "LoginMenu", "tblLogin_Menu"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Sucessfully Save.";
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

        public string prcUpdateData(LoginMenu model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            root = GetType().Namespace.Split('.')[0];
            try
            {

                int isParent = 0;
                if (model.IsParent == true)
                {
                    isParent = 1;
                    model.MenuController = "#";
                    model.MenuLink = "#";
                }

                int isInactive = 0;
                if (model.IsInactive == true) isInactive = 1;

                string sqlQuery = "Update tblLogin_Menu Set MenuName = '" + clsProc.softifyAvoidSingleQuote(model.MenuName) + "',MenuIcon = '" + clsProc.softifyAvoidSingleQuote(model.MenuIcon) + "', MenuController = '" +
                                        clsProc.softifyAvoidSingleQuote(model.MenuController.ToString()) + "', MenuLink = '" + clsProc.softifyAvoidSingleQuote(model.MenuLink.ToString()) + "', ParentId = '" + clsProc.softifyAvoidSingleQuote(model.ParentId.ToString()) + "', IsParent = '" + isParent + "', IsInactive = '" + isInactive + "', ProjectName = 'SoftifyFoodPOSNew'  Where MenuId = " + model.MenuId + "";

                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.MenuId, sqlQuery, "Update", "LoginMenu", "tblLogin_Menu"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Sucessfully Updated.";
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

        public string prcDeleteData(int ItemId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = String.Format("Delete From tblLogin_Menu " +
                                         " Where MenuId = " + ItemId + "");
                 
                clsCon.softifyDataSaveUsingSqlCommend(sqlQuery);
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
        #endregion porcdures
    }
}