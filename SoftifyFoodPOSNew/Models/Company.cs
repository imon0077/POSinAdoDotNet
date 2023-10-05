using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Company
    {


        public int comId { get; set; }
        public string comName { get; set; }
        public string comAlias { get; set; }
        public string comAddress { get; set; }
        public string comAddress2 { get; set; }
        public string comPhone { get; set; }
        public string comFax { get; set; }
        public string comEmail { get; set; }
        public string comWeb { get; set; }
        public string comType { get; set; }
        public string comFinYear { get; set; }
        public string InvoicePrintSize { get; set; }
        public bool IsSerial { get; set; }
        public bool IsWarranty { get; set; }
        public string comImage { get; set; }

        public List<CompanySub> CompanySubList { get; set; }
        //comId, InvoiceType, Type, COM, Year, Number

        public class CompanySub
        {
            public string VarId { get; set; }
            public string Type { get; set; }
            public string COM { get; set; }
            public int Year { get; set; }
            public int Number { get; set; }
            public string No { get; set; }
        }





        private void prcSetData(IDataReader reader)
        {
            comId = Convert.ToInt32(reader["comId"].ToString());
            comName = reader["comName"].ToString();
            comAlias = reader["comAlias"].ToString();
            comAddress = reader["comAddress"].ToString();
            comAddress2 = reader["comAddress2"].ToString();
            comPhone = reader["comPhone"].ToString();
            comFax = reader["comFax"].ToString();
            comEmail = reader["comEmail"].ToString();
            comWeb = reader["comWeb"].ToString();
            comType = reader["comType"].ToString();
            comFinYear = reader["comFinYear"].ToString();
         
        }

        public static List<Company> prcGetData()
        {
            IDataReader reader = null;
            List<Company> list = new List<Company>();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGet_Compnay '" + HttpContext.Current.Session["ComId"] + "', -1 ";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    Company Item = new Company();
                    Item.prcSetData(reader);
                    list.Add(Item);
                }
                return list;
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

        public static Company prcGetData(long id)
        {
            IDataReader reader = null;
            Company model = new Company();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetFactory " + HttpContext.Current.Session["ComId"] + ", " + id + " ";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    model.prcSetData(reader);
                    break;
                }
                return model;
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

        public static DataSet prcGetDatacombo()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGet_Compnay " + HttpContext.Current.Session["ComId"]+", 0 ";
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