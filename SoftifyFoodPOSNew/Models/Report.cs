using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Softify;
using Microsoft.Reporting.WebForms;
using System.Web.Mvc;
using System.IO;

namespace SoftifyFoodPOSNew.Models
{
    public class Report
    {
        [Display(Name= "Id :")]
        public int rptId { get; set; }
        [Required(ErrorMessage = "Please provide report code.")]
        [Display(Name="Report Code :")]
        public string rptCode { get; set; }
        [Required(ErrorMessage = "Please provide report path.")]
        [Display(Name = "Report Path :")]
        public string rptPath { get; set; }
        [Required(ErrorMessage = "Please provide report name.")]
        [Display(Name = "Report Name :")]
        public string rptName { get; set; }
        [Required(ErrorMessage = "Please provide report DSN Name.")]
        [Display(Name = "Report DSN Name")]
        public string DSNName { get; set; }
        [Display(Name = "Exist Subreport :")]
        public Boolean IsExistSub { get; set; }
        public List<SubReport> rptSub { get; set; } 

        public class SubReport
        {
            [Display(Name="Report Id")]
            public int rptId { get; set; }
            [Display(Name = "Report Id")]
            public string rptNameSub { get; set; }
            [Display(Name = "Report Id")]
            public string SqlQuerySub { get; set; }
            [Display(Name = "Report Id")]
            public string DSNNameSub { get; set; }
            [Display(Name = "Report Id")]
            public string RFNId { get; set; }
        }

        public static Report prcSetData(string flag, DataSet dsList)
        {
            Report report = new Report();
            List<SubReport> subReports = new List<SubReport>();

            if (flag == "Details")
            {
                report.rptId = int.Parse(dsList.Tables[1].Rows[0]["ReportID"].ToString());
                report.rptCode = dsList.Tables[1].Rows[0]["ReportID"].ToString();
                report.rptPath = dsList.Tables[1].Rows[0]["ReportID"].ToString();
                report.rptName = dsList.Tables[1].Rows[0]["ReportID"].ToString();
                report.DSNName = dsList.Tables[1].Rows[0]["ReportID"].ToString();
                report.IsExistSub = (int.Parse(dsList.Tables[1].Rows[0]["ReportID"].ToString()) == 0) ? false : true;
                

                for (int i = 0; i < dsList.Tables[2].Rows.Count; i++)
                {
                    SubReport sReport = new SubReport();
                    sReport.rptId = int.Parse(dsList.Tables[2].Rows[i]["SubReportID"].ToString());
                    sReport.rptNameSub = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    sReport.SqlQuerySub = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    sReport.DSNNameSub = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    sReport.RFNId = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    subReports.Add(sReport);
                }

                report.rptSub = subReports;
            }
            else
            {
                for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
                {
                    SubReport sReport = new SubReport();
                    sReport.rptId = int.Parse(dsList.Tables[2].Rows[i]["SubReportID"].ToString());
                    sReport.rptNameSub = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    sReport.SqlQuerySub = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    sReport.DSNNameSub = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    sReport.RFNId = dsList.Tables[2].Rows[i]["SubReportType"].ToString();
                    subReports.Add(sReport);
                }

                report.rptSub = subReports;
            }
            return report;
        }



        public static DataSet prcGetData(string Flag, int id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("GTRReports",true);
            try
            {
                string sqlQuery = "Exec prcGetReports '" + Flag + "'," + id + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return dsList;
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


        internal static ReportViewer SetLocalReport(DataTable dt, string name, string reportPath)
        {
            ReportViewer reportViewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                DocumentMapWidth = Unit.Percentage(100),
                Width = Unit.Percentage(150),
                Height = Unit.Percentage(150),
                ZoomMode = ZoomMode.PageWidth
            };

            reportViewer.LocalReport.ReportPath = reportPath;
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(name, dt));
            //reportViewer.LocalReport.Refresh();

            //ViewPage.ReportViewer = reportViewer;
            //TempData["ReportViewer"] = reportViewer;
            //TempData.Keep("ReportViewer");
            return reportViewer;
        }
        private ReportParameter[] GetParametersLocal()
        {
            ReportParameter p1 = new ReportParameter("ReportTitle", "Local Report Example");
            return new ReportParameter[] { p1 };
        }
        internal static string ReturnExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".txt":
                    return "text/plain";
                case ".doc":
                    return "application/msword";
                case ".pdf":
                    return "application/pdf";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".jpg":
                case "jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".wav":
                    return "audio/wav";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".dwg":
                    return "image/vnd.dwg";
                default:
                    return "application/octet-stream";
            }
        }
        public static byte[] Export(string ReportType, string ReportName, string ReportPath, DataTable dataTable)
        {
          
            try
            {
                LocalReport localReport = new LocalReport();
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1";
                string reportName = "Report Name";
                string mimeType;
                string encoding;
                string fileNameExtension = (ReportType == "Excel") ? "xlsx" : (ReportType == "Word" ? "doc" : "pdf");
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                localReport.ReportPath = ReportPath;
                reportDataSource.Value = dataTable;

                reportName = ReportName;
                localReport.DataSources.Add(reportDataSource);
                renderedBytes = localReport.Render(ReportType, "", out mimeType, out encoding,
                    out fileNameExtension, out streams, out warnings);
                //Response.AddHeader("Content-Disposition", $"attachment; filename={reportName}.{fileNameExtension}");
                //Response.ContentType = Models.Report.ReturnExtension("." + fileNameExtension.ToLower());
                //Response.AddHeader("content-length", renderedBytes.Length.ToString());
                //Response.BinaryWrite(renderedBytes);
                return renderedBytes;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            //finally
            //{
            //    clsCon = null;
            //}
        }

    }
    public partial class dsLocalReport
    {
    }
}