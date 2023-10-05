using System;
using System.Data;
using System.Web.Mvc;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;
using System.Net;
using System.IO;
using System.Collections;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class SMSHelperController : Controller
    {
        public int comId = clsCommon.ComID;
        
        public string PrcSmsProcess(double id, string Criteria)
        {
            SoftifySQLConnection clsConnection = new SoftifySQLConnection(true);
            DataSet dsDetails = new DataSet();
            string smsMsg = "";

            try
            {
                string sqlQuery2 = "Exec prcGetSmsGrid " + id + ", " + comId + ", '" + Criteria + "' ";
                clsConnection.softifyFillDatasetUsingSQLCommand(ref dsDetails, sqlQuery2);
                dsDetails.Tables[0].TableName = "tblEmployee";

                foreach (DataRow row in (InternalDataCollectionBase)dsDetails.Tables["tblEmployee"].Rows)
                {
                    smsMsg = SendSms(row["MobileNo"].ToString(), row["sms"].ToString(), comId, Int64.Parse(row["DocId"].ToString()), row["DocType"].ToString(), row["FormName"].ToString());
                }
                return smsMsg;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }


        //start : SendSms  
        public string SendSms(string mobileNo, string SMSText, int comId, Int64 DocId, string DocType, string FormName)
        {
            #region declaration
            ArrayList arQuery = new ArrayList();
            SoftifySQLConnection clsConnection = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();

            DataSet dsDetails = new DataSet();
            var Company = "";
            var Address = "";
            var UserID = "";
            var Password = "";
            var SendID = "";
            var Message1 = "";
            var MobileNo1 = "";
            var smsStatus = "";
            var exMessage = "";
            var sqlQuery = "";
            #endregion declaration

            try
            {
                string sqlQuery2 = "Exec prcGetSmsGrid '2', " + comId + " ";
                clsConnection.softifyFillDatasetUsingSQLCommand(ref dsDetails, sqlQuery2);
                dsDetails.Tables[0].TableName = "tblSmsProvider";

                #region sms providers settings
                foreach (DataRow row in (InternalDataCollectionBase)dsDetails.Tables["tblSmsProvider"].Rows)
                {
                    if (row["Company"].ToString() == "jadusms")
                    {
                        Company = row["Company"].ToString();
                        Address = row["smsAddress"].ToString();
                        UserID = "?Username=" + row["smsUser"];
                        Password = "&Password=" + row["smsPassword"];
                        SendID = "&From=" + row["smsSender"];
                        Message1 = "";
                        MobileNo1 = "";
                    }
                }
                #endregion sms providers settings



                #region SMS Sending via jadusms
                if (Company == "jadusms")
                {
                    WebClient webClient = new WebClient();

                    if (mobileNo.Length > 10)
                    {
                        string address = Address + "&mobileNo=" + mobileNo + "&smsContent=" + SMSText;

                        Stream stream = webClient.OpenRead(address);
                        StreamReader streamReader = new StreamReader(stream);
                        streamReader.ReadToEnd();
                        stream.Close();
                        streamReader.Close();
                        smsStatus = "sent";
                    }
                    else
                    {
                        smsStatus = "wrong mobile No.";
                    }

                    sqlQuery = String.Format("Insert Into tblSMS_Trans_Log(DocId, DocType, FormName, mobileNo, SMSText, SmsProvider, smsStatus) " +
                                 " Values ( '" + DocId + "', '" + DocType + "', '" + FormName + "', '" + mobileNo + "', '" + SMSText + "', '" + Company + "', '" + smsStatus + "' )");
                    arQuery.Add(sqlQuery);
                    clsConnection.softifyDataSaveUsingSqlCommend(arQuery);
                }
                #endregion SMS Sending via jadusms

                return " SMS Sent";
            }
            catch (Exception ex)
            {
                exMessage = ex.Message.ToString();
                smsStatus = "exception thrown";
                sqlQuery = String.Format("Insert Into tblSMS_Trans_Log(DocId, DocType, FormName, mobileNo, SMSText, SmsProvider, smsStatus, exMessage) " +
                             " Values ( '" + DocId + "', '" + DocType + "', '" + FormName + "', '" + mobileNo + "', '" + SMSText + "', '" + Company + "', '" + smsStatus + "', '" + exMessage + "' )");
                arQuery.Add(sqlQuery);
                clsConnection.softifyDataSaveUsingSqlCommend(arQuery);

                throw ex;
            }
        }
        //end : SendSms




    }
}