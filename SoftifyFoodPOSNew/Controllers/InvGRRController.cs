using Softify;
using SoftifyFoodPOSNew.Models;
using System;
using System.Collections;
using System.Data;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using SoftifyFoodPOSNew.CustomeFilter;
using System.Linq;
using System.Web;
using System.IO;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class InvGRRController : Controller
    {
        #region CommonData
        DataSet _dsList = new DataSet();
        string _sqlQuery = "";
        ArrayList _arQuery = new ArrayList();
        softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        #endregion

        public string prcLoadCombo()
        {
            try
            {
                _sqlQuery = "Exec prcGetInv_GRR " + Session["ComId"] + ", 0";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }
        }

        public string GetItemAutocomplete(string Flag)
        {

            try
            {
                _sqlQuery = $"Exec prcGetInv_ItemAutocomplete {Session["ComId"]}, '{Flag}' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }
        }

        public ActionResult Index()
        {
            return View();
        }

        public string GetGRRList(string dtFrom, string dtTo)
        {

            try
            {
                if (string.IsNullOrEmpty(dtFrom))
                {
                    dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                if (string.IsNullOrEmpty(dtTo))
                {
                    dtTo = DateTime.Now.ToString("dd-MMM-yyyy");
                }

                _sqlQuery = "Exec prcGetInv_GRRList " + Session["ComId"] + ", " + Session["UserId"] + ",0, '" + dtFrom + "','" + dtTo + "' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(InvGRR model)
        {

            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = prcDataSave(model);
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

        public ActionResult BarcodeItem(Int64 Id)
        {
            InvGRR model = new InvGRR();
            model.GrrId = Id;
            return View(model);
        }

        [HttpPost]
        public ActionResult BarcodeItem(InvGRR model)
        {

            try
            {
                if (model.InvGRRSubList[0].GrrId > 0 && model.InvGRRSubList[0].QtyRcv > 0)
                {
                    //Clear existing data
                    _sqlQuery = $"Delete TempBarcodePrint Where ComId = '{Session["ComId"]}' ";
                    _arQuery.Add(_sqlQuery);

                    //foreach(var item in model.InvGRRSubList){
                    for (int i = 0; i < model.InvGRRSubList.Count; i++)
                    {
                        for (int j = 0; j < model.InvGRRSubList[i].QtyRcv; j++)
                        {
                            // if j =10
                            //loop will run 10 times
                            _sqlQuery = $"Insert into TempBarcodePrint(ComId, ProductId, GrrId) values( '{Session["ComId"]}', {model.InvGRRSubList[i].ProductId}, {model.InvGRRSubList[i].GrrId} ) ";
                            _arQuery.Add(_sqlQuery);
                        }

                    }

                }

                //Transaction with database
                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }

        }

        public string GetGRRBarcode(int id)
        {

            try
            {
                _sqlQuery = "Exec prcGetInv_BarcodeList " + Session["ComId"] + ", " + id + " ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }
        }

        public ActionResult RptBarcode(string rptType = "pdf")
        {
            DataSet rptDS = new DataSet();
            string ReportCaption = "Barcode Print";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptType == "Excel") ? "xlsx" : (rptType == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string rptQuery = "";

                localReport.ReportPath = Server.MapPath("~/Report/rptBarcodePrint.rdlc");
                rptQuery = $"Exec rptBarCodePrint {Session["ComId"]} ";

                clsCon.softifyFillDatasetUsingSQLCommand(ref rptDS, rptQuery);
                reportDataSource.Value = rptDS.Tables[0];
                if (rptDS.Tables[0].Rows.Count == 0)
                {
                    ModelState.AddModelError("CustomError", "Data Not Found.....");
                }
                else
                {
                    localReport.DataSources.Add(reportDataSource);
                    localReport.EnableExternalImages = true;
                    renderedBytes = localReport.Render(rptType, "", out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
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

            return null;

        }

        public ActionResult Edit(Int64 id)
        {
            InvGRR model = new InvGRR();
            model.GrrId = id;
            return View(model);
        }

        public string prcGetGRRById(Int64 id)
        {
            try
            {
                _sqlQuery = $"Exec prcGetInv_GRR {Session["ComId"]}, {id} ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }
        }

        [HttpPost]
        public ActionResult Edit(InvGRR model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = prcDataUpdate(model);
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

        #region Start procedure
        public string prcDataSave(InvGRR model)
        {
            try
            {

                // Check duplicate item
                #region duplicate data checking
                //if (model.InvGRRSubList != null && model.InvGRRSubList.Count > 0)
                //{
                //    for (int i = 0; i < model.InvGRRSubList.Count; i++)
                //    {
                //        for (int j = i + 1; j < model.InvGRRSubList.Count; j++)
                //        {
                //            if (model.InvGRRSubList[i].ModelNo.ToString() == model.InvGRRSubList[j].ModelNo.ToString())
                //            {
                //                return "Sorry!! duplicate product selected. try with different item.";
                //            }
                //        }
                //    }
                //}
                #endregion duplicate data checking

                _sqlQuery = $"Select Cast(isNull(Max(GrrId),0)+1 As float) AS NewId from tblInv_Grr_Main ";
                double NewId = _clsCon.softifyCountingDataDouble(_sqlQuery);

                _sqlQuery = $"Insert Into tblInv_Grr_Main (ComId, GrrId, GrrNo, dtGrr, SupplierId, ChallanNo, LCId, Receiver, PayMode, HeadId,dtCheque, ChequeNo, RecvPayId, TotalAmount, DueAmount, PaidAmount, Remarks, WHId, PCName, LUserId, dtEntry) Values ({Session["ComId"]}, {NewId}, dbo.fncInvNewNo( { clsCommon.ComID }, 'GRR', right(DATEPART(yy, '{model.dtGrr}' ),2), {model.SupplierId}), '{_clsProc.softifyDateFormat(model.dtGrr.ToString())}', {model.SupplierId}, '{model.ChallanNo}', '{model.LCId}', '{_clsProc.softifyAvoidSingleQuote(model.ReceivedBy)}', '{model.PayMode}', {model.HeadId}, '{model.dtCheque}', '{model.ChequeNo}', {0}, {model.TotalAmount}, {model.DueAmount}, {model.PaidAmount}, '{_clsProc.softifyAvoidSingleQuote(model.Notes)}', {model.WHId}, '{_clsProc.softifyPCName()}', {Session["UserId"]}, GetDate() )";
                _arQuery.Add(_sqlQuery);

                //START : Transaction Log 
                _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "InvGRR", "tblInv_Grr_Main"));
                //END : Transaction Log 

                if (model.InvGRRSubList != null && model.InvGRRSubList.Count > 0)
                {

                    foreach (var item in model.InvGRRSubList)
                    {
                        if (item.QtyRcv > 0 && item.Price > 0)
                        {

                            //Insert details data
                            _sqlQuery = $"Insert Into tblInv_Grr_Sub(ComId, GrrId, ProductId, QtyRcvd, BalanceQty, UnitPrice, Amount, SellingPrice, dtValidate, Photo, RowNo, Warranty) Values ({Session["ComId"]}, '{NewId}', {item.ProductId},  '{_clsProc.softifyConvertDouble(item.QtyRcv.ToString())}', '{_clsProc.softifyConvertDouble(item.QtyRcv.ToString())}', {_clsProc.softifyConvertDouble(item.Price.ToString())}, '{_clsProc.softifyConvertDouble(item.Amount.ToString())}', '{_clsProc.softifyConvertDouble(item.SellingPrice.ToString())}', '{item.dtValidate}', '', '{item.RowNo}', '{item.Warranty}') ";
                            _arQuery.Add(_sqlQuery);

                            //Stock update
                            _sqlQuery = $"Exec prcStockUpdateByGRR {Session["ComId"]}, {item.ProductId}, {item.QtyRcv} ";
                            _arQuery.Add(_sqlQuery);

                            // Set Product Selling Price
                            _sqlQuery = $"Update tblSal_Product Set SellingPrice = {_clsProc.softifyConvertDouble(item.SellingPrice.ToString())}, LastGrrPrice = {_clsProc.softifyConvertDouble(item.Price.ToString())} Where ComId = {clsCommon.ComID} And ProductId = {item.ProductId} ";
                            _arQuery.Add(_sqlQuery);

                            //START : Transaction Log 
                            _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "SellingPrice", "InvGRR", "tblSal_Product"));
                            //END : Transaction Log }
                        }
                        else
                        {
                            return "Sorry!! invalid GRN qty or unit price, Row No is : "+item.RowNo.ToString();
                        }

                    }
                }


                //Transaction with database
                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "1";

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }
        }

        public string prcDataUpdate(InvGRR model)
        {
            try
            {
                //Stock minus
                _sqlQuery = $"Exec prcStockUpdateByGRR {Session["ComId"]}, 0, 0, {model.GrrId} ";
                _arQuery.Add(_sqlQuery);

                _sqlQuery = $"Delete tblInv_Grr_Sub Where ComId = {Session["ComId"]} And GrrId = {model.GrrId} ";
                _arQuery.Add(_sqlQuery);
                //End Delete existing


                // Update GRR Main table
                _sqlQuery = $"Update tblInv_Grr_Main Set dtGrr = '{model.dtGrr}', SupplierId = {model.SupplierId}, ChallanNo = '{model.ChallanNo}', LCId = '{model.LCId}', Receiver = '{model.ReceivedBy}', PayMode='{model.PayMode}', HeadId={model.HeadId}, ChequeNo='{model.ChequeNo}', dtCheque='{model.dtCheque}', TotalAmount = {model.TotalAmount}, Remarks = '{model.Notes}', WHId = {model.WHId},  RecvPayId = '{0}', UpdatedById = {Session["ComId"]}, dtUpdate = GetDate() Where ComId = {Session["ComId"]} And GrrId = {model.GrrId} ";
                _arQuery.Add(_sqlQuery);

                //START : Transaction Log 
                _arQuery.Add(clsCommon.TransLogInsert(model.GrrId, _sqlQuery, "Update", "InvGRR", "tblInv_Grr_Main"));
                //END : Transaction Log 

                //Insert Data                           
                if (model.InvGRRSubList != null && model.InvGRRSubList.Count > 0)
                {
                    foreach (var item in model.InvGRRSubList)
                    {
                        //Insert details data
                        _sqlQuery = $"Insert Into tblInv_Grr_Sub(ComId, GrrId, ProductId,  WeightId, QtyRcvd, BalanceQty, UnitPrice, Amount, SellingPrice, RowNo, Warranty) Values ({Session["ComId"]}, '{model.GrrId}', {item.ProductId},  {item.WeightId}, '{_clsProc.softifyConvertDouble(item.QtyRcv.ToString())}', '{_clsProc.softifyConvertDouble(item.QtyRcv.ToString())}', {_clsProc.softifyConvertDouble(item.Price.ToString())}, '{_clsProc.softifyConvertDouble(item.Amount.ToString())}', '{_clsProc.softifyConvertDouble(item.SellingPrice.ToString())}', '{item.RowNo}', '{item.Warranty}') ";
                        _arQuery.Add(_sqlQuery);

                        //Stock update
                        _sqlQuery = $"Exec prcStockUpdateByGRR {clsCommon.ComID}, {item.ProductId}, {item.QtyRcv} ";
                        _arQuery.Add(_sqlQuery);

                        // Set Product Selling Price
                        _sqlQuery = $"Update tblSal_Product Set SellingPrice = {_clsProc.softifyConvertDouble(item.SellingPrice.ToString())}, LastGrrPrice = {_clsProc.softifyConvertDouble(item.Price.ToString())} Where ComId = {clsCommon.ComID} And ProductId = {item.ProductId} ";
                        _arQuery.Add(_sqlQuery);

                        //START : Transaction Log 
                        _arQuery.Add(clsCommon.TransLogInsert(model.GrrId, _sqlQuery, "SellingPrice", "InvGRR", "tblSal_Product"));
                        //END : Transaction Log 
                    }
                }

                //Transaction with database
                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }

        } //end : prcUpdateData

        public string prcDeleteData(int ItemId, int SupplierId)
        {

            try
            {
                //Stock minus
                _sqlQuery = $"Exec prcStockUpdateByGRR {Session["ComId"]}, 0, 0, {ItemId} ";
                _arQuery.Add(_sqlQuery);

                //START : Transaction Log 
                _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "StockUpdateGRR", "InvGRR", "tblSal_Product"));
                //END : Transaction Log 

                _sqlQuery = "Delete tblInv_Grr_Main Where ComId = " + Session["ComId"] + " And GrrId = " + ItemId + " ";
                _arQuery.Add(_sqlQuery);

                //START : Transaction Log 
                _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "Delete", "InvGRR", "tblInv_Grr_Main"));
                //END : Transaction Log 

                //Transaction with database
                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally { prcSetObjectNull(); _dsList.Dispose(); }

        }

        public ActionResult RptGrr(string id, string rptFormat = "pdf")
        {
            SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);

            DataSet rptDS = new DataSet();
            string DataSourceName = "DataSet1";
            string ReportCaption = "GRR Report";
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

                rptQuery = "EXEC rptGRR " + Session["ComId"] + ", " + id + "  ";
                localReport.ReportPath = Server.MapPath("~/Report/rptGRR.rdlc");
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

        public ActionResult GrrStatement()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GrrStatement(string dtFrom = "", string dtTo = "", int SupplierId = 0, string rptFormat = "pdf")
        {
            SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
            DataSet rptDS = new DataSet();
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };
            string ReportCaption = "Sales Statement Report";

            string mimeType, encoding, fileNameExtension = (rptFormat == "Excel") ? "xlsx" : (rptFormat == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            string rptQuery = "";
            try
            {
                rptQuery = "EXEC rptSales_Statement_Details " + Session["ComId"] + ", '" + dtFrom + "', '" + dtTo + "' ";
                localReport.ReportPath = Server.MapPath("~/Report/rptSales_Statement_Details.rdlc");
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


        private void prcSetObjectNull()
        {
            _clsCon = null;
            _clsProc = null;
            _arQuery = null;
            _sqlQuery = null;
        }

        #endregion Procedure

    }
}