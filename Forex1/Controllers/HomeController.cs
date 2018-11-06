using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Forex.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            ExchangeRateController ctrl = new ExchangeRateController();
            DateTime startDate = new DateTime(2018, 1, 15);
            DateTime endDate = new DateTime(2018, 2, 12);
            dynamic result = JsonConvert.DeserializeObject<dynamic>(ctrl.GetExchangeRates(startDate, endDate, "SEK", "NOK"));

            string resultString = string.Format("A min rate of {0} on {1}{2}", result["MinRate"].ToString(), result["MinRateAt"].ToString(),Environment.NewLine);
            resultString += string.Format("A Max rate of {0} on {1}{2}", result["MaxRate"].ToString(), result["MaxRateAt"].ToString(), Environment.NewLine);
            resultString += string.Format("An Average rate of {0}", result["AverageRate"].ToString());

            ViewBag.Result = resultString;
            return View(); 

        }
    }
}
