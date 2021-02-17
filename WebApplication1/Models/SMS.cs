using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace WebApplication1.Models
{
    public class SMSParameters
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public String To_number { get; set; }
        public DateTime Date { get; set; }
        public string Msg { get; set; }

    }
    public class SMS
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public String To_number { get; set; }
        public DateTime Date { get; set; }
       
        public string Msg { get; set; }


        public SMS() { }

        public SMS(string id, string status, string to_number, DateTime date,  string msg)
        {
            this.Id = id;
            this.Status = status;
            this.To_number = to_number;
            this.Date = date;
            this.Msg = msg;
        }

        public int SaveData()
        {
            int result = 0;
            Date= DateTime.Now; 
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "SaveSMS";


                if (Id != null) cmd.Parameters.AddWithValue("id", Id);
                if (Status != null) cmd.Parameters.AddWithValue("status", Status);
                if (To_number != null) cmd.Parameters.AddWithValue("to_number", To_number);
                if (Date != null) cmd.Parameters.AddWithValue("date", Date);
                if (Msg != null) cmd.Parameters.AddWithValue("msg", Msg);


                SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                resultParam.Direction = ParameterDirection.InputOutput;



                int c = cmd.ExecuteNonQuery();


                result = Convert.ToInt32(resultParam.Value);
                cmd.Connection.Close();


            }
            return result;
        }
         
        public string Send()
        {
            using (WebClient client = new WebClient())
            {
                string response= client.DownloadString($"http://sms.htd.ps/API/SendSMS.aspx?id=eadaaac72e504a1f6e0b2a7a5cb60dc9&sender=easycharge1&to=97{To_number}&msg={Msg}&mode=1");
                try
                {
                    string[] ss = response.Split((new char[] { '|' }));
                   string[] sss = ss[1].Split((new char[] { ':' }));

                    Status = ss[0];
                    Id= sss[1];

                    SaveData();

                }
                catch (Exception)
                {}
                return Status;
            }
        }

        //public String Send(string telephone, string message)
        //{
        //    string Telephone = $"97{telephone}";
        //    string response;
        //    string[] ss;
        //    string[] sss;

        //    using (WebClient client = new WebClient())
        //    {
        //        response = client.DownloadString($"http://sms.htd.ps/API/SendSMS.aspx?id=eadaaac72e504a1f6e0b2a7a5cb60dc9&sender=easycharge1&to={Telephone}&msg={message}&mode=1");
        //        //"OK|970123456789:serial"
        //        //sms.Id=

        //       ss = response.Split((new char[] { '|' }));
        //       sss = ss[1].Split((new char[] { ':' }));


        //        if (ss[0] == "OK")
        //        {
        //            SMS sms = new SMS();
        //            sms.Status = ss[0];
        //            sms.To_number = Convert.ToInt32(sss[0]);
        //            sms.SMS_Id = Convert.ToInt32(sss[1]);
        //            sms.SaveData();
        //        }
        //    }
        //    return ss[0];
        //}
    }
        
}