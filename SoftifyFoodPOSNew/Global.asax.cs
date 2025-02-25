﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace SoftifyFoodPOSNew
{
    public class MvcApplication : System.Web.HttpApplication
    {
 
        internal static string UserErrorMessage = null;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
        }
        protected void Session_Start()
        {
           Session.Add("UserId", null);
           Session.Add("ComId",null);
           Session.Add("ComName",null);
           Session.Add("DisplayName", null);
           Session.Add("InvoicePrintSize", null);
           Session.Add("EmpPicLocation", null);
           Session.Add("BranchId", 0);
        }
    }
}
