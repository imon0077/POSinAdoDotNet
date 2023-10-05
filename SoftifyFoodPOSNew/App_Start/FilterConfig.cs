using System;
using System.Web;
using System.Web.Mvc;
using SoftifyFoodPOSNew.CustomeFilter;

namespace SoftifyFoodPOSNew
{
    public class FilterConfig
    {
        public int AdminLoginType { get; private set; }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new SessionFilter());

        }

    }
}
