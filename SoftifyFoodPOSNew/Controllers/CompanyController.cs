using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using System.IO;
using System.Web;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class CompanyController : Controller
    {
        DataSet dsList = new DataSet();
        public static DataSet DsListEdit = new DataSet();

        public static List<clsCommon.clsCombo1> BType = new List<clsCommon.clsCombo1>();
        //public static List<clsCommon.clsCombo1> varType = new List<clsCommon.clsCombo1>();
        public static List<Company> CompanyLoad = new List<Company>();

        public void prcLoadCombo()
        {
            dsList = Company.prcGetDatacombo();
            BType = clsGenerateList.prcColumnOne(dsList.Tables[0]);
            //varType = clsGenerateList.prcColumnOne(dsList.Tables[]);

        }


        public string GetComboLoad()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            sqlQuery = "Exec prcGet_Compnay '" + Session["ComId"] + "',0 ";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
            return clsCommon.JsonSerializeDataSet(dsList);
        }

        public JsonResult GetCompanyList()
        {
            CompanyLoad = Company.prcGetData();
            return Json(CompanyLoad.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            prcLoadCombo();
            return View();
        }


        public JsonResult prcGetCombo()
        {
            return Json(BType, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(Company model)
        {
            try
            {

                string msg = "";
                if (ModelState.IsValid)
                {
                    msg = prcDataSave(model);
                    if (msg.Contains("saved"))
                    {
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
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
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                DsListEdit = new DataSet();
                string sqlQuery = "Exec prcGet_Compnay " + Session["ComId"]+", " + id + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref DsListEdit, sqlQuery);
                prcLoadCombo();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorInfo", "Dashboard", new { exMessage = ex.Message });
            }
            finally
            {
                clsCon = null;
            }
        }


        [HttpPost]
        public ActionResult Edit(Company model)
        {
            try
            {
                string msg = "";
                if (ModelState.IsValid)
                {
                    msg = prcDataUpdate(model);
                }
                if (msg.Contains("updated"))
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


        public string GetEditData()
        {
            return clsCommon.JsonSerializeDataSet(DsListEdit);
        }
        public string prcDataSave(Company model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            HttpPostedFileBase postedFile = null;
            try
            {
                var sqlQuery = ""; 

                sqlQuery = "SELECT Cast(Isnull(MAX(comId),0) + 1 AS float)  AS NewId From tblCat_Company";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);
                string path = Server.MapPath("~/Content/assets/images/Company/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                /*  Image File Name */
                if (Request.Files.Count > 0)
                {
                        postedFile = Request.Files[0];
                        model.comImage = "C00"+ NewId + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".") + 0);
                        var fileName = Path.GetFileName(postedFile.FileName);
                        fileName = (fileName.Substring(postedFile.FileName.LastIndexOf(".") + 0));
                        postedFile.SaveAs(Path.Combine(path, "C00" + NewId + fileName));
                }

                //Insert Data
                sqlQuery = "Insert Into tblCat_Company (comId, comName, ComCode, comAddress, comAddress2, comPhone, comFax, comEmail, comWeb, comType, comAlias, comFinYear, InvoicePrintSize, IsSerial, IsWarranty, ImgName) "
                               + " Values (" + NewId + ",'" + clsProc.softifyAvoidSingleQuote(model.comName) + "', '" + "C00" + NewId + "','" + clsProc.softifyAvoidSingleQuote(model.comAddress) + "', '" +
                               clsProc.softifyAvoidSingleQuote(model.comAddress2) + "','" + clsProc.softifyAvoidSingleQuote(model.comPhone) + "', '" + 
                               clsProc.softifyAvoidSingleQuote(model.comFax) + "','" + clsProc.softifyAvoidSingleQuote(model.comEmail) + "','" + 
                               clsProc.softifyAvoidSingleQuote(model.comWeb) + "','" + clsProc.softifyAvoidSingleQuote(model.comType) + "','" +
                               clsProc.softifyAvoidSingleQuote(model.comAlias) + "', '" + clsProc.softifyAvoidSingleQuote(model.comFinYear) + "', '" + clsProc.softifyAvoidSingleQuote(model.InvoicePrintSize) + "' , '"+Convert.ToInt16(model.IsSerial)+ "','" + Convert.ToInt16(model.IsWarranty) + "', '" + model.comImage + "')";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Company", "tblCat_Company"));
                //END : Transaction Log 


                for (int i = 0; i < model.CompanySubList.Count; i++)
                {
                    //comId, InvoiceType, Type, COM, Year, Number

                    int rowId = i + 1;

                    sqlQuery = "INSERT INTO  tblCat_Company_Sub (RowId,comId, VarId, Type, COM,Year,Number)" +
                                    " Values ('" + rowId + "','" + NewId + "','" + model.CompanySubList[i].VarId + "','" + model.CompanySubList[i].Type + "','" + model.CompanySubList[i].COM + "','"+ model.CompanySubList[i].Year + "','"+ model.CompanySubList[i].Number + "')";
                    arQuery.Add(sqlQuery);

                    arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "Company", "tblCat_Company_Sub"));
                }

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
            }
        } //end : prcDataSave

        //To Update 
        public string prcDataUpdate(Company model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            HttpPostedFileBase postedFile = null;
            try
            {
                var sqlQuery = "";
                string path = Server.MapPath("~/Content/assets/images/Company/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                /*  Image File Name and save*/
                if (Request.Files.Count > 0)
                {
                    postedFile = Request.Files[0];
                    model.comImage = "C00" + model.comId + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".") + 0);
                    var fileName = Path.GetFileName(postedFile.FileName);
                    fileName = (fileName.Substring(postedFile.FileName.LastIndexOf(".") + 0));
                    postedFile.SaveAs(Path.Combine(path, "C00" + model.comId + fileName));
                }

                sqlQuery = "Update tblCat_Company set comName='" + clsProc.softifyAvoidSingleQuote(model.comName) + "',comAddress='" + 
                          clsProc.softifyAvoidSingleQuote(model.comAddress) + "', comAddress2='" + clsProc.softifyAvoidSingleQuote(model.comAddress2) + "', comPhone='" + 
                          clsProc.softifyAvoidSingleQuote(model.comPhone) + "', comFax='" + clsProc.softifyAvoidSingleQuote(model.comFax) + "',comEmail='" + 
                          clsProc.softifyAvoidSingleQuote(model.comEmail) + "', comWeb='" + clsProc.softifyAvoidSingleQuote(model.comWeb) + "', comType='" + 
                          clsProc.softifyAvoidSingleQuote(model.comType) + "',comAlias = '" + clsProc.softifyAvoidSingleQuote(model.comAlias)+ "', comFinYear = '" + 
                          clsProc.softifyAvoidSingleQuote(model.comFinYear) + "' , InvoicePrintSize='"+model.InvoicePrintSize+ "', ImgName='" + model.comImage + "',IsSerial='" + Convert.ToInt16(model.IsSerial)+ "',IsWarranty='" + Convert.ToInt16(model.IsWarranty) + "' Where ComId= '" + model.comId + "' ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.comId, sqlQuery, "Update", "Company", "tblCat_Company"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
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
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            ArrayList arQuery = new ArrayList();
            var sqlQuery = "";
            try
            {
                sqlQuery = "Delete tblCat_Company Where ComId = " + ItemId + " ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "Company", "tblCat_Company"));
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