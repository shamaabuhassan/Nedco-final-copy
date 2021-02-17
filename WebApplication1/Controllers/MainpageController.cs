using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class MainpageController : Controller
    {
        // GET: Mainpage
        public ActionResult Index(string error)
        {
            if (error == "2")
            {
                return RedirectToAction("index", "Employees");
            }
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
           // ViewBag.Id = Id;
            return View();
        }

        public ActionResult Logout()
        {
            Session["employee"] = null;
            return RedirectToAction("index", "Employees");
        }

   
        public ActionResult MainPage()
        {
            return RedirectToAction("index", "Mainpage");
        }


    }
}