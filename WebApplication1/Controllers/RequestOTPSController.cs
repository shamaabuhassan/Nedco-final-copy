using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class RequestOTPSController : Controller
    {
        // GET: RequestOTPS
        public ActionResult Index()
        {

            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            return View();
        }
        public ActionResult RequestOTP(string MeterId, int? Amount, string SerialNUM)
        {

            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {
                int rc;
                Topup topup = new Topup();
                if (SerialNUM != null)
                {
                    CashCard[] cashCard = CashCard.GetCashCards(new CashCardParameters { SerialNumber = SerialNUM }, out rc);
                     topup = new Topup(MeterId, Amount, cashCard[0].SerialNumber);
                }
                return View(topup);
            }
        }

        [HttpPost]
        public ActionResult RequestOTP(Topup topup)
        {

            int? result = 0;
            if (ModelState.IsValid)
            { //checking model state

                //check whether id is already exists in the database or not

                result = topup.SaveData();

                 if (result == 2)
                {
                    ModelState.AddModelError("SerialNUM", "this card is not valid ");
                    ViewBag.result = result;
                    return View(topup);
                }
                else if (result == 4)
                {
                    ModelState.AddModelError("SerialNUM", "this card is not for this meter ");
                    ViewBag.result = result;
                    return View(topup);
                }
                else if(result==1)
                {

                    ViewBag.result = result;
                    return RedirectToAction("ShowOTP", "RequestOTPS", new { otp = topup.OTP });
                   }
                 else
                {
                    ModelState.AddModelError("Amount", "there is no suffecient mony in the card");
                    ViewBag.result = result;
                    return View(topup);
                }
            }

            else
            {
                ViewBag.result = result;
                return View(topup);
            }

        }



        //public ActionResult GetOTP(string MeterId, int Amount,string SerialNUM)
        //{

        //    if (Session["employee"] == null)
        //    {
        //        return RedirectToAction("index", "Employees");
        //    }
        //    else
        //    {
        //        int rc;
        //        CashCard[] cashCard = CashCard.GetCashCards(new CashCardParameters { SerialNumber = SerialNUM }, out rc);
        //        Topup topup = new Topup(MeterId, Amount, cashCard[0].SerialNumber);
        //        topup.SaveData();
        //        return RedirectToAction("ShowOTP", "RequestOTPS", new { otp = topup.OTP });
        //    }
        //}

        public ActionResult ShowOTP(string otp,string success)
        {

            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {
                if (otp != null && success == null)
                {
                    ViewBag.otp = otp;
                    
                }
                else if (success != null )
                {
                    ViewBag.success = success;
                  
                }
                return View();

            }
        
        }
        public ActionResult Charge_this_otp(int? otp)
        {

            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {
                int rc;
                Topup[] topups = Topup.GetTopups(new TopupParameters { OTP = otp }, out rc);
                topups[0].Charged();


                return RedirectToAction("ShowOTP", "RequestOTPS", new { success = "charged"});
            }
        }
    }
}