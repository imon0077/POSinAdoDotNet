using SoftifyFoodPOSNew.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using SoftifyFoodPOSNew.CustomeFilter;

namespace SoftifyFoodPOSNew.Controllers
{
    [SessionFilter]
    public class HomeController : Controller
    {
        DataSet dsList = new DataSet();
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Career()
        {
            byte comId = 2;
            //dsList = AvailableJobs.prcGetData("Details", comId, 0, 0);
            //ViewBag.Status = dsList.Tables[1].Rows[0];
            //List<AvailableJobs> model = await AvailableJobs.prcGetData(comId);
            //if (model == null)
            //{
            //    return HttpNotFound();
            //}
            //TempData["model"] = model;

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}