using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.Models;
using System.Data;
using System.Threading.Tasks;
using SoftifyFoodPOSNew.CustomeFilter;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class AccPaymentController : Controller
    {

        public int comId = clsCommon.ComID;
        DataSet dsList = new DataSet();

        public async Task<string> prcLoadCombo()
        {
            DataSet ds = new DataSet();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = await Task.Run(() => $"Exec prcGetAcc_Payment {comId}, 0");
                clsCon.softifyFillDatasetUsingSQLCommand(ref ds, sqlQuery);
                return clsCommon.JsonSerializeDataSet(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsCon = null;
                ds = null;
                clsProc = null;
            }
        }

        // GET: 
        public ActionResult Index()
        {
            return View();
        }

        //load Payment list
        public async Task<string> GetPaymentList(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = await Task.Run(() => $"Exec prcGetAcc_Payment_List {Session["ComId"]}, 0, '{ dtFrom }', '{ dtTo }',{Session["UserId"]} ");
                await Task.Run(() => clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery));
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
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
        //[ValidateAntiForgeryToken]
        public ActionResult Create(AccPayment model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {                    
                        if (model.Amount > 0)
                        {
                            msg = prcSaveData(model);
                            if (msg.Contains("saved"))
                            {
                                return Json(msg, JsonRequestBehavior.AllowGet);
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
                if (ex.Message.Contains("UK_BankId, ChequeNo"))
                {
                    return Json("This cheque no already exists.", JsonRequestBehavior.AllowGet);
                }
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Edit(int id)
        {
            AccPayment model = new AccPayment();
            model.PaymentId = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AccPayment model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.PayMode.ToUpper() == "Cash".ToUpper() && model.BankId > 1)
                    {
                        return Json("Payment method and bank name are not matching...", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (model.Amount > 0)
                        {
                            msg = prcUpdateData(model);
                            if (msg.Contains("updated"))
                            {
                                return Json(msg, JsonRequestBehavior.AllowGet);
                            }
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Please, provide amount", JsonRequestBehavior.AllowGet);
                        }
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
                if (ex.Message.Contains("UK_BankId, ChequeNo"))
                {
                    return Json("This cheque no already exists.", JsonRequestBehavior.AllowGet);
                }
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        //Get Payment Data by Id to edit
        public async Task<string> GetPaymentById(int id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(() => $"Exec prcGetAcc_Payment_List {comId}, {id} ");
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsProc = null;
                clsCon = null;
                dsList = null;
            }
        }

        //load supplier information based on supplier selection
        public async Task<string> GetSupplierData(int id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(() => $"Exec prcGetAcc_Payment {comId}, {id} ");
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsProc = null;
                clsCon = null;
                dsList = null;
            }
        }

        #region payment approval
        public ActionResult PaymentApproval()
        {
            return View();
        }
        //load Payment list to approval/Crlear
        public async Task<string> GetPaymentApproval()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = await Task.Run(() => $"Exec prcGetAcc_Payment_List {Session["ComId"]}, -1 ");
                await Task.Run(() => clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery));
                DataTable dtData = (DataTable)dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsCon = null;
                dsList = null;
            }
        }

        #endregion payment approval

        #region start transaction with database
        public string prcSaveData(AccPayment model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";

            try
            {
                sqlQuery = $"Select dbo.fncInvNewNo({comId}, 'Payment', '', 0)";
                string PayNo = clsCon.softifyGetStringData(sqlQuery);

                sqlQuery = $"Select Cast(IsNull(MAX(PaymentId),0) + 1 as float) As NewId From tblAcc_Payment WHERE ComId = {comId} ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

               

                sqlQuery = $"INSERT INTO tblAcc_Payment(ComId, PaymentId, dtPayment, PaymentNo, SupplierId, GrrId, PayMode, BankId, ChequeNo, dtCheque, Amount, Discount, isPosted, isCleared, Remarks, dtEntry, LUserId)  Values({comId}, {NewId}, '{model.dtPayment}', '{PayNo}', {model.SupplierId}, {model.GrrId}, '{model.PayMode}', {model.BankId}, '{model.ChequeNo}', '{model.dtCheque}', {model.Amount}, {model.Discount}, 0, 0, '{model.Remarks}', GetDate(), {Session["UserId"]} )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "AccPayment", "tblAcc_Payment"));
                //END : Transaction Log           

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);

                ////Send SMS
                //if (PayNo != "")
                //{
                //    SMSHelperController SmsObj = new SMSHelperController();
                //    SmsObj.PrcSmsProcess(NewId, "PaymentNo");
                //}

                return "Data saved successfully! Pay No: "+ PayNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsProc = null;
                arQuery = null;
                clsCon = null;
            }
        }

        public string prcUpdateData(AccPayment model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                if (model.PayMode.Contains("Cash"))
                {
                    model.BankId = 1;
                }
                sqlQuery = $"Update tblAcc_Payment set dtPayment = '{model.dtPayment}', SupplierId = {model.SupplierId}, PayMode = '{model.PayMode}', BankId ={model.BankId}, ChequeNo = '{model.ChequeNo}', dtCheque = '{model.dtCheque}', Amount = '{model.Amount}', Discount = {model.Discount}, Remarks = '{model.Remarks}', UpdatedById = {Session["UserId"]}, dtUpdate = GetDate() Where ComId = {Session["ComId"]} And PaymentId={model.PaymentId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.PaymentId, sqlQuery, "Update", "AccPayment", "tblAcc_Payment"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsProc = null;
                arQuery = null;
                clsCon = null;
            }
        }

        public async Task<string> prcDeleteData(int ItemId)
        {
            ArrayList arQuery = new ArrayList();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(()=>$"Delete tblAcc_Payment Where ComId = {Session["ComId"]} And PaymentId = {ItemId} ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "AccPayment", "tblAcc_Payment"));
                //END : Transaction Log 

                //Delete From Transaction
                sqlQuery = await Task.Run(() => $"Exec prcProcessDeleteTransaction {Session["ComId"]}, {ItemId}, 'Payment' ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete Transaction", "AccPayment", "tblAcc_Payment"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
            }
        }

        //Approved Payment
        public async Task<string> prcApprovedPaymentData(int Id, string dtCheque)
        {
            ArrayList arQuery = new ArrayList();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(() => $"Update tblAcc_Payment Set isCleared =1, dtCheque = '{dtCheque}', dtCleared = GetDate() Where ComId = {Session["ComId"]} And PaymentId = {Id} ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(Id, sqlQuery, "Cheque Clear", "AccPayment", "tblAcc_Payment"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(sqlQuery);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                clsCon = null;
                clsProc = null;
                arQuery = null;
            }
        }

        #endregion database transaction
    } //end : Controller
}