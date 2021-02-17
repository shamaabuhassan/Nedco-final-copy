using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models_new
{
    public class Transfer
    {
        public int? Id { get; set; }
        public string SenderOTP { get; set; }
        public int? MeterId { get; set; }
        public decimal? Amount { get; set; }

        public Transfer() { }
        public Transfer(int id)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTransferById";
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    this.SenderOTP = Convert.ToString(r["senderOTP"]);
                    if (r["meter_id"] != DBNull.Value) this.MeterId = Convert.ToInt32(r["meter_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);


                    cmd.Connection.Close();
                }
            }
        }
        public Transfer(int? id, string senderOTP, int? meterId, decimal? amount)
        {
            this.Id = id;
            this.SenderOTP = senderOTP;
            this.MeterId = meterId;
            this.Amount = amount;

        }
        public int SaveData()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "SaveTransferData";
                if (Id != null) cmd.Parameters.AddWithValue("id", Id);
                if (SenderOTP != null) cmd.Parameters.AddWithValue("senderOTP", SenderOTP);
                if (MeterId != null) cmd.Parameters.AddWithValue("meter_id", MeterId);
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
        public static Transfer[] GetTransfers(TransferParameters parameters, out int rowsCount)
        {
            List<Transfer> l = new List<Transfer>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTransfers";
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
                        Transfer c = new Transfer();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToString(r["senderOTP"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToInt32(r["meter_id"]);
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