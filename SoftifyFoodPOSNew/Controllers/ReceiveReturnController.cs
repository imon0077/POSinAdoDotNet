using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class ReceiveReturnController : Controller
    {
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        public static DataSet dsEdit = new DataSet();
        // GET: Receive Return 
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
             dsEdit = new DataSet();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "Exec prcGet_ReceiveReturn '" + Session["ComId"] + "', "+ id +" , 'edit'";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsEdit, sqlQuery);
                      
            return View();
        }
        public string GetMainData()
        {
            return clsCommon.JsonSerializeDataSet(dsEdit);
        }
        
        public string GetListData(string dtFrom, string dtTo)
        {
            DataSet dsData = new DataSet();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "Exec prcGet_ReceiveReturn '" + Session["ComId"] + "', 0 , 'list','"+ dtFrom +"','"+ dtTo +"'";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsData, sqlQuery);
            DataTable dtData = (DataTable)dsData.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public string GetProductList()
        {
            DataSet dsData = new DataSet();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "Exec prcGet_ReceiveReturn '" + Session["ComId"] + "', 0 , 'Productlist'";
            clsCon.softifyFillDatasetUsingSQLCommand(ref dsData, sqlQuery);
            DataTable dtData = (DataTable)dsData.Tables[0];
            return clsCommon.JsonSerialize(dtData);
        }
        public string DataSave(ReceiveReturn model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
            
                string sqlQuery = "";
                double NewId = 0;     
                           
                sqlQuery = "SELECT cast(Isnull(MAX(RcvReturnId),0) + 1 AS float)  AS RcvReturnId FROM tblRcv_Return_Main";
                NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                sqlQuery = "Insert Into tblRcv_Return_Main(RcvReturnId, dtRcvReturn,BranchId, LuserId, ComId)Values (" + NewId + ",'" + model.dtRcvReturn.ToString("dd-MMM-yyyy") +"','"+ Session["BranchId"] +"', '" + Session["UserId"] + "','" + Session["ComId"] + "')";
                arQuery.Add(sqlQuery);
                
                // START : Translog Insert
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "ReceiveReturn", "tblRcv_Return_Main"));
                // END : Translog Insert

                if (model.ReceiveReturnSubList.Count >= 0)
                {
                    for (int i = 0; i < model.ReceiveReturnSubList.Count; i++)
                    {
                        int row = i + 1;

                        var sqlQuery2 = "INSERT INTO tblRcv_Return_Sub(RcvReturnId, ProductName, ProductId, Code, AliceCode, Qty,TP,MRP,prodCatName, CatId, RowNo)" +
                                            " Values (" + NewId + ", '" + clsProc.softifyAvoidSingleQuote(model.ReceiveReturnSubList[i].ProductName) + "','" + model.ReceiveReturnSubList[i].ProductId + "','" + clsProc.softifyAvoidSingleQuote(model.ReceiveReturnSubList[i].BarCode) +"','"+clsProc.softifyAvoidSingleQuote(model.ReceiveReturnSubList[i].AliceCode) +"','" + model.ReceiveReturnSubList[i].Qty + "','"+model.ReceiveReturnSubList[i].TP+"','"+model.ReceiveReturnSubList[i].MRP+"','"+model.ReceiveReturnSubList[i].prodCatName + "', '" + model.ReceiveReturnSubList[i].CatId + "','" + row + "')";
                        arQuery.Add(sqlQuery2);

                        arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "ReceiveReturn", "tblRcv_Return_Sub"));
                    }
                }  
                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            finally
            {
                clsCon = null;
            }
        }
        public string DataUpdate(ReceiveReturn model, string Flag)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {

                string sqlQuery = "";
                //if (Flag =="edit") {
                    sqlQuery = "Update tblRcv_Return_Main set DeliveryStatus='" + Flag + "', dtRcvReturn='" + model.dtRcvReturn.ToString("dd-MMM-yyyy") + "', BranchId='" + Session["BranchId"] + "', LuserId ='" + Session["UserId"] + "', ComId ='" + Session["ComId"] + "' Where RcvReturnId =" + model.RcvReturnId;
                    arQuery.Add(sqlQuery);
                    arQuery.Add(clsCommon.TransLogInsert(model.RcvReturnId, sqlQuery, "Update", "ReceiveReturn", "tblRcv_Return_Main"));
                //}               

                if (model.ReceiveReturnSubList.Count >= 0)
                {
                    sqlQuery = "DELETE FROM tblRcv_Return_Sub Where RcvReturnId =" + model.RcvReturnId;
                    arQuery.Add(sqlQuery);
                    for (int i = 0; i < model.ReceiveReturnSubList.Count; i++)
                    {
                        int row = i + 1;
  
                        var sqlQuery2 = "INSERT INTO tblRcv_Return_Sub(RcvReturnId, ProductName, ProductId, Code, AliceCode, Qty,TP,MRP,prodCatName, CatId, RowNo)" +
                                            " Values (" + model.RcvReturnId + ", '" + clsProc.softifyAvoidSingleQuote(model.ReceiveReturnSubList[i].ProductName) + "','" + model.ReceiveReturnSubList[i].ProductId + "','" + clsProc.softifyAvoidSingleQuote(model.ReceiveReturnSubList[i].BarCode) + "','" + clsProc.softifyAvoidSingleQuote(model.ReceiveReturnSubList[i].AliceCode) + "','" + model.ReceiveReturnSubList[i].Qty + "','" + model.ReceiveReturnSubList[i].TP + "','" + model.ReceiveReturnSubList[i].MRP + "','" + model.ReceiveReturnSubList[i].prodCatName + "', '" + model.ReceiveReturnSubList[i].CatId + "','" + row + "')";
                        arQuery.Add(sqlQuery2);

                        arQuery.Add(clsCommon.TransLogInsert(model.RcvReturnId, sqlQuery, "Insert", "ReceiveReturn", "tblRcv_Return_Sub"));
                    }
                    clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                    return "1";
                }
                else
                {
                    clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                    return "1";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            finally
            {
                clsCon = null;
            }
        }
        public string DataDelete(int itemId)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {

                string sqlQuery = "";
                sqlQuery = "DELETE FROM tblRcv_Return_Main Where RcvReturnId =" + itemId;
                arQuery.Add(sqlQuery);
                arQuery.Add(clsCommon.TransLogInsert(itemId, sqlQuery, "Delete", "ReceiveReturn", "tblRcv_Return_Main"));

              
          
                

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            finally
            {
                clsCon = null;
            }
        }

        public ActionResult RptReceiveReturnSheet(string ReceiveReturnId = "''", string rptFormat = "pdf")
        {
            DataSet rptDS = new DataSet();
            string InvoicePrintSize = "A4";
            string DataSourceName = "DataSet1";
            string ReportCaption = "Receive Return Report";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptFormat == "Excel") ? "xlsx" : (rptFormat == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            if (string.IsNullOrEmpty(ReceiveReturnId.ToString()))
            {
                ReceiveReturnId = "0";
            }

            try
            {
                string rptQuery = "";
                
                    localReport.ReportPath = Server.MapPath("~/Report/rptReceiveReturn.rdlc");
              
                rptQuery = "EXEC rpt_ReceiveReturn " + Session["ComId"] + ", '" + ReceiveReturnId + "','"+ Session["UserId"] + "'  ";
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