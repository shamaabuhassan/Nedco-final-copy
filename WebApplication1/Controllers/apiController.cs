using Antlr.Runtime.Tree;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class apiController : Controller
    {
        // GET: api
        public ActionResult CheckLogin(string Username, string Password)
        {
            Customer customer = Customer.CheckLogin(Username, Password);
            if (customer != null)
            {
                return Content(JsonConvert.SerializeObject(new { result = "success", data = customer }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { result = "error" }));
            }

        }

        public ActionResult RequestOTP(string MeterId, int? Amount, string SerialNumber, int customerid)
        //this should reach the website button which request OTP 

        {
            int rc;
            Customer customer = new Customer(customerid);
            Meter[] meters = Meter.GetMeters(new MeterParameters { Meterid = MeterId }, out rc);//user of meter
            if (meters.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "invalid-meter"}));
            }

            CashCard[] cashCards = CashCard.GetCashCards(new CashCardParameters { SerialNumber = SerialNumber }, out rc);//cash card information
            if (cashCards.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "invalid-cashcard" }));
            }

            Customer customer1=null;
            if (meters != null)
            {
                 customer1 = new Customer(meters[0].UserId.Value);//user id of meter id entered
            }

            if (customer.Id == meters[0].UserId && customer.CardId == cashCards[0].Id)//for himself from his card
            {
                SMS sms = new SMS();
                sms.To_number = customer.Telephone;
                sms.Msg = $"أهلا وسهلا بك أنت تقوم بشحن {Amount} الى عدادك الان";
               string status = sms.Send();
                
              if(status == "OK")
                {
                    Topup topup = new Topup(null, MeterId, Amount, cashCards[0].SerialNumber);
                    
                    int result;
                    result = topup.SaveDataForAPP();
                    if (result == 1)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topup }));
                    }
                    else 
                    {
                        
                        return Content(JsonConvert.SerializeObject(new { result = "insuffecient-balance" ,data= cashCards[0].Amount}));
                    }
                }
                
            }

            else if (customer.Id == meters[0].UserId && customer.CardId != cashCards[0].Id) //for himself from another card
            {

                Customer[] customer2 = Customer.GetCustomers(new CustomerParameters { CardId = cashCards[0].Id }, out rc);
                SMS sms = new SMS();
                sms.To_number = customer2[0].Telephone;
                sms.Msg = $" يحاول {customer.Name} شحن عداده باستخدام البطاقة الخاصة بك بقيمة {Amount}";
             string status = sms.Send();
                
               if (status == "OK")
                {
                    Topup topup = new Topup(null, MeterId, Amount, cashCards[0].SerialNumber);
                    int result;
                    result = topup.SaveDataForAPP();
                    if (result == 1)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topup }));
                    }
                    else
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "insuffecient-balance", data = cashCards[0].Amount }));
                    }
                }
            }

            else if (customer.Id != meters[0].UserId && customer.CardId == cashCards[0].Id)//for another from his card
            {
                SMS sms = new SMS();
                sms.To_number = customer1.Telephone;
                sms.Msg = $"يحاول {customer.Name} شحن عدادك بقيمة {Amount}";
               string status = sms.Send();
                

                SMS sms1 = new SMS();
                sms1.To_number = customer.Telephone;
                sms1.Msg = $"يحاول {customer1.Name} شحن عداده باستخدام بطاقتك بقيمة {Amount}";
              string status1 = sms1.Send();
                
                if (status == "OK" && status1 == "OK")
                {

                    Topup topup = new Topup(null, MeterId, Amount, cashCards[0].SerialNumber);
                    int result;
                    result = topup.SaveDataForAPP();
                    if (result == 1)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topup }));
                    }
                    else 
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "insuffecient-balance", data = cashCards[0].Amount }));
                    }
                }

            }


            else if (customer.Id != meters[0].UserId && customer.CardId != cashCards[0].Id && customer1.CardId != cashCards[0].Id)//for another from another card
            {
                int rrc;
                Customer[] customer2 = Customer.GetCustomers(new CustomerParameters { CardId = cashCards[0].Id }, out rrc);

                SMS sms = new SMS();
                sms.To_number = customer2[0].Telephone;
                sms.Msg = $"يحاول {customer1.Name} شحن عداده بقيمة {Amount} باستخدام بطاقتك";
             string status = sms.Send();
                

                SMS sms1 = new SMS();
                sms1.To_number = customer1.Telephone;
                sms1.Msg = $"يحاول {customer.Name} شحن عدادك بقيمة {Amount} باستخدام بطاقة {customer2[0].Name}";
              string status1 = sms1.Send();
                
               if (status == "OK" && status1 == "OK")
                {
                    Topup topup = new Topup(null, MeterId, Amount, cashCards[0].SerialNumber);
                    int result;
                    result = topup.SaveDataForAPP();
                    if (result == 1)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topup }));
                    }
                    else 
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "insuffecient-balance", data = cashCards[0].Amount }));
                    }
                }
            }
            else if (customer.Id != meters[0].UserId && customer.CardId != cashCards[0].Id && customer1.CardId == cashCards[0].Id)//for another from the another card
            {

                SMS sms = new SMS();
                sms.To_number = customer1.Telephone;
                sms.Msg = $"يحاول {customer.Name} شحن عدادك باستخدام بطاقتك";
                string status = sms.Send();
             

                SMS sms1 = new SMS();
                sms1.To_number = customer.Telephone;
                sms1.Msg = $"أهلا وسهلا بك أنت تحاول الان شحن عداد {customer1.Name} باستخدام بطاقته {customer1.CardId}";
                string status1 = sms1.Send();
               
                

               if (status == "OK" && status1 == "OK")
                {
                    Topup topup = new Topup(null, MeterId, Amount, cashCards[0].SerialNumber);
                    int result;
                    result = topup.SaveDataForAPP();
                    if (result == 1)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topup }));
                    }
                    else
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "insuffecient-balance", data = cashCards[0].Amount }));
                    }
                }

            }

            return Content(JsonConvert.SerializeObject(new { result = "error" }));
        }



        public ActionResult ChargeMeterFromApp(int? OTP, int customerid)//this should reach the website button which charge the meter 
        {

            int rc;
            Customer customer = new Customer(customerid);
            Topup[] topup = Topup.GetTopups(new TopupParameters { OTP = OTP }, out rc);
           
            if (topup[0].Status == "0")
            {

                Meter[] meters = Meter.GetMeters(new MeterParameters { Meterid = topup[0].MeterId }, out rc);

                if (customer.Id == meters[0].UserId)
                {
                    SMS sms = new SMS();
                    sms.To_number = customer.Telephone;
                    sms.Msg = $"  أهلا وسهلا بك أنت تحاول الان شحن عدادك باستخدام موقعنا في الشركة ورقم الكود الذي يريد شحنه هو {OTP}";
                   string status = sms.Send();
                    

                   if (status == "OK")
                    {
                        topup[0].Charged();

                        return Content(JsonConvert.SerializeObject(new { result = "success" }));
                    }
                }

                else if (customer.Id != meters[0].UserId)
                {
                    Customer customer1 = new Customer(meters[0].UserId.Value);
                    SMS sms = new SMS();
                    sms.To_number = customer1.Telephone;
                    sms.Msg = $"يحاول {customer.Name} شحن عدادك باستخدام موقعنا في الشركة ورقم الكود الذي يريد شحنه هو {OTP}";
                    string status = sms.Send();
                  
                   if (status == "OK")
                    {
                        topup[0].Charged();

                        return Content(JsonConvert.SerializeObject(new { result = "success" }));
                    }
                }

            }
            return Content(JsonConvert.SerializeObject(new { result = "error" }));


        }

        public ActionResult ChargeFromMainPage(int? OTP,string Meterid, int customerid)//chargr from mainpage
        {

            int rc;
            Customer customer = new Customer(customerid);

            Meter[] meter = Meter.GetMeters(new MeterParameters { Meterid = Meterid }, out rc);//meter information of entered meter
            if (meter.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "invalid-meter" }));
            }

            Topup[] topup = Topup.GetTopups(new TopupParameters { OTP = OTP }, out rc);// top up that has this otp
            if (topup.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "invalid-otp" }));
            }


            if (topup[0].Status == "0")
            {
                Meter[] meters = Meter.GetMeters(new MeterParameters { Meterid = topup[0].MeterId }, out rc);//meter information of otp enetered 

                if (customer.Id == meters[0].UserId && customer.Id == meter[0].UserId&&meter[0].Meterid==meters[0].Meterid)//customer meter as meter of otp as meter id entered
                {
                    SMS sms = new SMS();
                    sms.To_number = customer.Telephone;
                    sms.Msg = $"  أهلا وسهلا بك أنت تحاول الان شحن عدادك باستخدام موقعنا في الشركة ورقم الكود الذي يريد شحنه هو {OTP}";
                    string status = sms.Send();
                   

                    if (status == "OK")
                    {
                        topup[0].Charged();
                        return Content(JsonConvert.SerializeObject(new { result = "success",data=topup[0].Amount }));
                    }

                }

                else if (customer.Id != meters[0].UserId && customer.Id == meter[0].UserId && meter[0].Meterid == meters[0].Meterid)//customer id not as  meter id of otp and meter id entered as customer id
                {
                        return Content(JsonConvert.SerializeObject(new { result = "not-your-meterid" }));
                  

                }
                else if (customer.Id != meters[0].UserId && customer.Id != meter[0].UserId && meter[0].Meterid == meters[0].Meterid)
                    //if the customer try to charge to another 
                {
                    //if (meters[0].UserId == meter[0].UserId)//if the meter of otp as meter id entered
                    //{
                        Customer customer1 = new Customer(meters[0].UserId.Value);
                        SMS sms = new SMS();
                        sms.To_number = customer1.Telephone;
                        sms.Msg = $"يحاول {customer.Name} شحن عدادك باستخدام موقعنا في الشركة ورقم الكود الذي يريد شحنه هو {OTP}";
                        string status = sms.Send();
                    
                      if (status == "OK")
                        {
                            topup[0].Charged();
                            
                            return Content(JsonConvert.SerializeObject(new { result="success" }));
                      } 
                }
                else if (meter[0].Meterid != meters[0].Meterid)
                {
                    return Content(JsonConvert.SerializeObject(new { result = "not-compatible" }));
                }

            }
            return Content(JsonConvert.SerializeObject(new { result = "otp-with-status-1" }));

        }

        public ActionResult ReturnOTPFromMainPage(string Meterid, int customerid)
        {
            int rc;
            Customer customer = new Customer(customerid);
            Meter[] meters = Meter.GetMeters(new MeterParameters { Meterid = Meterid }, out rc);
            //if (meters.Length == 0)
            //{
            //    return Content(JsonConvert.SerializeObject(new { result = "meter-not-valid" }));
            //}

            if (customer.Id == meters[0].UserId)
            {
                SMS sms = new SMS();
                sms.To_number = customer.Telephone;
                sms.Msg = $"أهلا وسهلا بكك أنت تحاول الان استرجاع الكود الغير مشحون الخاص بك";
                string status = sms.Send();

                if (status == "OK")
                {
                    Topup[] topups = Topup.GetTopups(new TopupParameters { MeterId = Meterid, Status = "0" }, out rc);
                    return Content(JsonConvert.SerializeObject(new { result = "success", data = topups }));
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new { result = "error" }));
                }


            }

            else if (customer.Id != meters[0].UserId)
            {

                Customer customer1 = new Customer(meters[0].UserId.Value);

                SMS sms = new SMS();
                sms.To_number = customer1.Telephone;
                sms.Msg = $"يحاول {customer.Name} استرجاع الكود الغير مشحون الخاص بك";
                 string status = sms.Send();

                if (status == "OK")
                {
                    Topup[] topups = Topup.GetTopups(new TopupParameters { MeterId = Meterid, Status = "0" }, out rc);
                    return Content(JsonConvert.SerializeObject(new { result = "success", data = topups }));
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new { result = "error" }));
                }

            }

            else
            {
                 return Content(JsonConvert.SerializeObject(new { result = "error" }));
            }

        }



        public ActionResult TransferOTP(int? SenderOTP, string MeterId,int Amount, int customerid)
        {

            int rc;
            Customer customer = new Customer(customerid);
            Topup[] topups = Topup.GetTopups(new TopupParameters { OTP = SenderOTP }, out rc);//get senderotp meterid 
            if (topups.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "otp-not-valid" }));
            }

            if (topups.Length != 0 && topups[0].Status=="1")
            {
                return Content(JsonConvert.SerializeObject(new { result = "otp-charged" }));
            }
            Meter[] meters = Meter.GetMeters(new MeterParameters { Meterid = topups[0].MeterId }, out rc);//get meterid of otp
            if (meters.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "meter-ofotp-not-valid" }));
            }

            Meter[] meters1 = Meter.GetMeters(new MeterParameters { Meterid = MeterId }, out rc);// get userid will take the amount
            if (meters1.Length == 0)
            {
                return Content(JsonConvert.SerializeObject(new { result = "meter-response-not-valid" }));
            }

            Customer customer1 = new Customer(meters1[0].UserId.Value);//get customer info


            if (customer.Id == meters[0].UserId)//otp for customer
            {

                SMS sms = new SMS();
                sms.To_number = customer.Telephone;
                sms.Msg = $"أهلا وسلا بك في تطبيقنا أنت تحاول الان تحويل قيمة {Amount} الى حساب {customer1.Name} ";
                string status = sms.Send();


                SMS sms1 = new SMS();
                sms1.To_number = customer1.Telephone;
                sms1.Msg = $"يحاول {customer.Name} تحويل قيمة {Amount} الى عدادك";
               string status1 = sms1.Send();

               if (status == "OK" && status1 == "OK")
                {
                    Transfer transfer = new Transfer(null, SenderOTP, MeterId, Amount);

                    Topup[] topupp = new Topup[] { };

                    topupp = transfer.SaveData();
                    if (topupp.Length != 0)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topupp }));
                    }
                    else
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "insufficient-amount", data= topups[0].Amount}));
                    }


                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new { result = "error" }));
                }


            }

            else if (customer.Id != meters[0].UserId)//otp not for user
            {
                Customer customer2 = new Customer(meters[0].UserId.Value);//get customer of otp
                SMS sms = new SMS();
                sms.To_number = customer2.Telephone;
                sms.Msg = $"أهلا وسلا بك في تطبيقنا أنت تحاول الان تحويل قيمة {Amount} الي حساب {customer1.Name} ";
              string status = sms.Send();


                SMS sms1 = new SMS();
                sms1.To_number = customer1.Telephone;
                sms1.Msg = $"يحاول {customer2.Name} تحويل قيمة {Amount} الى عدادك";
                string status1 = sms1.Send();

               if (status == "OK" && status1 == "OK")
                {
                    Transfer transfer = new Transfer(null, SenderOTP, MeterId, Amount);

                    Topup[] topupp = new Topup[] { };

                    topupp = transfer.SaveData();
                    if (topupp.Length != 0)
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "success", data = topupp }));
                    }
                    else
                    {
                        return Content(JsonConvert.SerializeObject(new { result = "insufficient-amount", data = topups[0].Amount }));
                    }

                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new { result = "error" }));
                }

            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { result = "error" }));
            }
        }


        public ActionResult ChargeHist(DateTime fromdate, DateTime todate, int customerid)
        {
           
            int rc;
            Customer customer = new Customer(customerid);
            Meter[] meters = Meter.GetMeters(new MeterParameters { UserId = customer.Id }, out rc);

            List<Topup> topups1 = new List<Topup>();

            foreach (Meter meter in meters)
            {

                Topup[] topups = Topup.GetMonthlyTopups(new TopupParameters { fromdate = fromdate, todate = todate, MeterId = meter.Meterid }, out rc);
               
                  
                    foreach (Topup topup in topups)
                    {
                        topups1.Add(topup);
                    }
                
                
            }

           
                decimal? amount = 0;
                decimal? counter = 0;
                foreach (Topup topup in topups1)
                {
                    amount += topup.Amount;
                    counter += 1;
                }
                //decimal? result =decimal.Round( amount.Value/count.Value,2);

                return Content(JsonConvert.SerializeObject(new { result = "success", data = amount }));
            
        }


        //public ActionResult ChargingHist(int customerid)
        //{
        //    int rc;
        //    Customer customer = new Customer(customerid);
        //    Meter[] meters = Meter.GetMeters(new MeterParameters { UserId = customer.Id }, out rc);
        //    //  Topup[] topups = null;
        //    List<Topup> topups = new List<Topup>();
        //    //int c = 0;
        //    foreach (Meter meter in meters) {
        //        string meterid = meter.Meterid;
        //        Topup[] topup = Topup.GetTopups(new TopupParameters { MeterId = meterid }, out rc);
        //        foreach (Topup topup1 in topup)
        //        {
        //            topups.Add(topup1);
        //        }
        //      //  c++;
        //    }
        //    return Content(JsonConvert.SerializeObject(new { result = "success", data = topups }));
        //}


        public ActionResult TransferHist(int customerid, DateTime fromdate, DateTime todate)
        {
            
            int rc;
            Customer customer = new Customer(customerid);
            Meter[] meters = Meter.GetMeters(new MeterParameters { UserId = customer.Id }, out rc);
            decimal? amount = 0;
            List<Transfer> transfers1 = new List<Transfer>();
            
            foreach (Meter meter in meters)
            {
                string meterid = meter.Meterid;
                Transfer[] transfer = Transfer.GetTransferswithdate(new TransferParameters { MeterId = meter.Meterid , fromdate=fromdate,todate=todate}, out rc);
              
                    foreach (Transfer transfer1 in transfer)
                    {
                        transfers1.Add(transfer1);
                        amount += transfer1.Amount;
                    }
                
            }
         
            List<Transfer> transfers2 = new List<Transfer>();
            foreach (Meter meter in meters)
            {
                string meterid = meter.Meterid;
                Transfer[] transfert = Transfer.GetTransfersBySenderOTPwithdate(new TransferParameters { MeterId = meter.Meterid, fromdate = fromdate, todate = todate }, out rc);
                foreach (Transfer transfer2 in transfert)
                {
                    transfers2.Add(transfer2);
                    amount += transfer2.Amount;
                }
                
            }
            if (transfers1 != null && transfers2 == null)
            {
                return Content(JsonConvert.SerializeObject(new { result = "success", data =/*transfers1,*/amount }));
            }
            if (transfers1 == null && transfers2 != null)
            {
                transfers1.Clear();
                transfers1.AddRange(transfers2);
                return Content(JsonConvert.SerializeObject(new { result = "success", data = /*transfers1,*/amount}));
            }
            if (transfers1 != null && transfers2 != null)
            {

                transfers1.AddRange(transfers2);
                return Content(JsonConvert.SerializeObject(new { result = "success", data = /*transfers1,*/amount}));
            }
            return Content(JsonConvert.SerializeObject(new { result = "error"}));
        }

        public ActionResult ChangePassword(string password, int? customerid)
        {
            int result = 0;
            Customer customer = new Customer(customerid, null, null, null, null,null, null , null, password, null);
            result=customer.AlterPassword();
            if (result == 1)
            {
                return Content(JsonConvert.SerializeObject(new { result = "success" }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { result = "error" }));
            }
            
        }

        public ActionResult CheckMeter(string meterid)
        {
            int result = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "CheckMeter";

                cmd.Parameters.AddWithValue("@meterid", meterid);

                SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                resultParam.Direction = ParameterDirection.InputOutput;

               

                int c = cmd.ExecuteNonQuery();


                result = Convert.ToInt32(resultParam.Value);
                cmd.Connection.Close();

            }

            if (result == 1)
            {
                return Content(JsonConvert.SerializeObject(new { result = "success" }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { result = "meter-not-valid" }));
            }
        }
    }
}
