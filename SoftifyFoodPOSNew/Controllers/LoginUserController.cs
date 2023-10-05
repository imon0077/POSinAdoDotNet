using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Controllers
{
    [RoutePrefix("User/List")]
    public class LoginUserController : Controller
    {
        DataSet dsList = new DataSet();
        List<clsCommon.clsCombo2> RefList = new List<clsCommon.clsCombo2>();
        List<clsCommon.clsCombo2> BranchList = new List<clsCommon.clsCombo2>();

        public ActionResult Index()
        {
            return View();
        }

        public string GetUserList()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec dbo.prcGetLoginUser " + Session["ComId"] + ", -1 ";
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
            prcLoadCombo();
            return View();
        }

        [HttpPost]
        public ActionResult Create(LoginUser model)
        {
            string msg = "";
            try
            {
                msg = prcDataSave(model);
                if (msg.Contains("Saved"))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
    
            LoginUser model = new LoginUser();
            model = LoginUser.prcGetData(id);
            prcLoadCombo();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoginUser model)
        {
            if (ModelState.IsValid)
            {
                prcUpdateData(model);

                return RedirectToAction("Index");
            }

            return View(model);
        }


        #region porcdures

        public void prcLoadCombo()
        {
            dsList = LoginUser.prcGetDataCombo();

            RefList = clsGenerateList.prcColumnTwo(dsList.Tables[0]);
            ViewBag.RefList = RefList;

            BranchList = clsGenerateList.prcColumnTwo(dsList.Tables[1]);
            ViewBag.BranchList = BranchList;
        }

        public string prcDataSave(LoginUser model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "Select Cast(Isnull(MAX(LUserId),0) + 1 as float) As CatId From tblLogin_User";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "Insert Into tblLogin_User(ComId, LUserId, UserName, UserPass, DisplayName, CatId, RefId, BranchId, IsInactive, CreatedBy, PCName)  Values (" + Session["ComId"] + " ," + NewId + ",'" +
                            clsProc.softifyAvoidSingleQuote(model.UserName) + "','" + clsProc.softifyAvoidSingleQuote(clsProc.softifyEncryptString(model.UserPass)) + "','" +
                            clsProc.softifyAvoidSingleQuote(model.DisplayName) + "','0', '" + model.RefId + "', '" + model.BranchId + "',0,'" + Session["UserId"] + "', '"+clsProc.softifyPCName()+"' )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "LoginUser", "tblLogin_User"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Sucessfully Saved";
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

        public string prcUpdateData(LoginUser model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {

                int isInactive = 0;
                if (model.IsInactive == true) isInactive = 1;

                sqlQuery = "Update tblLogin_User Set UserName = '" + clsProc.softifyAvoidSingleQuote(model.UserName) + "', UserPass = '" +
                            clsProc.softifyAvoidSingleQuote(clsProc.softifyEncryptString(model.UserPass)) + "', DisplayName = '" +
                            clsProc.softifyAvoidSingleQuote(model.DisplayName) + "', CatId = '0', RefId = '" +
                            model.RefId + "', BranchId = "+model.BranchId+", IsInactive = '" + isInactive + "' Where LUserId = " + model.LUserId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.LUserId, sqlQuery, "Update", "LoginUser", "tblLogin_User"));
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

        public string prcDeleteData(LoginUser model)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = String.Format("Delete tblLogin_User Where LUserId = " + model.LUserId + "");
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