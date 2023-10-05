using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using System.IO;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class ProductController : Controller
    {
        #region Common Data
        public int ComId = clsCommon.ComID;
        ArrayList _arQuery = new ArrayList();
        DataSet _dsCombo = new DataSet();
        DataSet _dsList = new DataSet();
        public static DataSet dsEdit = new DataSet();
        softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        string _sqlQuery = "";
 
        #endregion Common Data

        public string PrcLoadCombo()
        {
            try
            {
                _sqlQuery = "EXEC [prcGetProduct] '" + ComId + "', 0 ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsCombo, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsCombo);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }


        public string GetSerialByProduct(int productid)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsCombo = new DataSet();
            try
            {
                string sqlQuery = "Exec [prcGetSerial] " + ComId + "," + productid + " ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsCombo, sqlQuery);
                DataTable dtData = (DataTable)dsCombo.Tables[0];
                return clsCommon.JsonSerialize(dtData);
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        public string PrcLoadList()
        {
            try
            {
                _sqlQuery = "EXEC [prcGetProduct_List] '" + ComId + "' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
  
        #region Product-Create
        public ActionResult CommonCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CommonCreate(Product model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = PrcDataSaveCommon(model);
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
            catch(Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        #endregion product creation


        #region Update Product
        public ActionResult Edit(Int64 Id)
        {
            Product model = new Product();
            model.ProductId = Id;
            return View(model);
        }

        public string GetProductById(Int64 Id)
        {
            try
            {
                _sqlQuery = "EXEC [prcGetProduct] '" + ComId + "', '" + Id + "' ";
                _clsCon.softifyFillDatasetUsingSQLCommand(ref _dsList, _sqlQuery);
                return clsCommon.JsonSerializeDataSet(_dsList);
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        [HttpPost]
        public ActionResult Edit(Product model)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    msg = PrcDataUpdate(model);
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
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Save/update/delete
        public string PrcDataSaveCommon(Product model)
        {
            try
            {
                _sqlQuery = "SELECT  cast(IsNull(MAX(ProductId),0) + 1 AS float) AS ProductId FROM tblSal_Product WHERE ComId = '" + ComId + "' ";
                double productId = _clsCon.softifyCountingDataDouble(_sqlQuery);

                HttpPostedFileBase postedFile = null;
                string path = Server.MapPath("~/Content/assets/images/Product/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                /*  Image File Name */
                if (Request.Files.Count > 0)
                {
                    postedFile = Request.Files[0];
                    model.ProductImage = "P" + productId + "_C" + Session["ComId"] + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".") + 0);
                }

                _sqlQuery = String.Format($"INSERT INTO tblSal_Product(ComId, ProductId, ProductCode, ProductName, ProductUnitId,  WeightId, CountryId, ProductCategoryId, ProductSCategoryId,Barcode,Description, BrandId, ProductImage, Model, SellingPrice, CostPrice, IsSerial, Isvalidity, Warranty,ROL,ROQ, LUserId, dtEntry) VALUES (" + ComId+", "+productId+", dbo.fncNewId('PRODUCT', " + productId + ", "+ComId+"), '"+model.ProductName+"', "+model.ProductUnitId+ ",  " + model.WeightId + ", " + model.CountryId + "," + model.ProductCategoryId+", 1,'"+model.BarCode+"', '"+model.Description+"', "+model.BrandId+",  '"+model.ProductImage+"',  '"+model.Model+ "', '"+model.SellingPrice+"',  '"+model.CostPrice+"', "+Convert.ToInt16(model.IsSerial)+", "+Convert.ToInt16(model.Isvalidity) +", '"+model.Warranty+"', "+model.ROL+", "+model.ROQ+", "+Session["UserId"]+", GetDate() )");
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(productId, _sqlQuery, "Insert", "ProductContrller", "tblSal_Product"));
                /* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                /*  Image File Name */
                if (Request.Files.Count > 0)
                {
                    postedFile.SaveAs(Path.Combine(path, model.ProductImage));
                }
                return "Data saved successfully";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        public string PrcDataUpdate(Product model)
        {
            string path = Server.MapPath("~/Content/assets/images/Product/");
            HttpPostedFileBase postedFile = null;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                /*  Image File Name and save*/
                if (Request.Files.Count > 0)
                {
                    postedFile = Request.Files[0];
                    model.ProductImage = "P" + model.ProductId + "_C" + Session["ComId"] + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".") + 0);
                }

                //Update
                if (!string.IsNullOrEmpty(model.ProductImage))
                {
                    _arQuery.Add(prcUpdateProductData(model, "ProductImage= '" + model.ProductImage + "',"));
                }
                else
                {
                    _arQuery.Add(prcUpdateProductData(model, string.Empty));
                }

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(model.ProductId, _sqlQuery, "Update", "ProductContrller", "tblSal_Product"));
                /* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "Data updated successfully";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }

        private string prcUpdateProductData(Product model, string ProductImage) => "UPDATE tblSal_Product SET ProductName = '"+model.ProductName+ "', ProductCategoryId = " + model.ProductCategoryId+", BrandId = "+model.BrandId+", BarCode = '"+model.BarCode+"', Description = '"+model.Description+"', ProductUnitId = "+model.ProductUnitId+ ", WeightId = " + model.WeightId + ", CountryId = " + model.CountryId + ", " + ProductImage + " CostPrice = '"+model.CostPrice+"', SellingPrice = '"+model.SellingPrice+"', Model = '"+model.Model+ "', ROL = "+model.ROL+", ROQ = "+model.ROQ+", IsSerial="+Convert.ToInt16(model.IsSerial)+ ", Isvalidity = " + Convert.ToInt16(model.Isvalidity) +", Warranty = '"+model.Warranty+"' WHERE ComId = "+ComId+" And ProductId = "+model.ProductId+" ";

        public string PrcDeleteData(Int64 itemId)
        {
            try
            {
                _sqlQuery = $"DELETE FROM tblSal_Product WHERE ComId = {ComId} And ProductId = {itemId}  ";
                _arQuery.Add(_sqlQuery);

                /* START : Transaction Log */
                _arQuery.Add(clsCommon.TransLogInsert(itemId, _sqlQuery, "Delete", "ProductContrller", "tblSal_Product"));
                /* END : Transaction Log */

                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);
                return "1";
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsCon = null; _clsProc = null; _arQuery = null; _dsCombo.Dispose(); _dsList.Dispose(); }
        }
        #endregion

    }

}