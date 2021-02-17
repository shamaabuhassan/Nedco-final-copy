using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        public ActionResult Index(string name)
        {
            ViewBag.name = name;
            return View();
        }

        public ActionResult Search(int? CountryId)
        {
            int rc;
            if (CountryId != null)
            {
                Customer[] customers = Customer.GetCustomers(new CustomerParameters { CountryId = CountryId }, out rc);
                ViewBag.customers = customers;
            }
            return View();
        }

        public ActionResult Save(int? id, string username, int? CardId, string telephone, int? countryId, int? cityId, int? townid, string street, string password,string name)
        {
            if (username != null) { 
            Customer customer = new Customer(id, username, CardId, telephone, countryId, cityId, townid, street, password, name);
            int result;
            result = customer.SaveData();
            ViewBag.result = result;
        }
            return View();
        }

        public ActionResult customerslist()
        {
           
            if (Session["employee"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "Employees", new { error = 2 });
            }
        }
    }
}