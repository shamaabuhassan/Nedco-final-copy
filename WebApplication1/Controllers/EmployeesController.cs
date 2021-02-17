using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmployeesController : Controller
    {
        // GET: Employees
        public ActionResult Index(string error)
        {
            if (Session["employee"] != null)
            {
                return RedirectToAction("index", "Mainpage");
            }
           
            ViewBag.error = error;
            return View();
        }


        public ActionResult checklogin(string username,string password)
        {
            Employee employee = Employee.CheckEmployeeLogin(username, password);
            if (employee != null)
            {
                Session["employee"] = employee;
                return RedirectToAction("index", "Mainpage");
            }
            else
            {
                return RedirectToAction("Index", "Employees", new { error = 2 });
            }
        }
    }
}