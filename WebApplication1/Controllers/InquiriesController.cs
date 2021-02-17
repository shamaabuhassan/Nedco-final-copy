using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class InquiriesController : Controller
    {
        // GET: Inquiries
        public ActionResult Index()
        {
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            return View();
        }

        public ActionResult Charges(string Meterid)
        {
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {
                //ViewBag.MeterId = MeterId;
                Meter meter = new Meter(Meterid);
                return View(meter);
            }
        }

        [HttpPost]
        public ActionResult Charges(Meter meter)
        {
            //int? result = 0;
            
            if (ViewData.ModelState.IsValidField("meter id"))
            { //checking model state

                //check whether id is already exists in the database or not
                int rc ,count=0;
                Meter[] meters = Meter.GetMeters(new MeterParameters { }, out rc);

                foreach(Meter meter1 in meters)
                {
                    if (meter.Meterid == meter1.Meterid)
                    {
                        count = 1;
                    }
                }

                if (count == 0)
                {
                    ModelState.AddModelError("MeterId", "meter not valid");
                    
                    return View(meter);
                }
                else if (count == 1)
                {
                    
                    ViewBag.MeterId = meter.Meterid;
                    return View(meter);
                }
                else
                {
                    return View(meter);
                }
               
            }

            else
            {
                ModelState.AddModelError("MeterId", "meter must entered");
                return View(meter);
            }

        }


        public ActionResult Transfers(string Meterid)
        {
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {

                Meter meter = new Meter(Meterid);
                    return View(meter);
                
            }

            //return View();
        }

        [HttpPost]
        public ActionResult Transfers(Meter meter)
        {
            //int? result = 0;
            if (ViewData.ModelState.IsValidField("meter id"))
            { //checking model state

                //check whether id is already exists in the database or not
                int rc, count = 0;

                
                 Meter[] meters = Meter.GetMeters(new MeterParameters { }, out rc);

                foreach (Meter meter1 in meters)
                {
                    if (meter.Meterid == meter1.Meterid)
                    {
                        count = 1;
                    }
                }

                if (count == 0)
                {
                    ModelState.AddModelError("MeterId", "meter not valid");

                    return View(meter);
                }
                else if (count == 1)
                {

                    Transfer[] transfers = Transfer.GetTransfers(new TransferParameters { MeterId = meter.Meterid}, out rc);

                    Transfer[] transfers2 = Transfer.GetTransfersBySenderOTP(new TransferParameters { MeterId = meter.Meterid }, out rc); //get meter of senderotp

                    if (transfers.Length != 0 && transfers2.Length == 0)
                    {
                        ViewBag.transfers = transfers;
                        ViewBag.MeterId = meter.Meterid;
                        return View(meter);
                    }
                    else if (transfers.Length == 0 && transfers2.Length != 0)
                    {
                        return RedirectToAction("Transfrom", "Transfer", new { MeterId = meter.Meterid });
                    }
                    else if (transfers.Length != 0 && transfers2.Length != 0)
                    {

                        return RedirectToAction("Trans_from_to", "Transfer", new { MeterId = meter.Meterid });
                    }
                    else if (transfers.Length == 0 && transfers2.Length == 0)
                    {
                        int notrans = 1;
                        ViewBag.notrans = notrans;
                        return View(meter);
                    }
                    else
                    {
                        return View(meter);
                    }
                }
                else
                {
                    return View(meter);
                }

            }

            else
            {
              // ModelState.AddModelError("MeterId", "meter must entered");
                return View(meter);
            }

        }




        //[HttpPost]
        //public ActionResult MonthlyCharge(Topup[] topups)
        //{
        //    //int? result = 0;
        //    if (ViewData.ModelState.IsValidField("meter id"))
        //    { //checking model state

        //        //check whether id is already exists in the database or not
               

        //        if (topups.Length == 0)
        //        {
        //            ModelState.AddModelError("MeterId", "meter not valid");
        //            return View(topups);
        //        }
        //        else {
        //            decimal? amount = 0;
        //            decimal? count = 0;
        //            foreach (Topup topup in topups)
        //            {
        //                amount += topup.Amount;
        //                count += 1;
        //            }
        //            ViewBag.amount = amount;
        //            ViewBag.count = count;
        //            return View(topups);
        //        }
        //    }

        //    else
        //    {
        //        //ModelState.AddModelError("MeterId", "meter must entered");
        //        return View(topups);
        //    }

        //}

        public ActionResult MonthlyCharge(DateTime? fromdate, DateTime? todate, string MeterId)
        {
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {
               
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate != null && todate != null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate == null && todate == null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    int both = 0;
                    ViewBag.both = both;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate != null && todate == null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    int to = 0;
                    ViewBag.to = to;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate == null && todate != null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    int from = 0;
                    ViewBag.from = from;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate == null && todate == null)
                {
                   // ModelState.AddModelError("MeterId", "meter id is required");
                    int both = 0;
                    ViewBag.both = both;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate != null && todate == null)
                {
                    //ModelState.AddModelError("MeterId", "meter id is required");
                    int to = 0;
                    ViewBag.to = to;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate == null && todate != null)
                {
                    //ModelState.AddModelError("MeterId", "meter id is required");
                    int from = 0;
                    ViewBag.from = from;
                    return View();

                }
                else if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate != null && todate != null)
                
                {
                    //check whether id is already exists in the database or not
                    int rc, count = 0;
                    Meter[] meters = Meter.GetMeters(new MeterParameters { }, out rc);

                    foreach (Meter meter1 in meters)
                    {
                        if (MeterId == meter1.Meterid)
                        {
                            count = 1;
                        }
                    }

                    if (count == 1)
                    {
                        //Topup[] topupfrom = Topup.GetMonthlyTopups(new TopupParameters { fromdate = fromdate}, out rc);

                        //if (topupfrom.Length == 0) {
                        //    int errorfrom = 0;
                        //    ViewBag.errorfrom = errorfrom;
                        //    return View();
                        //}
                        //Topup[] topupto = Topup.GetMonthlyTopups(new TopupParameters {  todate = todate }, out rc);
                        //if (topupto.Length == 0) {
                        //    int errorto = 0;
                        //    ViewBag.errorto = errorto;
                        //    return View();
                        //}

                        Topup[] topups = Topup.GetMonthlyTopups(new TopupParameters { fromdate = fromdate, todate = todate, MeterId = MeterId }, out rc);

                        if (topups.Length == 0) {
                            int notopups = 0;
                            ViewBag.notopups = notopups;
                            return View();
                        }
                        decimal? amount = 0;
                        decimal? countt = 0;
                        foreach (Topup topup in topups)
                        {
                            
                            amount += topup.Amount;
                            countt += 1;
                        }
                        ViewBag.amount = amount;
                        ViewBag.count = countt;
                        return View();
                    }
                    else
                    {
                        if (MeterId != null)
                        {
                            int valid = 0;
                            ViewBag.valid = valid;
                            return View();
                        }
                        else
                        {
                            return View();
                        }
                    }
                }

                else
                {
                    return View();
                }
            }

        }



        public ActionResult CodeCharging(DateTime? fromdate, DateTime? todate, string MeterId)
        {
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }
            else
            {

                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate != null && todate != null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate == null && todate == null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    int both = 0;
                    ViewBag.both = both;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate != null && todate == null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    int to = 0;
                    ViewBag.to = to;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == false && fromdate == null && todate != null)
                {
                    ModelState.AddModelError("MeterId", "meter id is required");
                    int from = 0;
                    ViewBag.from = from;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate == null && todate == null)
                {
                    // ModelState.AddModelError("MeterId", "meter id is required");
                    int both = 0;
                    ViewBag.both = both;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate != null && todate == null)
                {
                    //ModelState.AddModelError("MeterId", "meter id is required");
                    int to = 0;
                    ViewBag.to = to;
                    return View();

                }
                if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate == null && todate != null)
                {
                    //ModelState.AddModelError("MeterId", "meter id is required");
                    int from = 0;
                    ViewBag.from = from;
                    return View();

                }
                else if (ViewData.ModelState.IsValidField("MeterId") == true && fromdate != null && todate != null)

                {
                    //check whether id is already exists in the database or not
                    int rc, count = 0;
                    Meter[] meters = Meter.GetMeters(new MeterParameters { }, out rc);

                    foreach (Meter meter1 in meters)
                    {
                        if (MeterId == meter1.Meterid)
                        {
                            count = 1;
                        }
                    }

                    if (count == 1)
                    {
                        //Topup[] topupfrom = Topup.GetMonthlyTopups(new TopupParameters { fromdate = fromdate}, out rc);

                        //if (topupfrom.Length == 0) {
                        //    int errorfrom = 0;
                        //    ViewBag.errorfrom = errorfrom;
                        //    return View();
                        //}
                        //Topup[] topupto = Topup.GetMonthlyTopups(new TopupParameters {  todate = todate }, out rc);
                        //if (topupto.Length == 0) {
                        //    int errorto = 0;
                        //    ViewBag.errorto = errorto;
                        //    return View();
                        //}

                        Topup[] topups = Topup.GetCodeChargingTopup(new TopupParameters { fromdate = fromdate, todate = todate, MeterId = MeterId }, out rc);

                        if (topups.Length == 0)
                        {
                            int notopups = 0;
                            ViewBag.notopups = notopups;
                            return View();
                        }
                        decimal? amount = 0;
                        decimal? countt = 0;
                        foreach (Topup topup in topups)
                        {

                            amount += topup.Amount;
                            countt += 1;
                        }
                        ViewBag.amount = amount;
                        ViewBag.count = countt;
                        return View();
                    }
                    else
                    {
                        if (MeterId != null)
                        {
                            int valid = 0;
                            ViewBag.valid = valid;
                            return View();
                        }
                        else
                        {
                            return View();
                        }
                    }
                }

                else
                {
                    return View();
                }
            }

        }


    }
}