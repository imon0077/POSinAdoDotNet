using System;
using System.Collections;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using Microsoft.Reporting.WebForms;
using System.Linq;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class ProductShortageController : Controller
    {
        #region Common Data
        public int ComId = clsCommon.ComID;
        ArrayList _arQuery = new ArrayList();
        DataSet _dsCombo = new DataSet();
        DataSet _dsList = new DataSet();
        softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        string _sqlQuery = "";
        public static DataSet DsListEdit = new DataSet();
        #endregion Common Data

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        public string PrcLoadCombo()
        {
            try
            {
                _sqlQuery = $"EXEC PrcGetProduct_Shortage {ComId},  0  ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string GetItem(string value)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = $"Exec prcGetItem_Auto '{Session["ComId"]}', '{value}' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        public string PrcLoadList(string dtFrom, string dtTo)
        {
            try
            {
                _sqlQuery = $"EXEC PrcGetProduct_Shortage_List {ComId}, '{dtFrom}','{dtTo}'  ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string GetShortageById(int id)
        {
            try
            {
                _sqlQuery = $"EXEC [PrcGetProduct_Shortage] {ComId}, {id} ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductShortage model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var msg = PrcDataSave(model);
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
            ProductShortage model = new ProductShortage();
            model.ShortageId = id;
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(ProductShortage model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var msg = PrcDataUpdate(model);
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
        }

        #endregion End Action

        #region Procedure
        public string PrcDataSave(ProductShortage model)
          {
            try
            {
                _sqlQuery = $"SELECT Cast(isNull(MAX(ShortageId),0) + 1 AS float) AS ShortageId FROM tblSal_Product_Shortage_Main WHERE ComId = {ComId} ";
                double NewId = _clsCon.softifyCountingDataDouble(_sqlQuery);

                _sqlQuery = $"INSERT INTO tblSal_Product_Shortage_Main(ComId, ShortageId, WHId, dtShortage,  Remarks, LUserId, dtEntry) VALUES ({ComId}, {NewId}, '{model.WHId}',  '{model.dtShortage.ToString()}',  '{model.Remarks}', {Session["UserId"]},  GetDate() )";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "ProductShortageController", "tblSal_Product_Shortage_Main"));
                /* END : Transaction Log */

                if (model.ShortageSubList != null && model.ShortageSubList.Count > 0)
                {
                    foreach (var item in model.ShortageSubList)
                    {
                        if (item.Qty > 0)
                        {
                            _sqlQuery = $"INSERT INTO tblSal_Product_Shortage_Sub (ComId,  WhId, ShortageId, ProductId, Qty, Rate, Amount) Values ({ComId},{model.WHId}, {NewId}, {item.ProductId}, {item.Qty}, {item.Rate},{item.Amount} )";
                            _arQuery.Add(_sqlQuery);

                            /* START : Transaction Log */
                            _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "ProductShortageController", "tblSal_Product_Shortage_Sub"));
                            /* END : Transaction Log */
                        }
                    }
                }

                ////Update stock 
                //_sqlQuery = $"Exec [prcStockUpdateBy_Shortage] {ComId},{NewId}, 0 ";
                //_arQuery.Add(_sqlQuery);

                ///* START : Transaction Log */
                //_arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "StockUpdateShortage", "ProductShortageController", "tblSal_Product_Stock"));
                ///* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data saved successfully";
            }


            catch (Exception ex)
            {
                return "Error! Ex:" + ex.Message;
            }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDataUpdate(ProductShortage model)
        {
            try
            {
                ////Adjust stock 
                //_sqlQuery = $"Exec [prcStockUpdateBy_Shortage] {ComId},  {model.ShortageId}, 1 ";
                //_arQuery.Add(_sqlQuery);

                _sqlQuery = $"Delete tblSal_Product_Shortage_Sub Where ComId ={ComId} And  ShortageId = {model.ShortageId} ";
                _arQuery.Add(_sqlQuery);

                //  Update: Main Table
                _sqlQuery = $"Update tblSal_Product_Shortage_Main Set dtShortage='{model.dtShortage.ToString()}', TypeId= '{model.TypeId}', Remarks= '{model.Remarks}', UpdateById= {Session["UserId"]}, dtUpdate= GetDate() Where ComId = {ComId} And ShortageId= {model.ShortageId} ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(model.ShortageId, _sqlQuery, "Update", "ProductShortageContrller", "tblSal_Product_Shortage_Main"));
                /* END : Transaction Log */

                if (model.ShortageSubList != null && model.ShortageSubList.Count > 0)
                {
                    foreach (var item in model.ShortageSubList)
                    {
                        if (item.Qty > 0)
                        {
                            _sqlQuery = $"INSERT INTO tblSal_Product_Shortage_Sub (ComId, BranchId, WhId, ShortageId, ProductId, Qty, Rate, Amount) Values ({ComId}, {Session["BranchId"]},{model.WHId}, {model.ShortageId}, {item.ProductId}, {item.Qty}, {item.Rate},{item.Amount} )";
                            _arQuery.Add(_sqlQuery);

                            /* START : Transaction Log */
                            _arQuery.Add(clsCommon.TransLogInsert(model.ShortageId, _sqlQuery, "Insert", "ProductShortageContrller", "tblSal_Product_Shortage_Sub"));
                            /* END : Transaction Log */
                        }
                    }
                }

                //////Update stock 
                //_sqlQuery = $"Exec [prcStockUpdateBy_Shortage] {ComId},{model.ShortageId}, 0 ";
                //_arQuery.Add(_sqlQuery);

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data updated successfully";
            }

            catch (Exception ex)
            {return ex.Message.ToString();}
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDeleteData(Int64 ItemId)
        {
            try
            {
                ////Adjust stock 
                //_sqlQuery = $"Exec [prcStockUpdateBy_Shortage] {ComId},{ItemId}, 1 ";
                //_arQuery.Add(_sqlQuery);

                _sqlQuery = $"DELETE tblSal_Product_Shortage_Main WHERE ComId = {ComId} And ShortageId = {ItemId}  ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "Delete", "ProductShortageContrller", "tblSal_Product_Shortage_Main"));
                /* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data Deleted Successfully.";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
        #endregion

        #region Damage Report
        public ActionResult RptDamage(string id, string rptFormat = "pdf")
        {
            SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);

            DataSet rptDS = new DataSet();
            string DataSourceName = "DataSet1";
            string ReportCaption = "Damage Report";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptFormat == "Excel") ? "xlsx" : (rptFormat == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            if (string.IsNullOrEmpty(id.ToString()))
            {
                id = "0";
            }

            try
            {
                string rptQuery = "";
                rptQuery = $"EXEC rptShortage {ComId},  {id}  ";
                localReport.ReportPath = Server.MapPath("~/Report/rptProduct_Shortage.rdlc");

                _clsCon.softifyFillDatasetUsingSQLCommand(ref rptDS, rptQuery);

                reportDataSource.Value = rptDS.Tables[0];
                if (rptDS.Tables[0].Rows.Count == 0)
                {
                    ModelState.AddModelError("CustomError", "Data Not Found.....");
                }
                else
                {
                    localReport.DataSources.Add(reportDataSource);
                    localReport.EnableExternalImages = true;
                    renderedBytes = localReport.Render(rptFormat, "", out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    Response.AddHeader("Content-Disposition", $"inline; filename = {ReportCaption}.{fileNameExtension}");
                    Response.ContentType = Models.Report.ReturnExtension("." + fileNameExtension.ToLower());
                    Response.AddHeader("content-length", renderedBytes.Length.ToString());
                    Response.BinaryWrite(renderedBytes);
                    return File(renderedBytes, mimeType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _clsCon = null;
            }
            return null;
            
        }
        #endregion
    }

}