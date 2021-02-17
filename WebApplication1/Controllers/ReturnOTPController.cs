using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ReturnOTPController : Controller
    {
        // GET: ReturnOTP
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Return( string meterid)
        {
            if (Session["customer"] != null)
            {
                int rc;
                Customer customer = (Session["customer"] as Customer);
                Meter[] meter = Meter.GetMeters(new MeterParameters { Meterid = meterid }, out rc);

                if (customer.Id == meter[0].UserId)
                {
                    SMS sms = new SMS();
                    sms.To_number = customer.Telephone;
                    sms.Msg = $"أهلا وسهلا بكك أنت تحاول الان استرجاع الكود الغير مشحون الخاص بك";
                    string status=sms.Send();
                    sms.SaveData();
                    if (status == "OK")
                    {
                        return RedirectToAction("OTPS", "ReturnOTP", new { meterid = meterid });
                    }
                    else
                    {
                        return RedirectToAction("index", "ReturnOTP");
                    }

                }

                else if (customer.Id == meter[0].UserId)
                {
                    
                    Customer customer1 = new Customer(meter[0].UserId.Value);

                    SMS sms = new SMS();
                    sms.To_number = customer1.Telephone;
                    sms.Msg = $"يحاول {customer.Name} استرجاع الكود الغير مشحون الخاص بك";
                    string status = sms.Send();
                    sms.SaveData();
                    if (status == "OK")
                    {
                        return RedirectToAction("OTPS", "ReturnOTP", new { meterid = meterid });
                    }
                    else
                    {
                        return RedirectToAction("index", "ReturnOTP");
                    }
                    
                }
                return View();
            }
            else
            {
                return RedirectToAction("index", "ReturnOTP");
            }
        }

        public ActionResult OTPS(int meterid) {
            ViewBag.meterid = meterid;
            return View();
        }
    }
}