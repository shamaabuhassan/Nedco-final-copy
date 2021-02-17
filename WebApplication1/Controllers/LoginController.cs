using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index(string error)
        {
            ViewBag.error = error;
            return View();

        }

      

        public ActionResult checklogin(string username,string password)
        {  
                Customer customer = Customer.CheckLogin(username, password);
                if (customer != null)
                {
                    Session["customer"] = customer;
                    return RedirectToAction("Index", "Home",new {id=customer.Id});
                }
                else
                {
                    return RedirectToAction("Index", "Login", new { error = 2 });
                    
                }
            
        }

        
    }

}