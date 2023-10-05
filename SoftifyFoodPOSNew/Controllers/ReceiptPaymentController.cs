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
    public class ReceiptPaymentController : Controller
    {

        public int ComId = clsCommon.ComID;
        DataSet dsList = new DataSet();
        public static DataSet dsEdit = new DataSet();
        public static DataSet dsListEdit = new DataSet();

        List<clsCommon.clsCombo1> BrandList = new List<clsCommon.clsCombo1>();
        List<clsCommon.clsCombo2> EmployeeList = new List<clsCommon.clsCombo2>();
        List<clsCommon.clsCombo1> TypeList = new List<clsCommon.clsCombo1>();
        List<clsCommon.clsCombo2> JobList = new List<clsCommon.clsCombo2>();

        public ActionResult Index()
        {
            return View();
        }

        public string GetRecvList(string dtFrom = "", string dtTo = "")
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec [prcGet_ReceiptPayment_List] " + Session["ComId"] + ", " + Session["UserId"] + ", '" + dtFrom + "','" + dtTo + "' ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        public ActionResult CommonCreate()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ReceiptPayment model)
        {
            try
            {
                var message = prcDataSave(model);
                return Json(message);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Edit(int id)
        {
            dsEdit = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec [prcGetReceiptPayment] " + ComId + " , '" + id + "'";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsEdit, sqlQuery);
                return View();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsCon = null;
            }

        }


        [HttpPost]
        public ActionResult Edit(ReceiptPayment model)
        {
            try
            {
                string RecvNo = prcUpdateData(model);
                return Json(RecvNo);
            }
            catch (Exception ex)
            {
                return Json("!! Failure: " + ex.Message);
            }

        } //end : Edit

        // Angular js Load Combo
        public string GetComboLoad()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec prcGetReceiptPayment " + ComId + ",0 ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsCon = null;
            }

        }

        public string GetDueAmount(int ClientId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec prcGetDueAmount " + ClientId + "," + ComId + "";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsCon = null;
            }            
        }

        public string GetSupDueAmount(int SupplierId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec prcGetSupDueAmount " + SupplierId + "," + ComId + "";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsCon = null;
            }

        }

        public string GetEditdata()
        {
            return clsCommon.JsonSerializeDataSet(dsEdit);
        }

        public string[] prcDataSave(ReceiptPayment model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string RecvNo = "";
            try
            {
                var sqlQuery = "Select Cast(Isnull(MAX(RecvPayId),0) + 1 AS float)  AS RecvPayId FROM tblRecvPay_Main";
                double RecvPayId = clsCon.softifyCountingDataDouble(sqlQuery);
               
                sqlQuery = "Insert Into tblRecvPay_Main(RecvPayId, ComId, RecvNo, dtRecv,  RecvPayType, Remarks, LUserId, PcName) Values ( " + RecvPayId + "," + Session["ComId"] + ", dbo.fncNewId('payment', " + RecvPayId + "),'" + clsProc.softifyDateFormat(model.dtRecv.ToString()) + "', '" + model.RecvPayType + "', '" + model.Remarks + "', " + Session["UserId"] + ", '" + clsProc.softifyPCName() + "' ) ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(RecvPayId, sqlQuery, "Insert", "ReceiptPayment", "tblRecvPay_Main"));
                //END : Transaction Log       

                if (model.ReceiptPaymentSubList != null && model.ReceiptPaymentSubList.Count > 0)
                {
                    for (int i = 0; i < model.ReceiptPaymentSubList.Count; i++)
                    {
                        if (model.ReceiptPaymentSubList[i].PayMode == "Cash")
                            model.ReceiptPaymentSubList[i].ChequeDate = "";

                        sqlQuery = "Insert Into tblRecvPay_Sub(RecvPayId,SupplierId,ClientId,DueAmount, Amount, Discount, PayMode,HeadId,ChequeNo,ChequeDate, Remarks,ComId) Values (" +RecvPayId + ", '" + model.ReceiptPaymentSubList[i].SupplierId + "','" + model.ReceiptPaymentSubList[i].ClientId + "','" + model.ReceiptPaymentSubList[i].DueAmount + "','" + model.ReceiptPaymentSubList[i].Amount + "', '" + model.ReceiptPaymentSubList[i].Discount + "','" + model.ReceiptPaymentSubList[i].PayMode + "', '" + model.ReceiptPaymentSubList[i].HeadId + "', '" + model.ReceiptPaymentSubList[i].ChequeNo + "', '" + clsProc.softifyDateFormat(model.ReceiptPaymentSubList[i].ChequeDate.ToString()) + "','" + model.ReceiptPaymentSubList[i].Remarks + "','" + Session["ComId"] + "') ";
                        arQuery.Add(sqlQuery);

                        //START : Transaction Log 
                        arQuery.Add(clsCommon.TransLogInsert(RecvPayId, sqlQuery, "Insert", "ReceiptPayment", "tblRecvPay_Sub"));
                        //END : Transaction Log  
                    }
                }

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                /*---Insert TransProcess-------Start---*/
                if (model.RecvPayType == "Payment")
                {
                    //clsCommon.TransProcessInsert("Payment", RecvPayId, model.ReceiptPaymentSubList[0].Amount);
                }
                else if (model.RecvPayType == "Collection")
                {
                   // clsCommon.TransProcessInsert("Collection", RecvPayId, model.ReceiptPaymentSubList[0].Amount);

                    if (model.ReceiptPaymentSubList[0].Discount > 0)
                    {
                        //clsCommon.TransProcessInsert("Collection", RecvPayId, model.ReceiptPaymentSubList[0].Discount);
                    }
                }
                /*---Insert TransProcess--------END------*/
                sqlQuery = "Select RecvNo  FROM tblRecvPay_Main where RecvPayId = '" + RecvPayId + "'";
                RecvNo = clsCon.softifyGetStringData(sqlQuery);
                var result = new string[] { RecvNo, RecvPayId.ToString() };
                return result;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : 

        public string prcUpdateData(ReceiptPayment model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                var sqlQuery = "";
                sqlQuery = " DELETE FROM tblRecvPay_Sub  WHERE RecvPayId = " + model.RecvPayId + " ";
                arQuery.Add(sqlQuery);

                sqlQuery = string.Format("UPDATE tblRecvPay_Main SET  Remarks = '" + clsProc.softifyAvoidSingleQuote(model.Remarks) + "',  dtRecv = '" + clsProc.softifyDateFormat(model.dtRecv.ToString()) + "', RecvPayType = '" + model.RecvPayType + "' where RecvPayId = " + model.RecvPayId + " ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.RecvPayId, sqlQuery, "Update", "ReceiptPayment", "tblRecvPay_Main"));
                //END : Transaction Log  

                for (int i = 0; i < model.ReceiptPaymentSubList.Count; i++)
                {
                    sqlQuery = "Insert Into tblRecvPay_Sub(RecvPayId,SupplierId,ClientId,DueAmount, Amount, PayMode,HeadId,ChequeNo,ChequeDate, Remarks,ComId) Values (" +
                        model.RecvPayId + ", '" + model.ReceiptPaymentSubList[i].SupplierId + "','" + model.ReceiptPaymentSubList[i].ClientId + "','" + model.ReceiptPaymentSubList[i].DueAmount + "','" + model.ReceiptPaymentSubList[i].Amount + "', '" + model.ReceiptPaymentSubList[i].PayMode + "', '" + model.ReceiptPaymentSubList[i].HeadId + "', '" + model.ReceiptPaymentSubList[i].ChequeNo + "', '" + model.ReceiptPaymentSubList[i].ChequeDate + "','" + model.ReceiptPaymentSubList[i].Remarks + "','" + Session["ComId"] + "') ";
                    arQuery.Add(sqlQuery);

                    //START : Transaction Log 
                    arQuery.Add(clsCommon.TransLogInsert(model.RecvPayId, sqlQuery, "Update", "ReceiptPayment", "tblRecvPay_Sub"));
                    //END : Transaction Log 
                }

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.RecvPayId, sqlQuery, "Delete", "ReceiptPayment", "tblStr_TransProcess"));
                //END : Transaction Log  

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Successfully Updated.";

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                clsCon = null;
            }
        } //end : 

        public string prcDeleteData(int ItemId, int SupplierId, int ClientId)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            var sqlQuery = "";
            try
            {
                sqlQuery = $"DELETE FROM  tblRecvPay_Main WHERE ComId = {Session["ComId"]} And RecvPayId = {ItemId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "ReceiptPayment", "tblRecvPay_Main"));
                //END : Transaction Log    

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data Deleted Successfully.";
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

        #region Cleared ChequeDate
        public string prcChequeCleare(int Id, string Type, string dtCheque)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            var sqlQuery = "";
            try
            {
                if (Type == "Collection")
                {
                    sqlQuery = "Update tblSales_Collection Set isCleared = 1, dtCheque = '" + dtCheque + "' WHERE CollectionId = " + Id + " ";
                    arQuery.Add(sqlQuery);
                    
                    sqlQuery = "Update tblStr_TransProcess Set isCleared = 1, dtCheque = '" + dtCheque + "' Where DocId = '" + Id + "' And TransType='" + Type + "'";
                    arQuery.Add(sqlQuery);
                }
                else if (Type == "Payment")
                {
                    sqlQuery = "Update tblAcc_Payment Set isCleared = 1, dtCheque = '" + dtCheque + "' WHERE PaymentId = " + Id + " ";
                    arQuery.Add(sqlQuery);
                    
                    sqlQuery = "Update tblStr_TransProcess Set isCleared = 1 Where DocId = '" + Id + "' And TransType='" + Type + "'";
                    arQuery.Add(sqlQuery);
                }
                else
                {
                    sqlQuery = "Update tblExpense_Main Set isCleared = 1, ChequeDate = '" + dtCheque + "' WHERE ExpId = " + Id + " ";
                    arQuery.Add(sqlQuery);
                }
                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
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

        #endregion

        #region Mony Receipt
        public ActionResult RptMonyReceipt(int Id, string rptType = "pdf")
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet rptDS = new DataSet();
            string ReportCaption = "Money Receipt";
            LocalReport localReport = new LocalReport();
            ReportDataSource reportDataSource = new ReportDataSource { Name = "DataSet1" };

            string mimeType, encoding, fileNameExtension = (rptType == "Excel") ? "xlsx" : (rptType == "Word" ? "doc" : "pdf");
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            try
            {
                string rptQuery = "";
                //string sqlQuery = "";

                localReport.ReportPath = Server.MapPath("~/Report/rptMoneyReceipt.rdlc");

                rptQuery = "EXEC rptMoneyReceipt '" + Session["ComId"] + "', '" + Id + "' ";
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
            finally
            {
                clsCon = null;
            }
            return null;
        }

        #endregion
    }
}