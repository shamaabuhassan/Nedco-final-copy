using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Models_new
{
    public class CashCard
    {
        public int? Id { get; set; }
        public string Password { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Amount { get; set; }

        public CashCard() { }
        public CashCard(int id)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetCashCardById";
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    this.Password = Convert.ToString(r["password"]);
                    if (r["customer_id"] != DBNull.Value) this.CustomerId = Convert.ToInt32(r["customer_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);


                    cmd.Connection.Close();
                }
            }
        }
        public CashCard(int? id, string password, int? customerId, decimal? amount)
        {
            this.Id = id;
            this.Password = password;
            this.CustomerId = customerId;
            this.Amount = amount;

        }
        public int SaveData()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "SaveCashCardData";

                if (Id != null) cmd.Parameters.AddWithValue("id", Id);
                if (Password != null) cmd.Parameters.AddWithValue("password", Password);
                if (CustomerId != null) cmd.Parameters.AddWithValue("customer_id", CustomerId);
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
        public static CashCard[] GetCashCards(CashCardParameters parameters, out int rowsCount)
        {
            List<CashCard> l = new List<CashCard>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetCashCards";
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
                        CashCard c = new CashCard();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["password"] != DBNull.Value) c.Password = Convert.ToString(r["password"]);
                        if (r["customer_id"] != DBNull.Value) c.CustomerId = Convert.ToInt32(r["customer_id"]);
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