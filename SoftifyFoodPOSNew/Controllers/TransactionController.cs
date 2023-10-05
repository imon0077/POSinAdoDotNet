using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using Microsoft.Reporting.WebForms;


namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class TransactionController : Controller
    {
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        public int comId = clsCommon.ComID;
        public static DataSet dsEdit = new DataSet();
        DataSet dsList = new DataSet();

        List<clsCommon.clsCombo1> Typelist = new List<clsCommon.clsCombo1>();
        public void prcLoadCombo()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery1 = "Exec prcGetTransToPost 2, 0, '' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery1);
                Typelist = clsGenerateList.prcColumnOne(dsList.Tables[0]);
                ViewBag.Typelist = Typelist;
            }
            catch(Exception ex)
            {
                throw (ex);
            }
            finally
            {
                clsCon = null;
            }

        }
        public ActionResult Index()
        {
            prcLoadCombo();
            return View();
        }
        public ActionResult LedgerDetails()
        {
            return View();
        }

        public ActionResult CashBankLedger()
        {
            return View();
        }

        public ActionResult DueLedger()
        {
            return View();
        }

        public string GetComboLoadLedger(Transaction model, string id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                if (string.IsNullOrEmpty(id.ToString())) { id = "0"; }

                sqlQuery = "EXEC prcGetLedger '" + model.LedgerStatus + "', '" + comId + "', '" + id + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }


        }

        public string GetSupplierDueData()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = $"EXEC rptDue_Ledger_Supplier {comId}, 0 ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }

        }

        public string GetCustomerDueData()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = $"EXEC rptDue_Ledger_Customer {comId}, 0 ";
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

        public string GetComboLoad(Transaction model)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet(); string sqlQuery = "";
            try
            {
                if (model.TransStatus == "post")
                    sqlQuery = "EXEC prcGetTransToPost 2,0,'" + model.TransType + "' ";

                else if (model.TransStatus == "unpost")
                    sqlQuery = "EXEC prcGetTransToUnPost 2,0,'" + model.TransType + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        public string prcDataSave(Transaction model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = ""; var tablename = "tblSal_Invoice_Main"; if (!model.TransType.Contains("INV")) tablename = "tblInv_Grr_Main";

                for (int i = 0; i < model.TransactionSubList.Count; i++)
                {
                    if (model.TransStatus == "post")
                    {
                        sqlQuery = "exec prcProcessTransection'"+ model.TransStatus + "','" + model.TransType + "','" + model.TransactionSubList[i].TransId + "','" + model.PostDate + "'";
                    }

                    else if (model.TransStatus == "unpost")
                    {
                        sqlQuery = "exec prcProcessTransection'"+ model.TransStatus + "','" + model.TransType + "','" + model.TransactionSubList[i].TransId + "','" + model.PostDate + "'";
                    }
                    arQuery.Add(sqlQuery);
                    //START : Transaction Log 
                    arQuery.Add(clsCommon.TransLogInsert(model.TransactionSubList[i].TransId, sqlQuery, "Update", "Transaction", tablename));
                    //END : Transaction Log 

                }

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                if (model.TransStatus == "post")
                {
                    return "Post Data Success";
                }
                else if (model.TransStatus == "unpost")
                {
                    return "Unpost Data Success";

                }
                return "";
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


        public ActionResult RptLedgerStatus(string id, string LedgerStatus = "", string FromDate = "", string ToDate = "", string GrrId = "", string rptType = "pdf")
        {
            DataSet rptDS = new DataSet();
            string ReportCaption = "Ledger Report";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptType == "Excel") ? "xlsx" : (rptType == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            if (string.IsNullOrEmpty(id.ToString()))
            {
                id = "0";
            }
            string rptQuery = "";
            try
            {
                localReport.ReportPath = Server.MapPath("~/Report/rptLedgerDetails.rdlc");
                if (LedgerStatus.Contains("Supplier"))
                {                    
                    rptQuery = $"EXEC rptTranLedger_Supplier {comId}, {id}, '{FromDate }', '{ToDate}', '{GrrId}' ";
                }
                else if (LedgerStatus.Contains("Customer"))
                {
                    rptQuery = $"EXEC rptTranLedger_Customer {comId}, {id}, '{FromDate }', '{ToDate}', '{GrrId}' ";
                }
                else if (LedgerStatus.Contains("CashBank"))
                {
                    rptQuery = $"EXEC rptTranLedger_CashBank {comId}, {id}, '{FromDate }', '{ToDate}', '{GrrId}' ";
                }
                else
                {
                    rptQuery = $"EXEC rptTranLedger_Expense {comId}, {id}, '{FromDate }', '{ToDate}', '{GrrId}' ";
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
            finally
            {
                _clsCon = null;
            }
            return null;
        }


        public ActionResult RptLedgerCashBank(string id, string LedgerStatus = "", string FromDate = "", string ToDate = "", string GrrId = "", string rptType = "pdf")
        {
            DataSet rptDS = new DataSet();
            string ReportCaption = "Ledger Report";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptType == "Excel") ? "xlsx" : (rptType == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            if (string.IsNullOrEmpty(id.ToString()))
            {
                id = "0";
            }
            string rptQuery = "";
            try
            {
                
                if (LedgerStatus.Contains("Details"))
                {
                    rptQuery = $"EXEC rptTranLedger_CashBank {comId}, {id}, '{FromDate }', '{ToDate}', '{GrrId}' ";

                    localReport.ReportPath = Server.MapPath("~/Report/rptLedgerDetails.rdlc");

                    _clsCon.softifyFillDatasetUsingSQLCommand(ref rptDS, rptQuery);
                }
                else
                {
                    rptQuery = $"EXEC rptTranLedger_CashBank_Summary {comId}, {id}, '{FromDate }', '{ToDate}', '{GrrId}' ";

                    localReport.ReportPath = Server.MapPath("~/Report/rptLedgerDetails.rdlc");

                    _clsCon.softifyFillDatasetUsingSQLCommand(ref rptDS, rptQuery);
                }

               
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
            finally
            {
                _clsCon = null;
            }
            return null;
        }


        public ActionResult RptLedgerDueStatus(string LedgerStatus = "", string rptType = "pdf")
        {
            DataSet rptDS = new DataSet(); 
            string ReportCaption = "Due Report";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptType == "Excel") ? "xlsx" : (rptType == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            if (string.IsNullOrEmpty(LedgerStatus.ToString()))
            {
                LedgerStatus = "";
            }
            string rptQuery = "";
            try
            {
                localReport.ReportPath = Server.MapPath("~/Report/rptDueLedgerDetails.rdlc");
                rptQuery = "EXEC rptDueLedger '" + comId + "', '"+ LedgerStatus + "'";
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
            finally
            {
                _clsCon = null;
            }
            return null;
        }

    }
}