using System;
using System.Collections;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using Microsoft.Reporting.WebForms;
using System.Linq;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class TransferController : Controller
    {

        #region Common Data
        public int ComId = clsCommon.ComID;
        ArrayList _arQuery = new ArrayList();
        DataSet _dsCombo = new DataSet();
        DataSet _dsList = new DataSet();
        softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        string _sqlQuery = "";
        public static DataSet DsListEdit = new DataSet();
        #endregion Common Data

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        public string PrcComboLoad()
        {
            try
            {
                _sqlQuery = $"EXEC prcGetproduct_Transfer {ComId}, 0, 0";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string GetTransferItem(int id = 0)
        {
            try
            {
                _sqlQuery = $"EXEC [PrcGetTransfer_Accept_List] {ComId}, {id} ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string GetItem(string value)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            try
            {
                string sqlQuery = $"Exec prcGetItem_Auto '{Session["ComId"]}', '{value}' ";
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
                clsCon = null;
                dsList = null;
            }
        }
        public ActionResult Accept()
        {
            return View();
        }
        public string PrcLoadList(string dtFrom, string dtTo)
        {
            try
            {
                _sqlQuery = $"EXEC prcGetProduct_Transfer_List {ComId},  '{dtFrom}', '{dtTo}'  ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcLoadListToAccept()
        {
            try
            {
                _sqlQuery = $"EXEC prcGetProduct_Transfer_List {ComId}, '',''  ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }


        public ActionResult Create()
        {
            return View();
        }

        //Load warehoue stock
        public string PrcLoadWHStock(int whid=0, string dtTransfer = "")
        {
            try
            {
                _sqlQuery = $"EXEC prcGetproduct_Transfer {ComId}, -1, {whid}, '{dtTransfer}'  ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        [HttpPost]
        public ActionResult Create(Transfer model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var msg = PrcDataSave(model);
                    if (msg.Contains("saved"))
                    {
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msgModelState = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return Json(msgModelState, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            Transfer model = new Transfer();
            model.TransferId = id;
            return View(model);
        }

        public string GetTransferById(int id)
        {
            try
            {
                _sqlQuery = $"EXEC [prcGetproduct_Transfer] {ComId}, {id} ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        [HttpPost]
        public ActionResult Edit(Transfer model)
        {
            try
            {
                if (ModelState.IsValid)

                {
                    var msg = PrcDataUpdate(model);
                    if (msg.Contains("updated"))
                    {
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msgModelState = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return Json(msgModelState, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetAcceptDataById(int id)
        {
            try
            {
                _sqlQuery = $"EXEC [prcGetProduct_Transfer_Accept] {ComId}, {id}";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string GetTransferDataListtoView(int id)
        {
            try
            {
                _sqlQuery = $"EXEC [PrcGetProduct_TransferData] {ComId}, {id}";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                DataTable dtData = (DataTable)_dsList.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        #endregion End Action

        public string PrcDataSave(Transfer model)
        {
            try
            {
                _sqlQuery = $"SELECT Cast(isNull(MAX(TransferId),0) + 1 AS float) AS TransferId FROM tblSal_Product_Transfer_Main WHERE ComId = {ComId} ";
                double NewId = _clsCon.softifyCountingDataDouble(_sqlQuery);
                string TransferNo = "T" + DateTime.Today.ToString("ddMMyy") + "-" + DateTime.Now.ToString("hhmmss");

                _sqlQuery = $"INSERT INTO tblSal_Product_Transfer_Main(ComId, TransferId, TransferNo, dtTransfar, WHFrom, WHTo, Remarks, dtEntry, LUserId) VALUES ({ComId}, {NewId}, '{TransferNo}','{model.dtTransfar.ToString()}', {model.WHFrom},'{model.WHTo}', '{model.Remarks}',  GetDate(), {Session["UserId"]} )";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "TransferContrller", "tblSal_Product_Transfer_Main"));
                /* END : Transaction Log */

                if (model.TransferSubList != null && model.TransferSubList.Count > 0)
                {

                    foreach (var item in model.TransferSubList)
                    {
                        if (item.Qty > 0)
                        {
                            _sqlQuery = $"INSERT INTO tblSal_Product_Transfer_Sub ( ComId, TransferId, ProductId, Qty, UnitPrice, Amount, RowNo) Values ({ComId}, {NewId}, {item.ProductId}, {item.Qty}, {item.UnitPrice},{item.Amount}, {item.RowNo} )";
                            _arQuery.Add(_sqlQuery);

                            /* START : Transaction Log */
                            _arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "Insert", "TransferController", "tblSal_Product_Transfer_Sub"));
                            /* END : Transaction Log */
                        }
                    }
                }

                ///* START : Transaction Log */
                //_arQuery.Add(clsCommon.TransLogInsert(NewId, _sqlQuery, "StockUpdateTransfer", "TransferContrller", "tblSal_Product"));
                ///* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data saved successfully";
            }


            catch (Exception ex)
            {
                return "Error! Ex:" + ex.Message;
            }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDataUpdate(Transfer model)
        {
            try
            {
               _sqlQuery = $"Delete tblSal_Product_Transfer_Sub Where ComId ={ComId} And TransferId  = {model.TransferId} ";
                _arQuery.Add(_sqlQuery);

                //  Update: Main Table
                _sqlQuery = $"Update tblSal_Product_Transfer_Main Set dtTransfar='{model.dtTransfar.ToString()}', dtTransfar='{model.dtTransfar.ToString()}',WHFrom='{model.WHFrom}', WHTo='{model.WHTo}',  Remarks='{model.Remarks}', UpdateById= {Session["UserId"]}, dtUpdate= GetDate() Where ComId = {ComId} And TransferId= {model.TransferId} ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(model.TransferId, _sqlQuery, "Update", "TransferContrller", "tblSal_Product_Transfer_Main"));
                /* END : Transaction Log */

                if (model.TransferSubList != null && model.TransferSubList.Count > 0)
                {

                    foreach (var item in model.TransferSubList)
                    {
                        if (item.Qty > 0)
                        {
                            _sqlQuery = $"INSERT INTO tblSal_Product_Transfer_Sub (ComId, TransferId, ProductId, Qty, UnitPrice, Amount, RowNo) Values ({ComId}, {model.TransferId}, {item.ProductId}, {item.Qty}, {item.UnitPrice},{item.Amount}, {item.RowNo} )";
                            _arQuery.Add(_sqlQuery);

                            /* START : Transaction Log */
                            _arQuery.Add(clsCommon.TransLogInsert(model.TransferId, _sqlQuery, "Insert", "TransferContrller", "tblSal_Product_Damage_Sub"));
                            /* END : Transaction Log */
                        }
                    }
                }

                ///* START : Transaction Log */
                //_arQuery.Add(clsCommon.TransLogInsert(model.TransferId, _sqlQuery, "TransferUpdate", "TransferContrller", "tblSal_Product"));
                ///* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Successfully updated";
            }

            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDeleteData(Int64 ItemId)
        {
            try
            {
                _sqlQuery = $"DELETE FROM tblSal_Product_Transfer_Main WHERE ComId = {ComId} And TransferId = {ItemId}  ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "Delete", "TransferContrller", "tblSal_Product_Transfer_Sub"));
                /* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data Deleted Successfully.";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcAcceptData(Int64 ItemId)
        {
            try
            {
               // _sqlQuery = $"Exec prcStockUpdateBy_Transfer {Session["ComId"]}, {ItemId} ";
                //_arQuery.Add(_sqlQuery);

                _sqlQuery = $"Update tblSal_Product_Transfer_Main Set isAccept= 1 WHERE ComId = {ComId} And TransferId = {ItemId}  ";
                //_arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(ItemId, _sqlQuery, "Accept", "TransferContrller", "tblSal_Product_Transfer_Sub"));
                /* END : Transaction Log */
                //Stock update

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "1";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

    }
}