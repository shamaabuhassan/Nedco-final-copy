using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    public class CashCardParameters
    {
        public int? Id { get; set; }
        public string Password { get; set; }
       
        public decimal? Amount { get; set; }

     
        public string SerialNumber { get; set; }


    }
    public class CashCard
    {
        public int? Id { get; set; }
        public string Password { get; set; }

        [Required (ErrorMessage ="amount is required please enter an initial amount")]
        public decimal ?Amount { get; set; }
       
        [Required (ErrorMessage ="The serial number is required and must be 12 digits")]
        public string SerialNumber { get; set; }
        //get element bu=y id 


        public CashCard(int? id)
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
                    if (r["amount"] != DBNull.Value) this.Amount = Convert.ToDecimal(r["amount"]);
                    if (r["serial_number"] != DBNull.Value) this.SerialNumber = Convert.ToString(r["serial_number"]);
                    cmd.Connection.Close();


                }
            }
        }

        //constructor
        public CashCard(int?id, decimal? amount,string SerialNumber)
        {
            this.Id = id;
           // this.Password = password;
            this.Amount = amount;
            this.SerialNumber = SerialNumber;

        }

        public void Delete()
        {
            
            using(SqlCommand cmd=new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "DeleteCashCard";

                cmd.Parameters.AddWithValue("@id", this.Id);
                cmd.ExecuteNonQuery();
            }
        }
        public int SaveData()
        {
            int result = 0;
            int rc;
            int count = 0;
            if (SerialNumber.ToString().Length== 12)
            {
                
                CashCard[] cashCards = CashCard.GetCashCards(new CashCardParameters { }, out rc);
                foreach(CashCard cashCard in cashCards)
                {
                    if (SerialNumber == cashCard.SerialNumber)
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
                        cmd.CommandText = "SaveCashCardData";



                        if (Password != null) cmd.Parameters.AddWithValue("password", Password);
                        if (Amount != null) cmd.Parameters.AddWithValue("amount", Amount);
                        if (SerialNumber != null) cmd.Parameters.AddWithValue("serial_number", SerialNumber);


                        SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                        idParam.Direction = ParameterDirection.InputOutput;
                        idParam.Value = this.Id;

                        SqlParameter resultParm = cmd.Parameters.Add("@result", SqlDbType.Int);
                        resultParm.Direction = ParameterDirection.InputOutput;



                        int c = cmd.ExecuteNonQuery();


                        this.Id = Convert.ToInt32(idParam.Value);
                        result = Convert.ToInt32(resultParm.Value);
                        cmd.Connection.Close();


                    }
                }
                else
                {
                    result = 2;
                }
            }
            return result;
        }
        public CashCard()
        { }

        //get data using list

        

             public static CashCard[] GetCashCardForCustomer(CashCardParameters parameters, out int rowsCount)
        {
            List<CashCard> l = new List<CashCard>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetCashCardForCustomer";

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        //create object of the class
                        CashCard c = new CashCard();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["password"] != DBNull.Value) c.Password = Convert.ToString(r["password"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);
                        if (r["serial_number"] != DBNull.Value) c.SerialNumber = Convert.ToString(r["serial_number"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

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

                cmd.Parameters.AddWithValue("serial_number", parameters.SerialNumber);
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        //create object of the class
                        CashCard c = new CashCard();
                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["password"] != DBNull.Value) c.Password = Convert.ToString(r["password"]);
                        if (r["amount"] != DBNull.Value) c.Amount = Convert.ToDecimal(r["amount"]);
                        if (r["serial_number"] != DBNull.Value) c.SerialNumber = Convert.ToString(r["serial_number"]);

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