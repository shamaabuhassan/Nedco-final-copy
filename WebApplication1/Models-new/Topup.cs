using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models_new
{
    public class Topup
    {
        public int? Id { get; set; }
        public int? MeterId { get; set; }
        public decimal? Amount { get; set; }
        public int? CardId { get; set; }
        public string OTP { get; set; }
        public DateTime? ChargeDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string Status { get; set; }

        public Topup() { }
        public Topup(int id)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTopupById";
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    if (r["meter_id"] != DBNull.Value) this.MeterId = Convert.ToInt32(r["meter_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);
                    if (r["card_id"] != DBNull.Value) this.CardId = Convert.ToInt32(r["card_id"]);
                    this.OTP = Convert.ToString(r["otp"]);
                    if (r["chargeDate"] != DBNull.Value) this.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
                    if (r["activationDate"] != DBNull.Value) this.ActivationDate = Convert.ToDateTime(r["activationDate"]);
                    this.Status = Convert.ToString(r["status"]);


                    cmd.Connection.Close();
                }
            }
        }
        public Topup(int? id, int? meterId, decimal? amount, int? cardId, string otp, DateTime? chargeDate, DateTime? activationDate, string status)
        {
            this.Id = id;
            this.MeterId = meterId;
            this.Amount = amount;
            this.CardId = cardId;
            this.OTP = otp;
            this.ChargeDate = chargeDate;
            this.ActivationDate = activationDate;
            this.Status = status;

        }
        public int SaveData()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "SaveTopupData";
                if (Id != null) cmd.Parameters.AddWithValue("id", Id);
                if (MeterId != null) cmd.Parameters.AddWithValue("meter_id", MeterId);
                if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);
                if (CardId != null) cmd.Parameters.AddWithValue("card_id", CardId);
                if (OTP != null) cmd.Parameters.AddWithValue("otp", OTP);
                if (ChargeDate != null) cmd.Parameters.AddWithValue("chargeDate", ChargeDate);
                if (ActivationDate != null) cmd.Parameters.AddWithValue("activationDate", ActivationDate);
                if (Status != null) cmd.Parameters.AddWithValue("status", Status);

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
        public static Topup[] GetTopups(TopupParameters parameters, out int rowsCount)
        {
            List<Topup> l = new List<Topup>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTopups";
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
                        Topup c = new Topup();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToInt32(r["meter_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);
                        if (r["card_id"] != DBNull.Value) c.CardId = Convert.ToInt32(r["card_id"]);
                        if (r["otp"] != DBNull.Value) c.OTP = Convert.ToString(r["otp"]);
                        if (r["chargeDate"] != DBNull.Value) c.ChargeDate = Convert.ToDateTime(r["chargeDate"]);
                        if (r["activationDate"] != DBNull.Value) c.ActivationDate = Convert.ToDateTime(r["activationDate"]);
                        if (r["status"] != DBNull.Value) c.Status = Convert.ToString(r["status"]);

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