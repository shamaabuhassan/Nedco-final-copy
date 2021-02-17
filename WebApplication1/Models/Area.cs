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

    public class AreaParameters
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string Type { get; set; }
    }
    public class Area
    {
        [Required (ErrorMessage ="Id is required and must be 3 digits please enter")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Name is required please enter and it must be unique")]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string Type { get; set; }


        public Area()
        { }

        public Area(int? id,string name,int? parentid,string type)
        {
            this.Id = id;
            this.Name = name;
            this.ParentId = parentid;
            this.Type = type;
        }

        public Area(string type)
        {

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetAreaByType";
                cmd.Parameters.AddWithValue("@type", type);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    r.Read();
                    if (r["id"] != DBNull.Value) this.Id = Convert.ToInt32(r["id"]);
                    this.Name = Convert.ToString(r["name"]);
                    if (r["parent_id"] != DBNull.Value) this.ParentId = Convert.ToInt32(r["parent_id"]);
                    this.Type = Convert.ToString(r["type"]);
                   

                    cmd.Connection.Close();


                }
            }
        }

        public int SaveData()
        {
            int rc, count = 0, count2 = 0, count3 = 1;
            int result = 0;
            Area[] areas = Area.getarea(new AreaParameters { }, out rc);

            if (Id.ToString().Length == 3)//3 digit
            {
                count3 = 0;
            }

            if (count3 == 0)
            {
                foreach (Area area in areas)
                {
                    if (Id == area.Id)
                    {
                        count = 1;
                    }
                }
            }

            foreach (Area area in areas)
            {
                if (Name == area.Name)
                {
                    count2 = 1;
                }
            }


            if (count3 == 0 && count == 0 && count2 == 0)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = new SqlConnection(cstr.con);
                    cmd.Connection.Open();
                    cmd.CommandText = "SaveArea";



                    if (Id != null) cmd.Parameters.AddWithValue("id", Id);
                    if (Name != null) cmd.Parameters.AddWithValue("name", Name);
                    if (Type != null) cmd.Parameters.AddWithValue("type", Type);
                    if (ParentId != null) cmd.Parameters.AddWithValue("parent_id", ParentId);

                    SqlParameter resultParm = cmd.Parameters.Add("@result", SqlDbType.Int);
                    resultParm.Direction = ParameterDirection.InputOutput;



                    int c = cmd.ExecuteNonQuery();

                    result = Convert.ToInt32(resultParm.Value);
                    cmd.Connection.Close();

                }
            }
            else if (count3 == 1)//not 3 digit
            {
                result = 2;
            }
            else if (count3 == 0 && count == 1)// id exist
            {
                result = 3;
            }
            else if (count2 == 1)//name exist
            {
                result = 4;
            }
            return result;
        }



        public static Area[] getarea(AreaParameters parameters, out int rowsCount)
        {
            List<Area> l = new List<Area>();
            using (SqlCommand cmd = new SqlCommand())
            {
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetArea";

                cmd.Parameters.AddWithValue("@parent_id", parameters.ParentId);
                cmd.Parameters.AddWithValue("@name", parameters.Name);
                cmd.Parameters.AddWithValue("@id", parameters.Id);

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Area a = new Area();
                        if (r["id"] != DBNull.Value) a.Id = Convert.ToInt32(r["id"]);
                        if (r["name"] != DBNull.Value) a.Name = Convert.ToString(r["name"]);
                        if (r["parent_id"] != DBNull.Value) a.ParentId = Convert.ToInt32(r["parent_id"]);
                        if (r["type"] != DBNull.Value) a.Type = Convert.ToString(r["type"]);

                        l.Add(a);

                    }

                }
                r.Close();
                cmd.Connection.Close();
                rowsCount = l.Count;
            }
            return l.ToArray();
        }

        public static Area[] getareabytype(AreaParameters parameters, out int rowsCount)
        {
            List<Area> l = new List<Area>();
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = new SqlConnection(cstr.con);
                cmd.Connection.Open();
                cmd.CommandText = "GetAreaByType";

                cmd.Parameters.AddWithValue("@type", parameters.Type);
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Area a = new Area();
                        if (r["id"] != DBNull.Value) a.Id = Convert.ToInt32(r["id"]);
                        if (r["name"] != DBNull.Value) a.Name = Convert.ToString(r["name"]);
                        if (r["parent_id"] != DBNull.Value) a.ParentId = Convert.ToInt32(r["parent_id"]);
                        if (r["type"] != DBNull.Value) a.Type = Convert.ToString(r["type"]);

                        l.Add(a);

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