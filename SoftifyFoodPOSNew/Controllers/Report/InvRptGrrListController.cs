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
    public class InvRptGrrListController : Controller
    {

        DataSet dsList = new DataSet();
        
        private string ReportPath = "";
        private string rptQuery = "";
        private string DataSourceName = "DataSet1";
        private string ReportCaption = "";


        public async Task<ActionResult> Report(string RptType, string Criteria, string dtFrom, string dtTo, int? GrrNo)
        {
            DataSet rptDS = new DataSet();

            if (Request.HttpMethod == "POST" && !string.IsNullOrEmpty(RptType))
            {
                if (string.IsNullOrEmpty(Criteria))
                {
                    ModelState.AddModelError("Criteria", "Please select report criteria");
                }
                if (Criteria == "PONo" && GrrNo == null)
                {
                    ModelState.AddModelError("Criteria", "Please select PO No");
                }
                if (string.IsNullOrEmpty(dtFrom))
                {
                    ModelState.AddModelError("Criteria", "Please select From Date");
                }
                if (string.IsNullOrEmpty(dtTo))
                {
                    ModelState.AddModelError("Criteria", "Please select To Date");
                }
                if (ModelState.IsValid)
                {

                    DataSourceName = "DataSet1";
                    ReportCaption = "PO Report...";
                    LocalReport localReport = new LocalReport();
                    ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

                    string mimeType;
                    string encoding;
                    string fileNameExtension = (RptType == "Excel") ? "xlsx" : (RptType == "Word" ? "doc" : "pdf");
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    string Id = "0";

                    if (Criteria.ToUpper() == "Date Wise".ToUpper())
                    {
                    }

                    if (Criteria.ToUpper() == "GrrWise".ToUpper())
                    {
                        Id = GrrNo?.ToString();
                    }
                    SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
                    try
                    {
                        if (Criteria.Contains("GrrWise"))
                        {
                            ReportPath = Server.MapPath("~/Report/rptGRR.rdlc");
                            rptQuery = "Exec rptGRR " + Session["ComId"] + ",'" + Id + "' ";
                        }
                        else
                        {
                            ReportPath = Server.MapPath("~/Report/rptGrrList.rdlc");
                            rptQuery = "Exec rptGrrList " + Session["ComId"] + ",  '" + Convert.ToDateTime(dtFrom).ToShortDateString() + "','" + Convert.ToDateTime(dtTo).ToShortDateString() + "', 0, 'Date Wise' ";
                        }

                        clsCon.softifyFillDatasetUsingSQLCommand(ref rptDS, rptQuery);
                        localReport.ReportPath = ReportPath;
                        reportDataSource.Value = rptDS.Tables[0];

                        localReport.DataSources.Add(reportDataSource);
                        renderedBytes = localReport.Render(RptType, "", out mimeType, out encoding,
                        out fileNameExtension, out streams, out warnings);
                        Response.AddHeader("Content-Disposition", $"inline; filename={ReportCaption}.{fileNameExtension}");
                        Response.ContentType = Models.Report.ReturnExtension("." + fileNameExtension.ToLower());
                        Response.AddHeader("content-length", renderedBytes.Length.ToString());
                        Response.BinaryWrite(renderedBytes);
                        ModelState.Clear();
                        return File(renderedBytes, mimeType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            ViewBag.Criteria = Criteria;
            ViewBag.RptType = RptType;
            dsList = await prcGetData(int.Parse(Session["ComId"].ToString()));
            ViewBag.GrrList = dsList.Tables[0];
            return View();
        }

   


        public static async Task<DataSet> prcGetData(int ComId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                DataSet dsList = new DataSet();

                string sqlQuery = "Exec prcRpt_GrrList " + ComId + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);

                return dsList;
            }
            catch (Exception ex)
            {
                throw new Exception(MvcApplication.UserErrorMessage = ex.Message);
            }
            finally
            {
                clsCon = null;
            }
        }


    }
}