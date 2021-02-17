using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebApplication1.Models
{
    public class TopupParameters
    {
        public int? Id { get; set; }
        public string MeterId { get; set; }
        public decimal? Amount { get; set; }
        public string SerialNUM { get; set; }
        public int? OTP { get; set; }
        public DateTime? ChargeDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string Status { get; set; }

        public DateTime? fromdate { get; set; }
        public DateTime? todate { get; set; }

    }


    public class Topup
    {
        public int? Id { get; set; }

        [Display (Name ="meterid")]
        [Required (ErrorMessage ="the meter id is required ")]
        public string MeterId { get; set; }

        [Required(ErrorMessage = "the amount  is required")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "the serial number is required")]
        public string SerialNUM { get; set; }
        public int? OTP { get; set; }
        public DateTime? ChargeDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string Status { get; set; }

        public Topup() { }


        public void Charged()
        {
            ActivationDate = DateTime.Now;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "Charged";


                if (this.MeterId != null) cmd.Parameters.AddWithValue("meter_id", this.MeterId);
                if (this.Amount != null) cmd.Parameters.AddWithValue("amount", this.Amount);
                if (this.SerialNUM != null) cmd.Parameters.AddWithValue("card_serialnum", this.SerialNUM);
                if (this.OTP != null) cmd.Parameters.AddWithValue("otp", this.OTP);
                if (this.ChargeDate != null) cmd.Parameters.AddWithValue("chargeDate", this.ChargeDate);
                if (this.ActivationDate != null) cmd.Parameters.AddWithValue("activationDate", ActivationDate);
                if (this.Status != null) cmd.Parameters.AddWithValue("status", this.Status);

                int c = cmd.ExecuteNonQuery();
                cmd.Connection.Close();

            }
        }
        public Topup(int? id)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTopupsByID";
                cmd.Parameters.AddWithValue("@ID", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    if (r["meter_id"] != DBNull.Value) this.MeterId = Convert.ToString(r["meter_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);
                    if (r["card_serialnum"] != DBNull.Value) this.SerialNUM = Convert.ToString(r["card_serialnum"]);
                    if (r["otp"] != DBNull.Value) this.OTP = Convert.ToInt32(r["otp"]);
                    // this.OTP = Convert.ToString(r["otp"]);
                    if (r["chargeDate"] != DBNull.Value) this.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
                    if (r["activationDate"] != DBNull.Value) this.ActivationDate = Convert.ToDateTime(r["activationDate"]);
                    this.Status = Convert.ToString(r["status"]);
                    cmd.Connection.Close();
                }
            }
        }

        //public Topup(string otp)
        //{

        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = new SqlConnection(cstr.con);
        //        cmd.Connection.Open();
        //        cmd.CommandText = "GetTopupsByOTP";
        //        cmd.Parameters.AddWithValue("@otp", otp);

        //        SqlDataReader r = cmd.ExecuteReader();
        //        if (r.HasRows)
        //        {
        //            r.Read();
        //            if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
        //            if (r["meter_id"] != DBNull.Value) this.MeterId = Convert.ToString(r["meter_id"]);
        //            if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);
        //            if (r["card_serialnum"] != DBNull.Value) this.SerialNUM = Convert.ToString(r["card_serialnum"]);
        //            if (r["otp"] != DBNull.Value) this.OTP = Convert.ToInt32(r["otp"]);
        //            //this.OTP = Convert.ToString(r["otp"]);
        //            if (r["chargeDate"] != DBNull.Value) this.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
        //            if (r["activationDate"] != DBNull.Value) this.ActivationDate = Convert.ToDateTime(r["activationDate"]);
        //            this.Status = Convert.ToString(r["status"]);
        //            cmd.Connection.Close();
        //        }
        //    }
        //}

        //constructor

        public Topup(int? id, string meterId, decimal? amount, string serialnum)
        {
            this.Id = id;
            this.MeterId = meterId;
            this.Amount = amount;
            this.SerialNUM = serialnum;

        }
        public Topup(string meterId, decimal? amount, string serialnum)
        {

            this.MeterId = meterId;
            this.Amount = amount;
            this.SerialNUM = serialnum;

        }


        public void Delete()
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "DeleteTopup";
                cmd.Parameters.AddWithValue("id", this.Id);

                cmd.ExecuteNonQuery();


            }

        }

        //public string GenerateRandom()
        //{
        //    Random generator = new Random();
        //    String r = generator.Next(100000, 999999).ToString("D6");
        //    return r;
        //}
        public int SaveData()
        {
            int rc, count4 = 0;
            CashCard cashCard=new CashCard();
            CashCard[] cashCards = CashCard.GetCashCards(new CashCardParameters { SerialNumber = SerialNUM }, out rc);
           
            if (cashCards.Length == 0)
            {
                count4 = 1;  
            }

            else
            {
                 cashCard = cashCards[0];
            }
            CashCard[] cashCards1 = CashCard.GetCashCards(new CashCardParameters { }, out rc);
            int result = 0;
            int count = 0, count2 = 0, count3 = 1;

            Meter[] meters = Meter.GetMeters(new MeterParameters {Meterid  = MeterId },out rc);//user for meter
            Customer customer = new Customer(meters[0].UserId.Value);//cardid for meter user

            foreach(CashCard cashCard1 in cashCards1)//card exist
            {
                if (SerialNUM == cashCard1.SerialNumber &&count4==0)
                {
                    count3 = 0;
                }
            }

            if (cashCard.Id != customer.CardId && count3==0 && count4 == 0)//card not for meter
            {
                count2 = 1;
            }


            if (cashCard.Amount < Amount &&  count4 == 0)//not suffecient amount
            {
                count = 1;
            }


            if (count == 0 && count2 == 0 && count3 == 0 && count4 == 0)
            {
                //OTP = 0;
                ChargeDate = DateTime.Now;
                Status = "0";

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = new SqlConnection(cstr.con);
                    cmd.Connection.Open();
                    cmd.CommandText = "SaveTopupData";


                    if (MeterId != null) cmd.Parameters.AddWithValue("meter_id", MeterId);
                    if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);
                    if (SerialNUM != null) cmd.Parameters.AddWithValue("card_serialnum", cashCard.SerialNumber);
                    // if (OTP != null) cmd.Parameters.AddWithValue("otp", OTP);
                    if (ChargeDate != null) cmd.Parameters.AddWithValue("chargeDate", ChargeDate);
                    if (ActivationDate != null) cmd.Parameters.AddWithValue("activationDate", ActivationDate);
                    if (Status != null) cmd.Parameters.AddWithValue("status", Status);

                    SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                    idParam.Direction = ParameterDirection.InputOutput;
                    idParam.Value = this.Id;

                    SqlParameter otpParam = cmd.Parameters.Add("@otp", SqlDbType.Int);
                    otpParam.Direction = ParameterDirection.InputOutput;

                    SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                    resultParam.Direction = ParameterDirection.InputOutput;


                    //try
                    // {
                    int c = cmd.ExecuteNonQuery();
                    //}
                    //catch (Exception ex)
                    //{
                    //    return 0;
                    //}

                    this.Id = Convert.ToInt32(idParam.Value);
                    this.OTP = Convert.ToInt32(otpParam.Value);
                    result = Convert.ToInt32(resultParam.Value);
                    cmd.Connection.Close();

                }
                decimal? amount = cashCard.Amount - Amount;
                CashCard cash = new CashCard(cashCard.Id, amount, cashCard.SerialNumber);
                cash.SaveData();
            }
            else if(count==1)//un sufficient amount
            {
                result = Convert.ToInt32(cashCard.Amount);
            }
           
            else if (count3 == 0 && count2==1)//not for meter
            {
                result = 4;
            }
            else if( count4 == 1)//card not valid
            {
                result = 2;
            }
            return result;

        }




        public static Topup[] GetMonthlyTopups(TopupParameters parameters, out int rowsCount)
        {
            List<Topup> l = new List<Topup>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetMonthlyTopups";

                cmd.Parameters.AddWithValue("@fromdate", parameters.fromdate);
                cmd.Parameters.AddWithValue("@todate", parameters.todate);
                cmd.Parameters.AddWithValue("@meter_id", parameters.MeterId);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Topup c = new Topup();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

                        if (r["card_serialnum"] != DBNull.Value) c.SerialNUM = Convert.ToString(r["card_serialnum"]);
                        if (r["otp"] != DBNull.Value) c.OTP = Convert.ToInt32(r["otp"]);
                        // if (r["otp"] != DBNull.Value) c.OTP = Convert.ToString(r["otp"]);
                        if (r["chargeDate"] != DBNull.Value) c.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
                        if (r["activationDate"] != DBNull.Value) c.ActivationDate = Convert.ToDateTime(r["activationDate"]);
                        if (r["status"] != DBNull.Value) c.Status = Convert.ToString(r["status"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }



        public static Topup[] GetCodeChargingTopup(TopupParameters parameters, out int rowsCount)
        {
            List<Topup> l = new List<Topup>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetMonthlyTopups";

                cmd.Parameters.AddWithValue("@fromdate", parameters.fromdate);
                cmd.Parameters.AddWithValue("@todate", parameters.todate);
                cmd.Parameters.AddWithValue("@meter_id", parameters.MeterId);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Topup c = new Topup();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

                        if (r["card_serialnum"] != DBNull.Value) c.SerialNUM = Convert.ToString(r["card_serialnum"]);
                        if (r["otp"] != DBNull.Value) c.OTP = Convert.ToInt32(r["otp"]);
                        // if (r["otp"] != DBNull.Value) c.OTP = Convert.ToString(r["otp"]);
                        if (r["chargeDate"] != DBNull.Value) c.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
                        if (r["activationDate"] != DBNull.Value) c.ActivationDate = Convert.ToDateTime(r["activationDate"]);
                        if (r["status"] != DBNull.Value) c.Status = Convert.ToString(r["status"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }

        public static Topup[] GetTopups(TopupParameters parameters, out int rowsCount)
        {
            List<Topup> l = new List<Topup>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTopups";

                cmd.Parameters.AddWithValue("@card_serialnum", parameters.SerialNUM);
                cmd.Parameters.AddWithValue("@status", parameters.Status);
                cmd.Parameters.AddWithValue("@otp", parameters.OTP);
                cmd.Parameters.AddWithValue("@meter_id", parameters.MeterId);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Topup c = new Topup();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);
                        if (r["card_serialnum"] != DBNull.Value) c.SerialNUM = Convert.ToString(r["card_serialnum"]);
                        if (r["otp"] != DBNull.Value) c.OTP = Convert.ToInt32(r["otp"]);
                        // if (r["otp"] != DBNull.Value) c.OTP = Convert.ToString(r["otp"]);
                        if (r["chargeDate"] != DBNull.Value) c.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
                        if (r["activationDate"] != DBNull.Value) c.ActivationDate = Convert.ToDateTime(r["activationDate"]);
                        if (r["status"] != DBNull.Value) c.Status = Convert.ToString(r["status"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }


        public int SaveDataTransfered()
        {
            int result = 0;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = new SqlConnection(cstr.con);
                    cmd.Connection.Open();
                    cmd.CommandText = "SaveTopupTransfer";


                    if (MeterId != null) cmd.Parameters.AddWithValue("meter_id", MeterId);
                    if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);
                    if (SerialNUM != null) cmd.Parameters.AddWithValue("card_serialnum", SerialNUM);
                    // if (OTP != null) cmd.Parameters.AddWithValue("otp", OTP);
                    //if (ChargeDate != null) cmd.Parameters.AddWithValue("chargeDate", ChargeDate);
                    if (ActivationDate != null) cmd.Parameters.AddWithValue("activationDate", ActivationDate);
                    if (Status != null) cmd.Parameters.AddWithValue("status", Status);

                    SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                    idParam.Direction = ParameterDirection.InputOutput;
                    idParam.Value = this.Id;

                    SqlParameter otpParam = cmd.Parameters.Add("@otp", SqlDbType.VarChar,6);
                    otpParam.Direction = ParameterDirection.InputOutput;
                otpParam.Value = this.OTP;

                SqlParameter chargeParam = cmd.Parameters.Add("@chargedate", SqlDbType.DateTime);
                chargeParam.Direction = ParameterDirection.InputOutput;
                chargeParam.Value = this.ChargeDate;

                SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                    resultParam.Direction = ParameterDirection.InputOutput;


                    int c = cmd.ExecuteNonQuery();

                    this.Id = Convert.ToInt32(idParam.Value);
                    this.OTP = Convert.ToInt32(otpParam.Value);
                this.ChargeDate = Convert.ToDateTime(chargeParam.Value);
                result = Convert.ToInt32(resultParam.Value);
                    cmd.Connection.Close();

                }

            
            return result;

        }




        public int SaveDataForAPP()
        {
            int rc, count=0,result=0;
            
            CashCard[] cashCards = CashCard.GetCashCards(new CashCardParameters { SerialNumber = SerialNUM }, out rc);

            

            if (cashCards[0].Amount < Amount)//not suffecient amount
            {
                count = 1;
            }


            if (count == 0 )
            {
                //OTP = 0;
                ChargeDate = DateTime.Now;
                Status = "0";

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = new SqlConnection(cstr.con);
                    cmd.Connection.Open();
                    cmd.CommandText = "SaveTopupData";


                    if (MeterId != null) cmd.Parameters.AddWithValue("meter_id", MeterId);
                    if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);
                    if (SerialNUM != null) cmd.Parameters.AddWithValue("card_serialnum", cashCards[0].SerialNumber);
                    // if (OTP != null) cmd.Parameters.AddWithValue("otp", OTP);
                    if (ChargeDate != null) cmd.Parameters.AddWithValue("chargeDate", ChargeDate);
                    if (ActivationDate != null) cmd.Parameters.AddWithValue("activationDate", ActivationDate);
                    if (Status != null) cmd.Parameters.AddWithValue("status", Status);

                    SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                    idParam.Direction = ParameterDirection.InputOutput;
                    idParam.Value = this.Id;

                    SqlParameter otpParam = cmd.Parameters.Add("@otp", SqlDbType.Int);
                    otpParam.Direction = ParameterDirection.InputOutput;

                    SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                    resultParam.Direction = ParameterDirection.InputOutput;


                    //try
                    // {
                    int c = cmd.ExecuteNonQuery();
                    //}
                    //catch (Exception ex)
                    //{
                    //    return 0;
                    //}

                    this.Id = Convert.ToInt32(idParam.Value);
                    this.OTP = Convert.ToInt32(otpParam.Value);
                    result = Convert.ToInt32(resultParam.Value);
                    cmd.Connection.Close();

                }
                decimal? amount = cashCards[0].Amount - Amount;
                CashCard cash = new CashCard(cashCards[0].Id, amount, cashCards[0].SerialNumber);
                cash.SaveData();
            }
         else if (count == 1)
            {
                result = 0;
            }
            return result;

        }
    }
}
