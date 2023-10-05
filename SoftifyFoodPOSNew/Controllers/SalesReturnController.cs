using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Softify;
using Microsoft.Reporting.WebForms;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class SalesReturnController : Controller
    {
        #region Common Data
        public int ComId = clsCommon.ComID;
        ArrayList _arQuery = new ArrayList();
        DataSet _dsList = new DataSet();
        softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        string _sqlQuery = "";
        #endregion Common Data


        // GET: SalesReturn
        public ActionResult Index()
        {
            return View();
        }

        public string GetReturnList(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = "EXEC prcGetSales_Return_List " + ComId + ", '" + dtFrom + "', '" + dtTo + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SalesReturn model)
        {
            try
            {
                string msg = "";
                if (ModelState.IsValid)
                {
                    msg = prcDataSave(model);
                    if (msg.Contains("saved"))
                        return Json("1", JsonRequestBehavior.AllowGet);
                    else
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

        public string GetComboLoad()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec prcGetSales_Return " + ComId + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        public string GetAutocompleteData(string Flag)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = $"Exec prcGetSalesInvoiceNo_AutoComplete {ComId}, '{Flag}' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        //Load Invoice data based on invoice selection
        public string GetInvoiceData(Int64 InvoiceId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = $"Exec prcGetSales_Return {ComId}, '{InvoiceId}' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        #region CURD Operation
        public string prcDataSave(SalesReturn model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = $"SELECT Cast(Isnull(MAX(ReturnId),0) + 1 AS float) AS NewId FROM tblSales_Return_Main Where ComId ={Session["ComId"]} ";
                double ReturnId = clsCon.softifyCountingDataDouble(sqlQuery);
                string ReturnNo = "RET" + Session["ComId"] + "-" + DateTime.Today.ToString("ddMMyy") + "-" + DateTime.Now.ToString("hhmmss");
                
                sqlQuery = $"Insert Into tblSales_Return_Main(ComId, ReturnId, ReturnNo, dtReturn, ClientId, InvoiceId, Reason, ReturnType, Total, Remarks, PCName, LuserId, dtEntry) Values ({ComId}, {ReturnId}, '{ReturnNo}' , '{model.dtReturn.ToString()}', {model.ClientId}, {model.InvoiceId}, '{model.Reason}', '{model.ReturnType }', '{model.Total}', '{model.Remarks}', '{clsProc.softifyPCName()}', {ComId}, GetDate() )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "Insert", "SalesReturn", "tblSales_Return_Main"));
                //END : Transaction Log 

                if (model.GateSalesReturnSubList != null && model.GateSalesReturnSubList.Count > 0)
                {
                    for (int i = 0; i < model.GateSalesReturnSubList.Count; i++)
                    {
                        int RowNo = i + 1;

                        sqlQuery = $"INSERT INTO tblSales_Return_Sub (ComId, ReturnId, ProductId, ReturnQty, PrdModel, Rate, SerialId, RowNo) Values ({ComId}, {ReturnId}, {model.GateSalesReturnSubList[i].ProductId}, {model.GateSalesReturnSubList[i].ReturnQty}, '{model.GateSalesReturnSubList[i].PrdModel}', {model.GateSalesReturnSubList[i].Rate}, {model.GateSalesReturnSubList[i].SerialId}, {RowNo} )";
                        arQuery.Add(sqlQuery);

                        //Transaction with database
                        arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "Insert", "SalesReturn", "tblSales_Return_Sub"));
                        //End of Transaction with database                     
                    }
                }
                else
                {
                    return "Please provide at least one item";
                }


                //Update stock 
                sqlQuery = $"Exec prcStockUpdateBySalesReturn {ComId}, {ReturnId}, 0 ";
                arQuery.Add(sqlQuery);

                //Transaction with database
                arQuery.Add(clsCommon.TransLogInsert(ReturnId, sqlQuery, "StockUpdate", "SalesReturnController", "tblSal_Product"));
                //End of Transaction with database   

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);


                #region collection For DueReturn
                if (model.ReturnType == "DueReturn")
                {
                    ArrayList _arQuery = new ArrayList();
                    sqlQuery = $"Select dbo.fncInvNewNo({ComId}, 'Collection', '', 0)";
                    string CRNo = _clsCon.softifyGetStringData(sqlQuery);

                    sqlQuery = "Select Cast(IsNull(MAX(CollectionId),0) + 1 as float)  As NewId From tblSales_Collection";
                    double CollectionId = _clsCon.softifyCountingDataDouble(sqlQuery);

                    sqlQuery = $"INSERT INTO tblSales_Collection(ComId, CollectionId, dtCollection, CollectionNo, ClientId, InvoiceId, PayMode, BankId, Amount, Discount, isCleared, isPosted, Remarks, dtEntry, LuserId)  Values({ComId}, {CollectionId}, '{model.dtReturn}', '{CRNo}', {model.ClientId}, {model.InvoiceId}, 'Cash', '1', {model.Total}, 0, 0, 0, 'Due Return Adjustment' , GetDate(), {Session["UserId"]} )";
                    _arQuery.Add(sqlQuery);

                    //START : Transaction Log 
                    _arQuery.Add(clsCommon.TransLogInsert(CollectionId, sqlQuery, "Insert", "SalesCollection", "tblSales_Collection"));
                    //END : Transaction Log 

                    _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                }
                #endregion collection For DueReturn

                /***********************************************/

                #region Expense For CashReturn
                if (model.ReturnType == "CashReturn")
                {
                    ArrayList _arQuery = new ArrayList();
                    sqlQuery = $"SELECT Cast(isNull(MAX(ExpId),0) + 1 AS float) AS NewId FROM tblExpense_Main Where ComId = {Session["ComId"]} ";
                    double ExpId = clsCon.softifyCountingDataDouble(sqlQuery);

                    sqlQuery = "Insert Into tblExpense_Main(ComId, ExpId, ExpNo, dtExp, ExpType, HeadId, Description, Total, LUserId, PCName, dtEntry) Values ('" + ComId + "', '" + ExpId + "', dbo.fncNewId('EXP', '" + ExpId + "', " + ComId + "), '" + model.dtReturn + "', 'Cash', '1', '" + ReturnNo + "', '" + model.Total + "', '" + Session["UserId"] + "', '" + clsProc.softifyPCName() + "', GetDate() )";
                    _arQuery.Add(sqlQuery);

                    //START : Transaction Log 
                    _arQuery.Add(clsCommon.TransLogInsert(ExpId, sqlQuery, "Insert", "Expenses", "tblExpense_Main"));
                    //END : Transaction Log 
                    
                    sqlQuery = "INSERT INTO tblExpense_Sub (ComId, ExpId, HeadId, Amount, Remarks, RowNo) Values (" + ComId + ", '" + ExpId + "', '2', '" + model.Total + "', 'Cash Return Expense, Ret. No. : "+ ReturnNo + "', '1')";
                    _arQuery.Add(sqlQuery);

                    _arQuery.Add(clsCommon.TransLogInsert(ExpId, sqlQuery, "Insert", "ExpensesController", "tblExpense_Sub"));
                    
                    _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                }
                #endregion Expense For CashReturn


                return "Data saved successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            finally
            {
                _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsList.Dispose();
            }
            //} //end : prcDataSave

        }

        public ActionResult RptReturn(string id, string rptFormat = "pdf")
        {
            DataSet rptDS = new DataSet();
            string ReportCaption = "Sales Return Report";
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

                rptQuery = $"EXEC rptInvoice_Return {ComId}, {id}  ";
                localReport.ReportPath = Server.MapPath("~/Report/rptInvoice_Return.rdlc");

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

        #endregion CURD Operation
    }
}