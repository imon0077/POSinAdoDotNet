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
    public class POSController : Controller
    {
        #region Common Data
        public int ComId = clsCommon.ComID;
        ArrayList _arQuery = new ArrayList();
        DataSet _dsCombo = new DataSet();
        DataSet _dsList = new DataSet();
        softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        string _sqlQuery = ""; public static DataSet DsListEdit = new DataSet();
        #endregion Common Data

        public string PrcLoadCombo()
        {
            try
            {
                _sqlQuery = "EXEC [prcGetSales_POS] '" + ComId + "', 0 ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsCombo, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsCombo);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        public string PrcLoadList(string dtFrom, string dtTo)
        {
            try
            {
                _sqlQuery = $"EXEC [prcGetSales_POS_List] {ComId}, 0, '{dtFrom}', '{dtTo}', '{Session["UserId"]}' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string GetItemAutocomplete(string Flag, string Flag2="")
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = $"Exec prcGetInv_ItemSalesAuto  {ComId}, N'{Flag}', '{Flag2}' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);

                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { throw (ex); }
            finally { sqlQuery = null; clsCon = null; dsList.Dispose(); }
        }

        public string GetLastPriceByGrr(int GrrId = 0)
        {
            DataSet dsData = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                if (GrrId == 0)
                    return "Error: Item Id Not found ";

                string sqlQuery = $"Exec [prcGetInv_GrrLastPrice] {ComId}, {GrrId} ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsData, sqlQuery);
                DataTable dtData = (DataTable)dsData.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return "Error!" + ex.Message;
            }
            finally
            {
                clsCon = null;
                dsData = null;
            }
        }

        public string GetPrevDues(int id = 0)
        {
            DataSet dsData = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                if (id == 0)
                    return "Error: Id Not found ";

                string sqlQuery = $"Select dbo.fncGet_PrevDues(0, {ComId}, {id})";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsData, sqlQuery);
                DataTable dtData = (DataTable)dsData.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return "Error!" + ex.Message;
            }
            finally
            {
                clsCon = null;
                dsData = null;
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(POS model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.NetPayable > 0)
                    {
                        var msg = PrcDataSave(model);
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Please, provide amount", JsonRequestBehavior.AllowGet);
                    }
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

        public ActionResult Edit(Int64 id)
        {
            POS model = new POS();
            model.InvoiceId = id;
            return View(model);
        }

        public string GetInvoiceById(Int64 id)
        {
            try
            {
                _sqlQuery = $"EXEC [prcGetSales_POS] {ComId}, {id} ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        [HttpPost]
        public ActionResult Edit(POS model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.NetPayable > 0)
                    {
                        msg = PrcDataUpdate(model);
                        if (msg.Contains("updated"))
                        {
                            return Json(1, JsonRequestBehavior.AllowGet);
                        }
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Please, provide amount", JsonRequestBehavior.AllowGet);
                    }
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
        public string[] PrcDataSave(POS model)
        {
            try
            {
                _sqlQuery = "SELECT Cast(isNull(MAX(InvoiceId),0) + 1 AS float) AS InvoiceId FROM tblSal_Invoice_Main WHERE ComId = '" + ComId + "' ";
                double NewId = _clsCon.softifyCountingDataDouble(_sqlQuery);
                string InvoiceNo = "";

                _sqlQuery = $"SELECT Cast(isNull(MAX(TransId),0) + 1 AS float) AS TransId FROM tblStr_TransProcess WHERE ComId = {ComId} ";
                double TransId = _clsCon.softifyCountingDataDouble(_sqlQuery);

                _sqlQuery = "INSERT INTO tblSal_Invoice_Main(ComId, InvoiceId, InvoiceNo, dtInvoice, ClientId, WhId, TotalAmount, Discount,Percentage, Tax, Shipping, NetPayable, Paid, Due, PcName, UserId, dtEntry) VALUES ('" + ComId + "', '" + NewId + "', dbo.fncInvNewNo( " + ComId + ", 'Invoice', right(DATEPART(yy,'" + model.dtInvoice.ToString() + "'),2), " + NewId + "), '" + model.dtInvoice.ToString() + "', '" + model.ClientId + "','" + model.WHId + "', '" + model.TotalAmount + "', '" + model.Discount + "', '" + model.Percentage + "','" + model.Tax + "', '" + model.Shipping + "', '" + model.NetPayable + "', '" + model.Paid + "', '" + model.Due + "', '" + _clsProc.softifyPCName() + "', " + Session["UserId"] + ", GetDate() )";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "POSContrller", "tblSal_Invoice_Main"));
                /* END : Transaction Log */

                // Update Customer Balance
                _sqlQuery = $"Update tblClient_Information Set ClosingBalance = ClosingBalance + {model.Due} Where ComId = {ComId} And ClientId = {model.ClientId } ";
                _arQuery.Add(_sqlQuery);

                _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "ClosingBalance", "POSContrller", "tblClient_Information"));

                #region InvoiceSub
                if (model.InvoiceSubList != null && model.InvoiceSubList.Count > 0)
                {
                    for (int i = 0; i < model.InvoiceSubList.Count; i++)
                    {
                        _sqlQuery = "INSERT INTO tblSal_Invoice_Sub (ComId, InvoiceId, ProductId, ProductSubId, PrdModel, GrrId, ColorId, Warranty, Qty, SellingPrice, PrdDiscount, Amount, RowNo, WHId) Values (" + ComId + ", " + NewId + ", '" + model.InvoiceSubList[i].ProductId + "', 1, '" + model.InvoiceSubList[i].PrdModel + "', '" + model.InvoiceSubList[i].GrrId + "', 1, '" + model.InvoiceSubList[i].warranty + "','" + model.InvoiceSubList[i].Qty + "', '" + model.InvoiceSubList[i].SellingPrice + "', '" + model.InvoiceSubList[i].PrdDiscount + "', '" + model.InvoiceSubList[i].Amount + "', '" + model.InvoiceSubList[i].RowNo + "',  '" + model.InvoiceSubList[i].WHId + "')";
                        _arQuery.Add(_sqlQuery);

                        /* START : Transaction Log */
                        _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "POSContrller", "tblSal_Invoice_Sub"));
                        /* END : Transaction Log */

                        ////Update GRR Stock Balance 
                        //_sqlQuery = $" Update tblInv_Grr_Sub SET BalanceQty = BalanceQty - '{model.InvoiceSubList[i].Qty}' Where GrrId = '{model.InvoiceSubList[i].GrrId}' AND ProductId = '{model.InvoiceSubList[i].ProductId}'";
                        //_arQuery.Add(_sqlQuery);
                        
                        ///* START : Transaction Log */
                        //_arQuery.Add(clsCommon.TransLogInsert(model.InvoiceSubList[i].GrrId, _sqlQuery, "Update", "POSContrller", "tblInv_Grr_Sub"));
                        ///* END : Transaction Log */

                   
                    }
                }
                #endregion InvoiceSub

                //Update stock 
                _sqlQuery = $"Exec prcStockUpdateBySales {ComId}, {NewId},0  ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "StockUpdateSales", "POSContrller", "tblSal_Product"));
                /* END : Transaction Log */


                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                _sqlQuery = "select InvoiceNo from tblSal_Invoice_Main where InvoiceId = '" + NewId + "'";
                InvoiceNo = _clsCon.softifyGetStringData(_sqlQuery);


                #region collection 
                if(model.Paid > 0)
                {
                    ArrayList _arQuery = new ArrayList();
                    string sqlQuery = $"Select dbo.fncInvNewNo({ComId}, 'Collection', '', 0)";
                    string CRNo = _clsCon.softifyGetStringData(sqlQuery);

                    sqlQuery = "Select Cast(IsNull(MAX(CollectionId),0) + 1 as float)  As NewId From tblSales_Collection";
                    double CollectionId = _clsCon.softifyCountingDataDouble(sqlQuery);
                    
                    sqlQuery = $"INSERT INTO tblSales_Collection(ComId, CollectionId, dtCollection, CollectionNo, ClientId, InvoiceId, PayMode, BankId, Amount, Discount, isCleared, isPosted, Remarks, dtEntry, LuserId)  Values({ComId}, {CollectionId}, '{model.dtInvoice}', '{CRNo}', {model.ClientId}, {NewId}, 'Cash', '1', {model.Paid}, 0, 0, 0, 'Collection during invoice' , GetDate(), {Session["UserId"]} )";
                    _arQuery.Add(sqlQuery);

                    //START : Transaction Log 
                    _arQuery.Add(clsCommon.TransLogInsert(CollectionId, sqlQuery, "Insert", "SalesCollection", "tblSales_Collection"));
                    //END : Transaction Log 

                    _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                }
                #endregion collection

                var result = new string[] { InvoiceNo, NewId.ToString() };
                //Send SMS
                //if (InvoiceNo != "")
                //{
                //    SMSHelperController SmsObj = new SMSHelperController();
                //    SmsObj.PrcSmsProcess(NewId, "Invoice");
                //}
                return result;

            }

            
            catch (Exception ex) { return new string[] { "Error! Ex:" + ex.Message, "" }; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDataUpdate(POS model)
        {
            try
            {
                //Adjust stock 
                _sqlQuery = $"Exec prcStockUpdateBySales {ComId}, {model.InvoiceId}, 1  ";
                _arQuery.Add(_sqlQuery);

                // Update Customer Balance
                _sqlQuery = $"Update A Set ClosingBalance = ClosingBalance - B.Due " + 
                            $"FROM tblClient_Information A "+ 
                            $"INNER JOIN tblSal_Invoice_Main B ON B.ClientId = A.ClientId " +  
                            $"Where A.ComId = {ComId} And B.InvoiceId = {model.InvoiceId } ";
                _arQuery.Add(_sqlQuery);

                _sqlQuery = $"Delete tblSal_Invoice_Sub Where ComId = {ComId} And InvoiceId  = {model.InvoiceId} ";
                _arQuery.Add(_sqlQuery);

                //  Update: Main Table
                _sqlQuery = $"Update tblSal_Invoice_Main Set dtInvoice = '{model.dtInvoice.ToString()}', ClientId = {model.ClientId},  WHId = {model.WHId}, TotalAmount = {model.TotalAmount}, Discount= {model.Discount}, Percentage = {model.Percentage}, Tax = {model.Tax}, Shipping = {model.Shipping}, NetPayable = {model.NetPayable}, Paid = {model.Paid}, Due = {model.Due}, UpdatedById = {Session["UserId"]}, dtUpdate = GetDate() Where ComId = {ComId} And InvoiceId = {model.InvoiceId} ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(model.InvoiceId, _sqlQuery, "Update", "POSContrller", "tblSal_Invoice_Main"));
                /* END : Transaction Log */

                #region Invoice Sub
                if (model.InvoiceSubList != null && model.InvoiceSubList.Count > 0)
                {
                    for (int i = 0; i < model.InvoiceSubList.Count; i++)
                    {
                        _sqlQuery = "INSERT INTO tblSal_Invoice_Sub (ComId, InvoiceId, ProductId, ProductSubId,  GrrId, ColorId, Warranty, Qty, SellingPrice, PrdDiscount, Amount, RowNo, WHId) Values (" + ComId + ", " + model.InvoiceId + ", '" + model.InvoiceSubList[i].ProductId + "', 1,  '" + model.InvoiceSubList[i].GrrId + "', 1, '" + model.InvoiceSubList[i].warranty + "', '" + model.InvoiceSubList[i].Qty + "', '" + model.InvoiceSubList[i].SellingPrice + "', '" + model.InvoiceSubList[i].PrdDiscount + "', '" + model.InvoiceSubList[i].Amount + "', '" + model.InvoiceSubList[i].RowNo + "',  '" + model.InvoiceSubList[i].WHId + "')";
                        _arQuery.Add(_sqlQuery);

                        /* START : Transaction Log */
                        _arQuery.Add(clsCommon.TransLogInsert(model.InvoiceId, _sqlQuery, "Insert", "POSContrller", "tblSal_Invoice_Sub"));
                        /* END : Transaction Log */
                        
                        #region serial data
                        if (model.InvoiceSerialList != null && model.InvoiceSerialList.Count > 0)
                        {
                            for (int j = 0; j < model.InvoiceSerialList.Count; j++)
                            {
                                if (model.InvoiceSerialList[j].ProductId == model.InvoiceSubList[i].ProductId)
                                {
                                    _sqlQuery = $"Update tblSal_Product_Serial Set IsSell = 1, InvoiceId = {model.InvoiceId} Where ComId = {ComId} And ProductId = {model.InvoiceSerialList[j].ProductId } And SerialId = {model.InvoiceSerialList[j].SerialId} ";
                                    _arQuery.Add(_sqlQuery);

                                    _arQuery.Add(clsCommon.TransLogInsert(model.InvoiceId, _sqlQuery, "SerialUpdateSales", "POSContrller", "tblSal_Product_Serial"));
                                }

                            }
                        }
                        #endregion serial data
                    }
                }
                #endregion Invoice Sub

                // Update: Collection Table
                _sqlQuery = $"Update tblSales_Collection Set dtCollection = '{model.dtInvoice.ToString()}', ClientId = {model.ClientId}, Amount = {model.Paid} Where ComId = {ComId} And InvoiceId = {model.InvoiceId} And Remarks = 'Collection during invoice' ";
                _arQuery.Add(_sqlQuery);
                
                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(model.InvoiceId, _sqlQuery, "Update", "POSContrller", "tblSales_Collection"));
                /* END : Transaction Log */

                // Update Customer Balance
                _sqlQuery = $"Update tblClient_Information Set ClosingBalance = ClosingBalance + {model.Due} Where ComId = {ComId} And ClientId = {model.ClientId } ";
                _arQuery.Add(_sqlQuery);

                _arQuery.Add(clsCommon.TransLogInsert(model.InvoiceId, _sqlQuery, "ClosingBalance", "POSContrller", "tblClient_Information"));

                //Update stock 
                _sqlQuery = $"Exec prcStockUpdateBySales {ComId}, {model.InvoiceId}, 0  ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(model.InvoiceId, _sqlQuery, "StockUpdateSales", "POSContrller", "tblSal_Product"));
                /* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDeleteData(Int64 ItemId, int ClientId, double NetPayable)
        {
            try
            {
                if (NetPayable > 0 && ClientId > 0) { 
                    // Update Customer Balance
                    _sqlQuery = $"Update tblClient_Information Set ClosingBalance = ClosingBalance - {NetPayable} Where ComId = {ComId} And ClientId = {ClientId } ";
                    _arQuery.Add(_sqlQuery);

                    //Update stock 
                    _sqlQuery = $"Exec prcStockUpdateBySales {ComId}, {ItemId}, 1 ";
                    _arQuery.Add(_sqlQuery);

                    _sqlQuery = $"DELETE FROM tblSal_Invoice_Main WHERE ComId = {ComId} And InvoiceId = {ItemId}  ";
                    _arQuery.Add(_sqlQuery);

                    /* START : Transaction Log */
                    _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "Delete", "POSContrller", "tblSal_Invoice_Main"));
                    /* END : Transaction Log */

                    _sqlQuery = $"DELETE FROM tblSales_Collection WHERE ComId = {ComId} And InvoiceId = {ItemId}  ";
                    _arQuery.Add(_sqlQuery);

                    /* START : Transaction Log */
                    _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "Delete", "POSContrller", "tblSales_Collection"));
                    /* END : Transaction Log */

                    _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                    return "Data Deleted Successfully.";
                }
                else
                    return "Invalid data";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
        #endregion

        #region Invoice Report
        public ActionResult RptSalesInvoice(string id, string rptFormat = "pdf")
        {
            DataSet rptDS = new DataSet();
            string InvoicePrintSize = "POS";
            string ReportCaption = "Sales Invoice Report";
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
                InvoicePrintSize = Session["InvoicePrintSize"].ToString();
                if (InvoicePrintSize.ToString().ToUpper() == "POS".ToString().ToUpper())
                {
                    rptQuery = $"EXEC rptInvoice {ComId}, {id}  ";
                    localReport.ReportPath = Server.MapPath("~/Report/rptInvoicePOS.rdlc");
                }
                else
                {
                    rptQuery = $"EXEC rptInvoice {ComId}, {id} ";
                    localReport.ReportPath = Server.MapPath("~/Report/rptInvoiceA4.rdlc");
                }

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
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                _clsCon = null;
            }
            return null;
        }
        #endregion

        #region Invoice Statement
        public ActionResult RptSalesStatement()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RptSalesStatement(string dtFrom, string dtTo, string flag, string rptFormat = "pdf")
        {
            DataSet rptDS = new DataSet();
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };
            string ReportCaption = "Sales Statement Report";

            string mimeType, encoding, fileNameExtension = (rptFormat == "Excel") ? "xlsx" : (rptFormat == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            try
            {
                string rptQuery = "";

                if (flag == "summary")
                {
                    rptQuery = "EXEC rptSales_Statement_Summary " + Session["ComId"] + ", '" + dtFrom + "', '" + dtTo + "'  ";
                    localReport.ReportPath = Server.MapPath("~/Report/rptSalesStatement_Summary.rdlc");
                }
                else
                {
                    rptQuery = "EXEC rptSales_Statement_Details " + Session["ComId"] + ", '" + dtFrom + "', '" + dtTo + "' ";
                    localReport.ReportPath = Server.MapPath("~/Report/rptSales_Statement_Details.rdlc");
                }

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
        public ActionResult RptSalesStatementInvoice()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RptSalesStatementInvoice(string InvoiceFrom, string InvoiceTo, string flag, string rptFormat = "pdf")
        {
            DataSet rptDS = new DataSet();
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };
            string ReportCaption = "Sales Statement Report Invoice";

            string mimeType, encoding, fileNameExtension = (rptFormat == "Excel") ? "xlsx" : (rptFormat == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            try
            {
                string rptQuery = "";

                if (flag == "summary")
                {
                    rptQuery = "EXEC rptSales_Statement_Summary " + Session["ComId"] + ", '" + InvoiceFrom + "', '" + InvoiceTo + "'  ";
                    localReport.ReportPath = Server.MapPath("~/Report/rptSalesStatement_Summary.rdlc");
                }
                else
                {
                    rptQuery = "EXEC rptSales_Statement_Details_Invoice " + Session["ComId"] + ", '" + InvoiceFrom + "', '" + InvoiceTo + "' ";
                    localReport.ReportPath = Server.MapPath("~/Report/rptSales_Statement_Details.rdlc");
                }

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

        public ActionResult RptCashBankClosing()
        {
            return View();
        }

        //$scope.model = {}

        //$scope.model.GateExpensesSubList = data.data.Table;

            //submit
            //$scope.model.dtFrom = $("#dtFrom").val();

        [HttpPost]
        public ActionResult RptCashBankClosing(Expenses model, string rptFormat = "pdf")
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);

            DataSet rptDS = new DataSet();
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };
            string ReportCaption = "Sales Statement Report";

            string mimeType, encoding, fileNameExtension = (rptFormat == "Excel") ? "xlsx" : (rptFormat == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            try
            {
                string rptQuery = "";

                var sqlQuery = "Truncate table TempCashBankClosing";
                arQuery.Add(sqlQuery);
                
                if (model.GateExpensesSubList != null && model.GateExpensesSubList.Count > 0)
                {
                    for (int i = 0; i < model.GateExpensesSubList.Count; i++)
                    {
                        sqlQuery = "EXEC rptTranLedger_CashBank_Summary " + Session["ComId"] + ", '" + model.GateExpensesSubList[i].LedgerId + "', '" + model.dtFrom + "', '" + model.dtTo + "'";
                        arQuery.Add(sqlQuery);                        
                    }
                }

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);



               //Get Result Set
                rptQuery = "EXEC rptTranLedger_CashBank_Summary " + Session["ComId"] + ", 0, '', '' ";
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
    }

}