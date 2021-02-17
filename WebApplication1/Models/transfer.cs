using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
 
    public class TransferParameters{
        public DateTime? fromdate { get; set; }
        public DateTime? todate { get; set; }
        public int? Id { get; set; }
        public int? SenderOTP { get; set; }
        public string MeterId { get; set; }
        public decimal? Amount { get; set; }

      
    }
public class Transfer
    {
        public string Status { get; set; }
        public int? Id { get; set; }
        public int? SenderOTP { get; set; }

        [Required (ErrorMessage ="the meter id is required") ]
        public string MeterId { get; set; }
        public decimal? Amount { get; set; }

  public string Sender_meter { get; set; }
   
        public Transfer()
        {

        }

     public Transfer(int id)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTransfersByID";
                cmd.Parameters.AddWithValue("@ID", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    if (r["senderOTP"] != DBNull.Value) this.SenderOTP = Convert.ToInt32(r["senderOTP"]);
                    //this.SenderOTP = Convert.ToString(r["senderOTP"]);
                    if (r["meter_id"] != DBNull.Value) this.MeterId = Convert.ToString(r["meter_id"]);
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);
                    cmd.Connection.Close();


                }
            }
        }


        //constructor

        public Transfer(int? id, int? senderOTP, string meterId, decimal? amount)
        {
            this.Id = id;
            this.SenderOTP = senderOTP;
            this.MeterId = meterId;
            this.Amount = amount;

        }

        //public int RandomNumber(int digits)
        //{
        //    int count = 0;
        //    int rand = 0;
        //    while (count != 1)
        //    {
        //        Random random = new Random();
        //        rand = random.Next();
        //        int len = rand.ToString().Length;

        //        if (len == digits)
        //            count = 1;
        //    }
        //    return rand;
        //}

        public void Delete()
        {
      using(SqlCommand cmd=new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "DeleteTransfer";

                cmd.Parameters.AddWithValue("@id", this.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public Topup[] SaveData()
        {
            int result = 0;
            int rc;
            Topup[] topups = Topup.GetTopups(new TopupParameters { OTP = SenderOTP },out rc);
            Topup topup = topups[0];
            List<Topup> t = new List<Topup>();
            if (topup.Status == "0")

            {
                if (topup.Amount > Amount)
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = new SqlConnection(cstr.con);
                        cmd.Connection.Open();
                        cmd.CommandText = "SaveTransferData";


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
                        result = Convert.ToInt32(resultParam.Value);
                        cmd.Connection.Close();

                        decimal? amount = topup.Amount - Amount;
                        Topup topup1 = new Topup(null,topup.MeterId, amount, topup.SerialNUM);
                        topup1.SaveDataTransfered();
                        Topup topup2 = new Topup(null,MeterId, Amount, topup.SerialNUM);
                        topup2.SaveDataTransfered();

                        Topup topupnotvalid = new Topup(topup.Id);
                        topupnotvalid.SaveDataTransfered();

                        t.Add(topup1);
                        t.Add(topup2);
                        
                    }
                }
            }
            return t.ToArray();
        }

        

            public static Transfer[] GetTransfersBySenderOTP(TransferParameters parameters, out int rowsCount)
        {
            List<Transfer> l = new List<Transfer>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTransfersBySenderOTP";

                cmd.Parameters.AddWithValue("@meter_id", parameters.MeterId);
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Transfer c = new Transfer();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToInt32(r["senderOTP"]);
                      //  if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToString(r["senderOTP"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["sender_meter"] != DBNull.Value) c.Sender_meter = Convert.ToString(r["sender_meter"]); 
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }

        public static Transfer[] GetTransfersBySenderOTPwithdate(TransferParameters parameters, out int rowsCount)
        {
            List<Transfer> l = new List<Transfer>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTransfersBySenderOTPwithdate";

                cmd.Parameters.AddWithValue("@meter_id", parameters.MeterId);
                cmd.Parameters.AddWithValue("@fromdate", parameters.fromdate);
                cmd.Parameters.AddWithValue("@todate", parameters.todate);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Transfer c = new Transfer();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToInt32(r["senderOTP"]);
                        //  if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToString(r["senderOTP"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["sender_meter"] != DBNull.Value) c.Sender_meter =Convert.ToString(r["sender_meter"]);
                       
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

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
                cmd.Parameters.AddWithValue("@senderotp",parameters.SenderOTP);
                cmd.Parameters.AddWithValue("@meter_id",parameters.MeterId);
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Transfer c = new Transfer();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToInt32(r["senderOTP"]);
                        // if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToString(r["senderOTP"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }

        public static Transfer[] GetTransferswithdate(TransferParameters parameters, out int rowsCount)
        {
            List<Transfer> l = new List<Transfer>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetTransferswithdate";
                //cmd.Parameters.AddWithValue("@senderotp", parameters.SenderOTP);

                cmd.Parameters.AddWithValue("@fromdate", parameters.fromdate);
                cmd.Parameters.AddWithValue("@todate", parameters.todate);
                cmd.Parameters.AddWithValue("@meter_id", parameters.MeterId);
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Transfer c = new Transfer();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToInt32(r["senderOTP"]);
                        // if (r["senderOTP"] != DBNull.Value) c.SenderOTP = Convert.ToString(r["senderOTP"]);
                        if (r["meter_id"] != DBNull.Value) c.MeterId = Convert.ToString(r["meter_id"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);

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