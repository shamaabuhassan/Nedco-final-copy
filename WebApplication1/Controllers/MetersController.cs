using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MetersController : Controller
    {
        // GET: Meters
        public ActionResult Index(int ?id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult Search(int ? UserId)
        {
            int rc;
            if (UserId != null)
            {
                Meter[] meters = Meter.GetMeters(new MeterParameters { UserId = UserId }, out rc);
                ViewBag.meters = meters;
            }
            return View();
        }

        public ActionResult Save( int? userId, decimal? amount,string meterid)
        {
            if (userId != null)
            {
                Meter meter = new Meter( userId, amount, meterid);
                int result;
                result = meter.SaveData();
                ViewBag.result = result;
            }
            return View();
        }
    }

}