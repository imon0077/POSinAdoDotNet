using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models.MiniInventory
{
    public class MiniSupplier
    {
        public int supplierId { get; set; }
        public string SupplierCode { get; set; }

        [Required(ErrorMessage = "Please Provide Supplier Name")]
        public string supplierName { get; set; }

        [Required(ErrorMessage = "Please Provide Address")]
        public string SupplierAddress { get; set; }

        public string SupplierPhone { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierEmail { get; set; }
        public string ContactName { get; set; }

        public string ContactPhone { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int AccId { get; set; }



        private void prcSetData(IDataRecord reader)
        {
            supplierId = Convert.ToInt32(reader["supplierId"].ToString());
            SupplierCode = reader["SupplierCode"].ToString();
            supplierName = reader["supplierName"].ToString();
            SupplierAddress = reader["SupplierAddress"].ToString();
            SupplierPhone = reader["SupplierPhone"].ToString();
            SupplierMobile= reader["SupplierPhone"].ToString();
            ContactName = reader["ContactName"].ToString();
            ContactPhone = reader["ContactPhone"].ToString();

            CountryId = Convert.ToInt32(reader["CountryId"].ToString());
            CountryName = reader["CountryName"].ToString();
           // AccIds = reader["AccId"].ToString();
        }

        ////For Combo
        public static DataSet prcGetData(int id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection("SoftifyStockManagement", true);
            try
            {
                string sqlQuery = "Exec prcGet_Supplier "+HttpContext.Current.Session["ComId"]+", " + id + " ";
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

    }
}