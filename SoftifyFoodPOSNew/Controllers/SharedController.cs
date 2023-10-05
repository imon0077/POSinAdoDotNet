using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Softify;
using SoftifyFoodPOSNew.CustomeFilter;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        public ActionResult Modal()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public string GetAutoCompleteData(string value)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "";
                sqlQuery = "Exec [prcGetSupplier]'" + Session["ComId"] + "','" + value + "'";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return clsCommon.JsonSerializeDataSet(dsList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                clsCon = null;
            }
        }
    }
}