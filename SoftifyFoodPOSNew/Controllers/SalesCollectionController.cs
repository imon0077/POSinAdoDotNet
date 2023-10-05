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
    public class SalesCollectionController : Controller
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
                string sqlQuery = await Task.Run(() => $"Exec prcGetSales_Colleciton {comId}, 0");
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

        // GET: SalesCollection
        public ActionResult Index()
        {
            return View();
        }

        //Cheque collection list
        public async Task<string> GetCollection(string dtFrom, string dtTo)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = await Task.Run(() => $"Exec prcGetSales_Colleciton_List {Session["ComId"]}, 0, '{ dtFrom }', '{ dtTo }', {Session["UserID"]} ");
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
        public ActionResult Create(SalesCollection model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    //if (model.PayMode.ToUpper() == "Cash".ToUpper() && model.BankId > 1)
                    //{
                    //    return Json("Payment method and bank name are not matching...", JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
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
                    //}
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
            SalesCollection model = new SalesCollection();
            model.CollectionId = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SalesCollection model)
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

        // get collection data by id to edit 
        public async Task<string> GetCollectionById(int id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(() => $"Exec prcGetSales_Colleciton_List {comId}, {id} ");
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

        // client data load based on client selection
        public async Task<string> GetClientData(int id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(() => $"Exec prcGetSales_Colleciton {comId}, {id} ");
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                //DataTable dtData = (DataTable)dsList.Tables[0];
                //return clsCommon.JsonSerialize(dtData);
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

        //Cheque collection list
        // GET: SalesCollection
        public ActionResult Deposit()
        {
            return View();
        }
        public async Task<string> GetCollDeposit()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = await Task.Run(() => $"Exec prcGetSales_Colleciton {Session["ComId"]}, -1 ");
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

        #region procedure
        //save data
        public string prcSaveData(SalesCollection model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";

            try
            {

                sqlQuery = $"Select dbo.fncInvNewNo({comId}, 'Collection', '', 0)";
                string CRNo = clsCon.softifyGetStringData(sqlQuery);


                sqlQuery = "Select Cast(IsNull(MAX(CollectionId),0) + 1 as float)  As NewId From tblSales_Collection";
                double NewId = clsCon.softifyCountingDataDouble(sqlQuery);


                sqlQuery = $"INSERT INTO tblSales_Collection(ComId, CollectionId, dtCollection, CollectionNo, ClientId, InvoiceId, PayMode, BankId, ChequeNo, dtCheque, Amount, Discount, isCleared, isPosted, Remarks, dtEntry, LuserId)  Values({comId}, {NewId}, '{model.dtCollection}', '{CRNo}', {model.ClientId}, {model.InvoiceId ?? 0}, '{model.PayMode}', {model.BankId}, '{model.ChequeNo}', '{model.dtCheque}', {model.Amount}, {model.Discount}, 0, 0, '{model.Remarks}' , GetDate(), {Session["UserId"]} )";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(NewId, sqlQuery, "Insert", "SalesCollection", "tblSales_Collection"));
                //END : Transaction Log 

                clsCon.softifyDataSaveUsingSqlCommend(arQuery);
                //Send SMS
                //if (CRNo != "")
                //{
                //    SMSHelperController SmsObj = new SMSHelperController();
                //    SmsObj.PrcSmsProcess(NewId, "CollectionNo");
                //}

                return "Data saved successfully. CR No: "+ CRNo;
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

        //update data
        public string prcUpdateData(SalesCollection model)
        {
            ArrayList arQuery = new ArrayList();
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            string sqlQuery = "";
            try
            {
                sqlQuery = $"Update tblSales_Collection set dtCollection = '{model.dtCollection}', ClientId = {model.ClientId}, PayMode = '{model.PayMode}', BankId ={model.BankId}, ChequeNo = '{model.ChequeNo}', dtCheque = '{model.dtCheque}', Amount = '{model.Amount}', Discount = {model.Discount}, Remarks = '{model.Remarks}', UpdatedById = {Session["UserId"]}, dtUpdate = GetDate() Where ComId = {Session["ComId"]} And CollectionId={model.CollectionId} ";
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(model.CollectionId, sqlQuery, "Update", "SalesCollection", "tblSales_Collection"));
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

        //delete data
        public async Task<string> prcDeleteData(int ItemId)
        {
            ArrayList arQuery = new ArrayList();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(()=>$"Delete tblSales_Collection Where ComId = {Session["ComId"]} And CollectionId = {ItemId} ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete", "SalesCollection", "tblSales_Collection"));
                //END : Transaction Log 

                //Delete From Transaction
                sqlQuery = await Task.Run(() => $"Exec prcProcessDeleteTransaction {Session["ComId"]}, {ItemId}, 'Collection' ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(ItemId, sqlQuery, "Delete Transaction", "SalesCollection", "tblSales_Collection"));
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

        //Deposit Collection
        public async Task<string> prcDepositData(int Id, string dtCheque)
        {
            ArrayList arQuery = new ArrayList();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            string sqlQuery = "";
            try
            {
                sqlQuery = await Task.Run(() => $"Update tblSales_Collection Set isCleared =1, dtCheque = '{dtCheque}', dtCleared = GetDate() Where ComId = {Session["ComId"]} And CollectionId = {Id} ");
                arQuery.Add(sqlQuery);

                //START : Transaction Log 
                arQuery.Add(clsCommon.TransLogInsert(Id, sqlQuery, "Cheque Clear", "SalesCollection", "tblSales_Collection"));
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

        #endregion end of procedure
    } //end : Controller
}