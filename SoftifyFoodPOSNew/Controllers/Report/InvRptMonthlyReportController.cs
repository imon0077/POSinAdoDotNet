using Softify;
using SoftifyFoodPOSNew.Models;
using System;
using System.Threading.Tasks;
using System.Data;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using SoftifyFoodPOSNew.CustomeFilter;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class InvRptMonthlyReportController : Controller
    {

        DataSet dsList = new DataSet();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);

        private string ReportPath = "";
        private string rptQuery = "";
        private string DataSourceName = "DataSet1";
        private string ReportCaption = "";



        public ActionResult RptMonthlyData()
        {
            return View();
        }


        public ActionResult RptMonthlyReport(string FromDate = "", string ToDate = "", string rptType = "pdf")
        {
            DataSet rptDS = new DataSet();
            string ReportCaption = "Monthly Report";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptType == "Excel") ? "xlsx" : (rptType == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            string rptQuery = "";
            try
            {                
                localReport.ReportPath = Server.MapPath("~/Report/rptMonthlyReport.rdlc");

                rptQuery = "EXEC rptMonthlyReport '" + Session["ComId"] + "', '" + FromDate + "','" + ToDate + "' ";
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