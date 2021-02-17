using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CustomerParameters
    {
        public string letter { get; set; }
        public int? Id { get; set; }
        public string Username { get; set; }
        public int? CardId { get; set; }
        public string Telephone { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }



    }
    public class Customer
    {
        public int? Id { get; set; }

        [Required (ErrorMessage ="the username is required please enter")]
        public string Username { get; set; }

        
        public int? CardId { get; set; }

        [Required(ErrorMessage = "the telephone is required and must be 10 numbers statr with (059) or (056)")]
        public string Telephone { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? TownId { get; set; }

        [Required (ErrorMessage ="street is required please enter your street") ]
        public string Street { get; set; }

        [Required (ErrorMessage ="Password is Required please enter")]
        public string Password { get; set; }

        [Required (ErrorMessage ="please enter your full name")]
        public string Name { get; set; }

        public string CountryName { get; set; }
        public string CityName { get; set; }

        public string TownName { get; set; }

        public static Customer CheckLogin(string Username,string Password) {



            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "CheckLogin";

                cmd.Parameters.AddWithValue("@username", Username);
                cmd.Parameters.AddWithValue("@password", Password);



                SqlParameter idParam = cmd.Parameters.Add("@user_id", SqlDbType.Int);
                idParam.Direction = ParameterDirection.InputOutput;

       

                int c = cmd.ExecuteNonQuery();

                int customerid = Convert.ToInt32(idParam.Value);
                cmd.Connection.Close();
                Customer customer;
                if (customerid != 0)
                {
                    customer = new Customer(customerid);
                    
                        }
                else
                {
                    customer = null;
                }

                return customer;
            }


        }
            public Customer(int ID)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetCustomerByID";
                cmd.Parameters.AddWithValue("@ID", ID);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    this.Username = Convert.ToString(r["username"]);
                    if (r["card_id"] != DBNull.Value) this.CardId = Convert.ToInt32(r["card_id"]);
                    this.Telephone = Convert.ToString(r["telephone"]);
                    if (r["country_id"] != DBNull.Value) this.CountryId = Convert.ToInt32(r["country_id"]);
                    if (r["city_id"] != DBNull.Value) this.CityId = Convert.ToInt32(r["city_id"]);
                    if (r["town_id"] != DBNull.Value) this.TownId = Convert.ToInt32(r["town_id"]);
                    this.Street = Convert.ToString(r["street"]);
                    this.Password = Convert.ToString(r["password"]);
                    this.Name = Convert.ToString(r["name"]);

                    cmd.Connection.Close();


                }
            }
        }

       
        public Customer() { }

        //counstructor
        public Customer(int? id, string username, int? cardId, string telephone, int? countryId, int? cityId, int? townid, string street, string password,string name)
        {
            this.Id = id;
            this.Username = username;
            this.CardId = cardId;
            this.Telephone = telephone;
            this.CountryId = countryId;
            this.CityId = cityId;
            this.TownId = townid;
            this.Street = street;
            this.Password = password;
            this.Name = name;

        }
