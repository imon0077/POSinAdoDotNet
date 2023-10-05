using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Softify;
using SoftifyFoodPOSNew.Models;
using System.Collections;
using System.Net;
using System.Threading.Tasks;

using SoftifyFoodPOSNew.CustomeFilter;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class DashboardController : Controller
    {
        DataSet datasetList = new DataSet();
        // GET: Dashboard

        public ActionResult Index()
        {
            return View();
        }

        public string GetDashboardData()
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            DataSet dsList = new DataSet();
            string sqlQuery = "";
            try
            {
                sqlQuery = "Exec prcGetDashBoard " + Session["ComId"] + ", "+Session["UserId"]+" ";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public ActionResult ErrorInfo(string exMessage)
        {
            ViewBag.exMessage = exMessage;
            return View().Danger("Data Insert Failure!! Cause Of " + exMessage);
        }


    }
}