using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Softify;
using SoftifyFoodPOSNew.Models;
using System.Collections;
using System.Net;

namespace SoftifyFoodPOSNew.Controllers
{
    public class LoginCategoryController : Controller
    {
        DataSet dsList = new DataSet();
        List<clsCommon.clsCombo2> Parents = new List<clsCommon.clsCombo2>();
        public ActionResult Index()
        {
            if (Session["DisplayName"] == null)
            {
                return RedirectToRoute("Acsol"); ;
            }
            prcLoadCombo();
            var list = LoginCategory.prcGetData();
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


            dsList = LoginCategory.prcGetData("Details", Id);
            ViewBag.LoginCategory = dsList.Tables[0].Rows[0];
            
            LoginCategory model = LoginCategory.prcGetData(Id);
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
                return RedirectToRoute("Acsol"); ;
            }
            prcLoadCombo();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoginCategory model)
        {
            if (ModelState.IsValid)
            {
                prcDataSave(model);

                return RedirectToAction("Index");
            }
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
            LoginCategory model = new LoginCategory();
            model = LoginCategory.prcGetData(id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoginCategory model)
        {
            if (ModelState.IsValid)
            {
                prcUpdateData(model);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(Int32 id)
        {
            if (Session["DisplayName"] == null)
            {
                return RedirectToRoute("Acsol"); ;
            }
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            dsList = LoginCategory.prcGetData("Details", id);
            ViewBag.LoginCategory = dsList.Tables[0].Rows[0];

            LoginCategory model = LoginCategory.prcGetData(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(LoginCategory model)
        {
            prcDeleteData(model);
            return RedirectToAction("Index");
        }

        #region porcdures

        public void prcLoadCombo()
        {
            dsList = LoginCategory.prcGetData("Config", 0);
            Parents = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            ViewBag.Parents = Parents;
        }


        public string prcDataSave(LoginCategory model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Select Isnull(MAX(CatId),0) + 1  As CatId From tblLogin_Category";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                int isParent = 0;
                if (model.IsParent == true) isParent = 1;
                int isInactive = 0;
                if (model.IsInactive == true) isInactive = 1;



                sqlQuery = String.Format("Insert Into tblLogin_Category(CatId, CatName, ParentId, IsParent, IsInactive, PCName) " +
                                         " Values ( " + NewId + ",'" + model.CatName + "','" + model.ParentId + "', '"+isParent+"','"+isInactive+"', '" + clsProc.softifyPCNameWeb(Request.UserHostAddress) + "' )");

                arQuery.Add(sqlQuery);
                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "LoginCategory", "tblLogin_Category"));
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

        public string prcUpdateData(LoginCategory model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                int isParent = 0;
                if(model.IsParent == true) isParent = 1;
                int isInactive = 0;
                if(model.IsInactive == true) isInactive = 1;

                sqlQuery = String.Format("Update tblLogin_Category Set  CatName = '"+clsProc.softifyAvoidSingleQuote(model.CatName.ToString()) +"', ParentId = '"+
                                                        clsProc.softifyAvoidSingleQuote(model.ParentId.ToString())+"', IsParent = '"+isParent+"', IsInactive = '"+
                                                        isInactive +"' Where CatId = " + model.CatId + "");

                arQuery.Add(sqlQuery);
                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Update", "LoginCategory", "tblLogin_Category"));
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

        public string prcDeleteData(LoginCategory model)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = String.Format("Delete From tblLogin_Category " +
                                         " Where CatId = " + model.CatId + "");
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