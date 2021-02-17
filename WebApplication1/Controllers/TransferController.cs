using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TransferController : Controller
    {
        // GET: Transfer
        public ActionResult Index(int ?id)
        {
            ViewBag.id = id;
            return View();
        }
        
        public ActionResult Search(string MeterId)
        {
            int rc;
            if (MeterId != null)
            {
                Transfer[] transfers = Transfer.GetTransfers(new TransferParameters { MeterId = MeterId }, out rc);
                ViewBag.transfers = transfers;
            }
            return View();
        }

        public ActionResult Save(int? id, int? senderOTP, string meterId, decimal? amount)
        {
            int rc;
            Topup[] topup = Topup.GetTopups(new TopupParameters {OTP =senderOTP }, out rc);//get senderotp meterid 
            Meter[] meters = Meter.GetMeters(new MeterParameters {Meterid=topup[0].MeterId}, out rc);//get meterid customer
            Customer customer = (Session["customer"] as Customer);//check if senderotp customer as session customer
            Meter[] meters1 = Meter.GetMeters(new MeterParameters { Meterid =meterId }, out rc);// get userid will take the amount
            Customer customer1 = new Customer(meters1[0].UserId.Value);//get customer info


            if (Session["customer"] != null)
            {
                if (customer.Id == meters[0].UserId) {

                    SMS sms = new SMS();
                    sms.To_number = customer.Telephone;
                    sms.Msg = $"أهلا وسلا بك في تطبيقنا أنت تحاول الان تحويل قيمة {amount} الي حساب {customer1.Name} ";
                    string status = sms.Send();
                    sms.SaveData();

                    SMS sms1 = new SMS();
                    sms.To_number = customer1.Telephone;
                    sms.Msg = $"يحاول {customer.Name} تحويل قيمة {amount} الى عدادك";
                    string status1 = sms1.Send();
                    sms1.SaveData();

                    if (status == "OK" && status1 == "OK")
                    {
                        Transfer transfer = new Transfer(id, senderOTP, meterId, amount);
                       
                        Topup[] result = null;
                        result = transfer.SaveData();
                        
                        ViewBag.result = result;

                    }
                }
             
                else if(customer.Id != meters[0].UserId)
                {
                   Customer customer2=new Customer (meters[0].UserId.Value);
                    SMS sms = new SMS();
                    sms.To_number = customer2.Telephone;
                    sms.Msg = $"أهلا وسلا بك في تطبيقنا أنت تحاول الان تحويل قيمة {amount} الي حساب {customer1.Name} ";
                    string status = sms.Send();
                    sms.SaveData();

                    SMS sms1 = new SMS();
                    sms.To_number = customer1.Telephone;
                    sms.Msg = $"يحاول {customer2.Name} تحويل قيمة {amount} الى عدادك";
                    string status1 = sms1.Send();
                    sms1.SaveData();

                    if (status == "OK" && status1 == "OK")
                    {
                        Transfer transfer = new Transfer(id, senderOTP, meterId, amount);
                        Topup[] result = null;
                        result = transfer.SaveData();

                        ViewBag.result = result;

                    }

                }
                return View();
            }
            else
            {
                return RedirectToAction("Save", "Transfer");
            }

            
        }

        public ActionResult transferrequests()
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

        public ActionResult Transfrom( string MeterId )
        {
            int rc;
            Transfer[] transfers2 = Transfer.GetTransfersBySenderOTP(new TransferParameters { MeterId = MeterId }, out rc);
            ViewBag.transfers2 = transfers2;
            return View();
        }

        public ActionResult Trans_from_to(string MeterId)
        {
            int rc;
            Transfer[] transfers2 = Transfer.GetTransfersBySenderOTP(new TransferParameters { MeterId = MeterId }, out rc);

            Transfer[] transfers = Transfer.GetTransfers(new TransferParameters { MeterId = MeterId }, out rc);
            ViewBag.transfers2 = transfers2;
            ViewBag.transfers = transfers;
            return View();
        }

        
        public ActionResult TransferHist()
        {
            return View();
        }
    }
}