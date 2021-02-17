using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ChargeMeterController : Controller
    {
        // GET: ChargeMeter
        public ActionResult Index(string error)
        {
            ViewBag.error = error;
            return View();
        }

        public ActionResult Charge(int? otp)
        {
            Customer customer = null;
            int rc;
            Topup[] topup = Topup.GetTopups(new TopupParameters { OTP = otp }, out rc);
            if (topup[0].Status == "0")
            {
                if (Session["customer"] != null)
                {
                    Meter[] meters = Meter.GetMeters(new MeterParameters { Meterid = topup[0].MeterId }, out rc);
                    customer = (Session["customer"] as Customer);
                    if (customer.Id == meters[0].UserId)
                    {
                        SMS sms = new SMS();
                        sms.To_number = customer.Telephone;
                        sms.Msg = $"  أهلا وسهلا بك أنت تحاول الان شحن عدادك باستخدام موقعنا في الشركة ورقم الكود الذي يريد شحنه هو {otp}";
                        string status = sms.Send();
                        sms.SaveData();
                        if (status == "OK")
                        {
                            topup[0].Charged();
                        }
                        else
                        {
                            return RedirectToAction("index", "ChargeMeter", new { error = 4 });
                        }
                    }

                    else if (customer.Id != meters[0].UserId)
                    {
                        Customer customer1 = new Customer(meters[0].UserId.Value);
                        SMS sms = new SMS();
                        sms.To_number = customer1.Telephone;
                        sms.Msg = $"يحاول {customer.Name} شحن عدادك باستخدام موقعنا في الشركة ورقم الكود الذي يريد شحنه هو {otp}";
                        string status = sms.Send();
                        sms.SaveData();
                        if (status == "OK")
                        {
                            topup[0].Charged();
                        }
                        else
                        {
                            return RedirectToAction("index", "ChargeMeter",new { error = 4 });
                        }
                    }
                    
                }
                else if (Session["customer"] == null)
                {
                    return RedirectToAction("index", "ChargeMeter", new { error = 2 });
                }
             return View();
            }
            else 
            {
                return RedirectToAction("index", "ChargeMeter", new { error = 3 });
            }
        }
    }
}