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
    public class AccDepositWithdrawController : Controller
    {
        public int comId = clsCommon.ComID;
        DataSet dsList = new DataSet();
        
        // GET: 
        public ActionResult Index()
        {
            return View();
        }

        //load Data list
        public async Task<string> GetDataList(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = await Task.Run(() => $"Exec prcGetAcc_Payment_List {Session["ComId"]}, 0, '{ dtFrom }', '{ dtTo }', '{Session["UserId"]}' ");
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
        public ActionResult Create(AccDepositWithdraw model)
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

        //Get Trn Data by Id to edit
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


        #region start transaction with database
        public string prcSaveData(AccDepositWithdraw model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";

            try
            {                
                sqlQuery = $"Select Cast(IsNull(MAX(TrnId),0) + 1 as float) As NewId From tblAcc_Deposit_Withdraw WHERE ComId = {comId} ";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);

                string TrnNo = "Trn" + Session["ComId"] + "-" + DateTime.Today.ToString("ddMMyy") + "-" + NewId;

                sqlQuery = $"INSERT INTO tblAcc_Deposit_Withdraw(ComId, TrnId, dtTrn, TrnNo, FromHeadId, ToHeadId, TrnType, ChequeNo, dtCheque, Amount, isPosted, isCleared, Remarks, dtEntry, LUserId)  Values({comId}, {NewId}, '{model.dtTrn}', '{TrnNo}', {model.FromHeadId}, {model.ToHeadId}, '{model.TrnType}', '{model.ChequeNo}', '{model.dtCheque}', {model.Amount}, 0, 0, '{model.Remarks}', GetDate(), {Session["UserId"]} )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "AccDepositWithdraw", "tblAcc_Deposit_Withdraw"));
                //END : Transaction Log           

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                return "Data saved successfully \r\nTrn No: " + TrnNo;
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
               
                sqlQuery = $"Update tblAcc_Deposit_Withdraw set dtPayment = '{model.dtPayment}', SupplierId = {model.SupplierId}, PayMode = '{model.PayMode}', BankId ={model.BankId}, ChequeNo = '{model.ChequeNo}', dtCheque = '{model.dtCheque}', Amount = '{model.Amount}', Discount = {model.Discount}, Remarks = '{model.Remarks}', UpdatedById = {Session["UserId"]}, dtUpdate = GetDate() Where ComId = {Session["ComId"]} And PaymentId={model.PaymentId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.PaymentId, sqlQuery, "Update", "AccDepositWithdraw", "tblAcc_Deposit_Withdraw"));
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
                sqlQuery = await Task.Run(()=>$"Delete tblAcc_Deposit_Withdraw Where ComId = {Session["ComId"]} And TrnId = {ItemId} ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "AccDepositWithdraw", "tblAcc_Deposit_Withdraw"));
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
        
        #endregion database transaction
    } //end : Controller
}