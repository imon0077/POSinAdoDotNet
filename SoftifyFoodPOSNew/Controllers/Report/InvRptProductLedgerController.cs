using Softify;
using SoftifyFoodPOSNew.Models;
using System;
using System.Threading.Tasks;
using System.Data;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using SoftifyFoodPOSNew.CustomeFilter;
using System.Collections;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class InvRptProductLedgerController : Controller
    {
        

        #region Common Data
            public int ComId = clsCommon.ComID;
            ArrayList _arQuery = new ArrayList();
            DataSet _dsCombo = new DataSet();
            DataSet _dsList = new DataSet();
            softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
            string _sqlQuery = "";
        #endregion Common Data


        /* Start : Stock Report */
        #region Stock Report
        public ActionResult StockReport()
        {
            return View();
        }

        public string GetStockList(string dtFrom, string dtTo, bool isCheckAll)
        {
            try
            {
                _sqlQuery = $"EXEC [rptStock_Statement_Details] {ComId}, '{dtFrom}', '{dtTo}', '{Convert.ToInt16(isCheckAll)}' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
        #endregion Stock Report
        /* End : Stock Report */



        /* Start : Item Wise Sales Report */
        #region ItemWiseSales Report
        public ActionResult ItemWiseSales()
        {
            return View();
        }

        public string GetItemWiseSalesList(string dtFrom, string dtTo, bool isCheckAll)
        {
            try
            {
                _sqlQuery = $"EXEC [rptSales_Statement_Itemwise] {ComId}, '{dtFrom}', '{dtTo}', '{Convert.ToInt16(isCheckAll)}' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
        #endregion ItemWiseSales Report
        /* End : Item Wise Sales Report */




        /*
        public async Task<ActionResult> Report(string RptType, string Criteria, string dtFrom, string dtTo, int? Category, int? SubCategory, int? Product)
        {
            DataSet rptDS = new DataSet();

            if (Request.HttpMethod == "POST" && !string.IsNullOrEmpty(RptType))
            {
                if (string.IsNullOrEmpty(Criteria))
                {
                    ModelState.AddModelError("Criteria", "Please select report criteria");
                }
                if (ModelState.IsValid)
                {

                    DataSourceName = "DataSet1";
                    ReportCaption = "Product Ledger...";
                    LocalReport localReport = new LocalReport();
                    ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

                    string mimeType;
                    string encoding;
                    string fileNameExtension = (RptType == "Excel") ? "xlsx" : (RptType == "Word" ? "doc" : "pdf");
                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    string CatId = "0", SCatId = "0", ProductId = "0";

                    if (Criteria.ToUpper() == "All".ToUpper())
                    {
                    }
                    else if (!string.IsNullOrEmpty(Category.ToString()))
                    {
                        CatId = Category?.ToString();
                    }
                    else if (!string.IsNullOrEmpty(Category.ToString()))
                    {
                        SCatId = SubCategory?.ToString();
                    }
                    else if (Criteria.ToUpper() == "ProductWise".ToUpper())
                    {
                        ProductId = Product?.ToString();
                    }
                  
                    SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
                    try
                    {
                        if (Criteria.Contains("All"))
                        {
                            ReportPath = Server.MapPath("~/Report/rptPrdRegister.rdlc");
                            rptQuery = "Exec rptProductSum " + Session["ComId"] + ", " + Session["UserId"] + ", '" + Convert.ToDateTime(dtFrom).ToShortDateString() + "','" + Convert.ToDateTime(dtTo).ToShortDateString() + "',0, 0, 0, 0 ";
                        }
                        else
                        {
                            ReportPath = Server.MapPath("~/Report/rptPrdRegister.rdlc");
                            rptQuery = "Exec rptProductSum " + Session["ComId"] + ", " + Session["UserId"] + ", '" + Convert.ToDateTime(dtFrom).ToShortDateString() + "','" + Convert.ToDateTime(dtTo).ToShortDateString() + "', "+ProductId+",0 , '" + CatId + "', '" + SCatId + "' ";
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
            ViewBag.CetegoryList = dsList.Tables[1];
            ViewBag.SubCetegorytList = dsList.Tables[2];
            ViewBag.ProducttList = dsList.Tables[4]; 
            return View();
        }
        */




        public static async Task<DataSet> prcGetData(int ComId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                DataSet dsList = new DataSet();

                string sqlQuery = "Exec prcrptProduct "+ComId+" ";
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