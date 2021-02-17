using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class EmployeeParameter
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public string Name { get; set; }
    }
    public class Employee
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public string Name { get; set; }

        public Employee() { }

        public Employee(int id,string username,string password, string telephone,string name)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
            this.Telephone = telephone;
            this.Name = name;
        }

        public Employee(int id)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetEmployeeById";

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    this.Username = Convert.ToString(r["username"]);
                    this.Password = Convert.ToString(r["password"]);
                    this.Telephone = Convert.ToString(r["telephone"]);
                    this.Name = Convert.ToString(r["name"]);

                    cmd.Connection.Close();


                }
            }
            }
         public static Employee CheckEmployeeLogin( string username,string password)
        {
            using(SqlCommand cmd=new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "CheckEmployeeLogin";

                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                SqlParameter idParam = cmd.Parameters.Add("@user_id", SqlDbType.Int);
                idParam.Direction = ParameterDirection.InputOutput;

                cmd.ExecuteNonQuery();
                int employeeid = Convert.ToInt32(idParam.Value);
                cmd.Connection.Close();
                Employee employee;

                if (employeeid != 0)
                {
                    employee = new Employee(employeeid);
                }
                else
                {
                    employee = null;
                }
                return employee;
            }
           
        }

    }
}