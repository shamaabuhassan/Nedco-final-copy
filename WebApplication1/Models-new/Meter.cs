using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models_new
{
    public class Meter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public decimal? Amount { get; set; }

        public Meter() { }
        public Meter(int id)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetMeterById";
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    if (r["user_id"] != DBNull.Value) this.UserId = Convert.ToInt32(r["user_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);


                    cmd.Connection.Close();
                }
            }
        }
        public Meter(int? id, int? userId, decimal? amount)
        {
            this.Id = id;
            this.UserId = userId;
            this.Amount = amount;

        }
        public int SaveData()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "SaveMeterData";
                if (Id != null) cmd.Parameters.AddWithValue("id", Id);
                if (UserId != null) cmd.Parameters.AddWithValue("user_id", UserId);
                if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);

                SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                idParam.Direction = ParameterDirection.InputOutput;

                SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                resultParam.Direction = ParameterDirection.InputOutput;

                idParam.Value = this.Id;

                int c = cmd.ExecuteNonQuery();

                this.Id = Convert.ToInt32(idParam.Value);
                int result = Convert.ToInt32(resultParam.Value);
                cmd.Connection.Close();
                return result;
            }
        }
        public void Delete()
        {
            this.Status = "3";
            this.SaveData();
        }
        public static Meter[] GetMeters(MeterParameters parameters, out int rowsCount)
        {
            List<Meter> l = new List<Meter>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetMeters";
                /*cmd.Parameters.AddWithValue("status", parameters.Status);
                cmd.Parameters.AddWithValue("page", parameters.CurrentPage);
                cmd.Parameters.AddWithValue("pageLength", parameters.PageLength);
                SqlParameter rowsCountParam = cmd.Parameters.Add("rowsCount", SqlDbType.Int);
                rowsCountParam.Direction = ParameterDirection.InputOutput;*/
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Meter c = new Meter();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["user_id"] != DBNull.Value) c.UserId = Convert.ToInt32(r["user_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                //rowsCount = Convert.ToInt32(rowsCountParam.Value);
                rowsCount = l.Count;
            }
            return l.ToArray();

        }


    }
}