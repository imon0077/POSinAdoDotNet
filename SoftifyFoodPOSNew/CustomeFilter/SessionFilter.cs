using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.CustomeFilter
{

    public class SessionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            clsCommon.ControllerNameP = filterContext.RouteData.Values["controller"].ToString();
            clsCommon.ActionNameP = filterContext.RouteData.Values["action"].ToString();
            string[] actions=  { "Edit", "Delete","CV", "Applicants","Details" };
            if (HttpContext.Current.Session["DisplayName"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"controller", "Login"},
                        {"action", "Login"}

                    });
            }

            else if (actions.Contains(clsCommon.ActionNameP ) && filterContext.RouteData.Values["id"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                        {"controller",clsCommon.ControllerNameP},
                        {"action", "Index"}

                   });
            }
            base.OnActionExecuting(filterContext);

        }

        
    }
}