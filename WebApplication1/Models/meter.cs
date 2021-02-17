using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class MeterParameters
    {
       // public int? Id { get; set; }
        public int? UserId { get; set; }


        public decimal? Amount { get; set; }
        [Display (Name ="meter id")]
        [Required(ErrorMessage = "meter id required ad must be 12 digits")]
        public string Meterid { get; set; }
    }
    public class Meter
    {
       // public int? Id { get; set; }

            [Required]
        public int? UserId { get; set; }

        [Required (ErrorMessage ="amount is required please enter an initial amount")]
        public decimal? Amount { get; set; }

        [Required (ErrorMessage ="meter id required ad must be 12 digits")]
        public string Meterid { get; set; }

        public Meter() { }

        public void Delete()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "DeleteMeter";

                cmd.Parameters.AddWithValue("@meterid", this.Meterid);

                cmd.ExecuteNonQuery();


            }
        }


        //public int CheckMeterForOTPReturn(int? meterid)
        //{
        //    int result = 0;
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = new SqlConnection(cstr.con);
        //        cmd.Connection.Open();
        //        cmd.CommandText = "CheckMeter";

        //        cmd.Parameters.AddWithValue("@meterid", meterid);

        //        SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
        //        resultParam.Direction = ParameterDirection.InputOutput;

        //        int c = cmd.ExecuteNonQuery();

              
        //        result = Convert.ToInt32(resultParam.Value);
        //        cmd.Connection.Close();

        //    }
        //    return result;
        //}
        public Meter(string MeterId)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetMeterByID";

                cmd.Parameters.AddWithValue("@meter_id", MeterId);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    //if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    if (r["user_id"] != DBNull.Value) this.UserId = Convert.ToInt32(r["user_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);
                    if (r["meter_id"] != DBNull.Value) this.Meterid = Convert.ToString(r["meter_id"]);

                    cmd.Connection.Close();


                }
            }
        }

        //counstructor

        public Meter(int? userId, decimal? amount,string meterid)
        {
            //this.Id = id;
            this.UserId = userId;
            this.Amount = amount;
            this.Meterid = meterid;

        }

        public int SaveData()
        {
            int result = 0;
            int rc;
            int count = 0;
            if (Meterid.Length == 12)
            {

                Meter[] meters = Meter.GetMeters(new MeterParameters { }, out rc);
                foreach (Meter meter in meters)
                {
                    if ( Meterid== meter.Meterid)
                    {
                        count = 1;
                    }
                }
                if (count == 0)
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = new SqlConnection(cstr.con);
                        cmd.Connection.Open();
                        cmd.CommandText = "SaveMeterData";

                        if (UserId != null) cmd.Parameters.AddWithValue("user_id", UserId);
                        if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);
                        if (Meterid != null) cmd.Parameters.AddWithValue("meter_id", Meterid);

                        //SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                        //idParam.Direction = ParameterDirection.InputOutput;

                        SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                        resultParam.Direction = ParameterDirection.InputOutput;

                        //idParam.Value = this.Id;

                        int c = cmd.ExecuteNonQuery();

                        //this.Id = Convert.ToInt32(idParam.Value);
                        result = Convert.ToInt32(resultParam.Value);
                        cmd.Connection.Close();


                    }
                }
                if (count == 1)
                {
                    result = 2;
                }
            }
            return result; 
        }

        //get using list

        public static Meter[] GetMeters(MeterParameters parameters, out int rowsCount)
        {
            List<Meter> l = new List<Meter>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetMeters";
                cmd.Parameters.AddWithValue("@user_id", parameters.UserId);
                cmd.Parameters.AddWithValue("@meter_id", parameters.Meterid);
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Meter c = new Meter();
                       // if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["user_id"] != DBNull.Value) c.UserId = Convert.ToInt32(r["user_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);
                        if (r["meter_id"] != DBNull.Value) c.Meterid = Convert.ToString(r["meter_id"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }
    }

}