//public Customer(int? id, string password) {
//            this.Id = id;
//            this.Password = password;
//        }
        public int SaveData()
        {
            //int rc;
            //int count = 0;
            //Customer[] customers = Customer.GetCustomers(new CustomerParameters {}, out rc);
            //foreach(Customer customer in customers)
            //{
            //    if(CardId==customer.CardId)
            //    {
            //        count = 1;
            //    }
            //    else
            //    {
            //        count = 0;
            //    }
            //}
            //if (count == 0)
            //{
            int rc;
            int count = 0;
            int result = 0;
            int count2 = 0;
            //int islength = 0;
             int count3 = 0;

            Customer[] customers = Customer.GetCustomers(new CustomerParameters { }, out rc);

            foreach(Customer customer in customers)
            {
                if (customer.Username.ToLower() == Username.ToLower())
                {
                    count = 1;
                }
            }
            if ( this.Telephone != null && this.Telephone.Length !=10 )//not 10 numbers
            {
                count2 = 1;
              
            }
           
            if (count2 == 0 )//lenght is 10
            {
                if(this.Telephone != null &&  this.Telephone[0].ToString()=="0" && this.Telephone[1].ToString()=="5" && (this.Telephone[2].ToString() == "6"|| this.Telephone[2].ToString() == "9"))
                count3 = 1;
            }
            if (count == 0 && count3==1 )
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = new SqlConnection(cstr.con);
                    cmd.Connection.Open();
                    cmd.CommandText = "SaveCustomerData";

                    if (Username != null) cmd.Parameters.AddWithValue("username", Username);
                    if (CardId != null) cmd.Parameters.AddWithValue("card_id", CardId);
                    if (Telephone != null) cmd.Parameters.AddWithValue("telephone", Telephone);
                    if (CountryId != null) cmd.Parameters.AddWithValue("country_id", CountryId);
                    if (CityId != null) cmd.Parameters.AddWithValue("city_id", CityId);
                    if (TownId != null) cmd.Parameters.AddWithValue("town_id", TownId);
                    if (Street != null) cmd.Parameters.AddWithValue("street", Street);
                    if (Password != null) cmd.Parameters.AddWithValue("password", Password);
                    if (Name != null) cmd.Parameters.AddWithValue("name", Name);


                    SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                    idParam.Direction = ParameterDirection.InputOutput;
                    idParam.Value = this.Id;

                    SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                    resultParam.Direction = ParameterDirection.InputOutput;

                    // idParam.Value = this.Id;

                    int c = cmd.ExecuteNonQuery();

                    this.Id = Convert.ToInt32(idParam.Value);
                    result = Convert.ToInt32(resultParam.Value);
                    cmd.Connection.Close();
                }

            }
            else if(count==1)//user exist
            {
                result = 2;
            }
            else if (count2 == 1)//number not 10digit
            {
                result = 0;
            }
            else if (count2 == 0 && count3 == 0)// not standad
            {
                result = 3;
            }
            return result;

            //}
            //else
            //{
            //    int result = 0;
            //    return result;
            //}
        }


        //get using 
        public void  Delete()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "DeleteCustomer";

                cmd.Parameters.AddWithValue("@id", this.Id);

                cmd.ExecuteNonQuery();


            }
        }

        public static Customer[] GetAutoCustomers(CustomerParameters parameters, out int rowsCount)
        {

            List<Customer> l = new List<Customer>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetAutoCustomers";
                cmd.Parameters.AddWithValue("@letter", parameters.letter);


                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Customer c = new Customer();

                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["username"] != DBNull.Value) c.Username = Convert.ToString(r["username"]);
                        if (r["card_id"] != DBNull.Value) c.CardId = Convert.ToInt32(r["card_id"]);
                        if (r["telephone"] != DBNull.Value) c.Telephone = Convert.ToString(r["telephone"]);
                        if (r["country_id"] != DBNull.Value) c.CountryId = Convert.ToInt32(r["country_id"]);
                        //c.CountryName = Convert.ToString(r["country_name"]);
                        //if (r["city_id"] != DBNull.Value) c.CityId = Convert.ToInt32(r["city_id"]);
                        //c.CityName = Convert.ToString(r["city_name"]);
                        if (r["town_id"] != DBNull.Value) c.TownId = Convert.ToInt32(r["town_id"]);
                        if (r["street"] != DBNull.Value) c.Street = Convert.ToString(r["street"]);
                        if (r["password"] != DBNull.Value) c.Password = Convert.ToString(r["password"]);
                        if (r["name"] != DBNull.Value) c.Name = Convert.ToString(r["name"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }
        public static Customer[] GetCustomers(CustomerParameters parameters, out int rowsCount)
        {
            List<Customer> l = new List<Customer>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetCustomers";
                cmd.Parameters.AddWithValue("@cardid", parameters.CardId);
                

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Customer c = new Customer();

                        if (r["id"] != DBNull.Value) c.Id = Convert.ToInt32(r["id"]);
                        if (r["username"] != DBNull.Value) c.Username = Convert.ToString(r["username"]);
                        if (r["card_id"] != DBNull.Value) c.CardId = Convert.ToInt32(r["card_id"]);
                        if (r["telephone"] != DBNull.Value) c.Telephone = Convert.ToString(r["telephone"]);
                        if (r["country_id"] != DBNull.Value) c.CountryId = Convert.ToInt32(r["country_id"]);
                        c.CountryName = Convert.ToString(r["country_name"]);
                        if (r["city_id"] != DBNull.Value) c.CityId = Convert.ToInt32(r["city_id"]);
                        c.CityName = Convert.ToString(r["city_name"]);
                        if (r["town_id"] != DBNull.Value) c.TownId = Convert.ToInt32(r["town_id"]);
                        c.TownName = Convert.ToString(r["town_id"]);
                        if (r["street"] != DBNull.Value) c.Street = Convert.ToString(r["street"]);
                        if (r["password"] != DBNull.Value) c.Password = Convert.ToString(r["password"]);
                        if (r["name"] != DBNull.Value) c.Name = Convert.ToString(r["name"]);

                        l.Add(c);
                    }
                }

                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();

        }


        public int AlterPassword()
        {
            int result = 0;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = new SqlConnection(cstr.con);
                    cmd.Connection.Open();
                    cmd.CommandText = "AlterPassword";

                    if (Username != null) cmd.Parameters.AddWithValue("username", Username);
                    if (CardId != null) cmd.Parameters.AddWithValue("card_id", CardId);
                    if (Telephone != null) cmd.Parameters.AddWithValue("telephone", Telephone);
                    if (CountryId != null) cmd.Parameters.AddWithValue("country_id", CountryId);
                    if (CityId != null) cmd.Parameters.AddWithValue("city_id", CityId);
                    if (TownId != null) cmd.Parameters.AddWithValue("town_id", TownId);
                    if (Street != null) cmd.Parameters.AddWithValue("street", Street);
                    if (Password != null) cmd.Parameters.AddWithValue("password", Password);
                    if (Name != null) cmd.Parameters.AddWithValue("name", Name);


                    SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                    idParam.Direction = ParameterDirection.InputOutput;
                    idParam.Value = this.Id;

                    SqlParameter resultParam = cmd.Parameters.Add("@result", SqlDbType.Int);
                    resultParam.Direction = ParameterDirection.InputOutput;

                    // idParam.Value = this.Id;

                    int c = cmd.ExecuteNonQuery();

                    this.Id = Convert.ToInt32(idParam.Value);
                    result = Convert.ToInt32(resultParam.Value);
                    cmd.Connection.Close();
                }

            return result;

           
        }
    }
}