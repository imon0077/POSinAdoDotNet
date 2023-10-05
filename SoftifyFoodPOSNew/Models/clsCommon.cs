using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class clsCommon
    {

        public static string ControllerNameP { get; set; }
        public static string ActionNameP { get; set; }
        public static int ComID { get; set; }
        public static int UserID { get; set; }


        #region Combo
        public class clsCombo1
        {
            public string Name { get; set; }
        }

        public class clsCombo2
        {
            public Int64 Id { get; set; }
            public string Name { get; set; }
        }
        #endregion Combo
        public static string JsonSerialize(DataTable dt)
        {
            //DataTable dt = (DataTable)dsData.Tables[0];
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = 500000000;

            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in dt.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }

            dt.Dispose();
            return jsSerializer.Serialize(parentRow);
        }
        //end : jsonConverter

        //start : JsonSerializeDataSet
        public static string JsonSerializeDataSet(DataSet ds)
        {
            ds.AcceptChanges();
            return JsonConvert.SerializeObject(ds, Formatting.Indented);
        }
        //end : JsonSerializeDataSet


        //start : TransLogInsert
        //clsCommon.TransLogInsert(1, _sqlQuery, "Insert", "ProductContrller", "tblSal_Product");
        public static string TransLogInsert(double refId, string sqlQuery, string tranType, string formName, string tableName)
        {
            ArrayList _arQuery = new ArrayList();
            softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
            //SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
            string _sqlQuery = "";
            try
            {
                _sqlQuery ="Insert Into tblUser_Trans_Log (ComId, LUserId, formName, tranStatement, tranType, PcName, TableName, RefId) Values ('" +
                    HttpContext.Current.Session["ComId"] + "', '" + HttpContext.Current.Session["UserId"] + "', '" +formName + "', '" + sqlQuery.Replace("'", "|") + "', '" + tranType + "', '" +_clsProc.softifyPCName() + "', '" + tableName + "',  " + refId + " )";

                /*_arQuery.Add(_sqlQuery);
                _clsCon.softifyDataSaveUsingSqlCommend(_arQuery);*/
                return _sqlQuery;
            }
            catch (Exception ex) { return "Error! Ex:" + ex.Message; }
            finally { _sqlQuery = ""; _clsProc = null; _arQuery = null; }
        }
        //end : TransLogInsert


        //public static string TransProcessInsert(double TransId, string DocType, double DocId, float Amount, string TranType = "", int RefId = 0)
        //{
        //    string _sqlQuery = "";
        //    try
        //    {
        //        _sqlQuery = $"Insert Into tblStr_TransProcess (ComId, TransId, TransDate, DocType, DocId, Amount, TransType, ReffId) Values( {ComID}, {TransId}, GetDate(), '{DocType}', {DocId}, {Amount}, '{TranType}', {RefId} )";

        //        return _sqlQuery;
        //    }
        //    catch (Exception ex) { return "Error! Ex:" + ex.Message; }
        //    finally { _sqlQuery = ""; }

        //}

        //public static string TransProcessUpdate(string DocType, double DocId, float Amount, string TransType)
        //{
        //    ArrayList arQuery = new ArrayList();
        //    softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        //    SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        //    string _sqlQuery = "";
        //    try
        //    {
        //        _sqlQuery = $"UPDATE tblStr_TransProcess SET Amount = {Amount}, TransDate= GetDate() Where DocId = {DocId} And DocType='{DocType}' And TransType = '{TransType}' ";
        //        return _sqlQuery;
        //    }
        //    catch (Exception ex) { return "Error! Ex:" + ex.Message; }
        //    finally { _sqlQuery = ""; _clsProc = null; arQuery = null; }
        //}

        //public static string TransProcessDelete(string DocType, double DocId, string TransType)
        //{
        //    ArrayList arQuery = new ArrayList();
        //    softifyInterfaceHelper _clsProc = new softifyInterfaceHelper();
        //    SoftifySQLConnection _clsCon = new SoftifySQLConnection(true);
        //    string _sqlQuery = "";
        //    try
        //    {
        //        _sqlQuery = $"Delete from tblStr_TransProcess Where ComId = {ComID} And DocId = {DocId} And DocType = {DocType} And TransType '{TransType}' ";
        //        arQuery.Add(_sqlQuery);
        //        _clsCon.softifyDataSaveUsingSqlCommend(arQuery);

        //        return _sqlQuery;
        //    }
        //    catch (Exception ex) { return "Error! Ex:" + ex.Message; }
        //    finally { _sqlQuery = ""; _clsProc = null; arQuery = null; }
        //}
    }

    public static class clsGenerateList
    {
        public static List<clsCommon.clsCombo1> prcColumnOne(DataTable dt)
        {
            List<clsCommon.clsCombo1> list = new List<clsCommon.clsCombo1>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                clsCommon.clsCombo1 item = new clsCommon.clsCombo1();
                item.Name = dt.Rows[i][0].ToString();
                list.Add(item);
            }
            return list;
        }
        public static List<clsCommon.clsCombo2> prcColumnTwo(DataTable dt)
        {
            List<clsCommon.clsCombo2> list = new List<clsCommon.clsCombo2>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                clsCommon.clsCombo2 item = new clsCommon.clsCombo2();
                item.Id = Int64.Parse(dt.Rows[i][0].ToString());
                item.Name = dt.Rows[i][1].ToString();
                list.Add(item);
            }
            return list;
        }
        public static List<clsCommon.clsCombo2> prcColumnTwo(DataRow[] datarow)
        {
            List<clsCommon.clsCombo2> list = new List<clsCommon.clsCombo2>();

            foreach (DataRow dr in datarow)
            {
                clsCommon.clsCombo2 item = new clsCommon.clsCombo2();
                item.Id = Int64.Parse(dr[0].ToString());
                item.Name = dr[1].ToString();
                list.Add(item);
            }
            return list;
        }

    }




}