using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    
    public class cstr
    {
        public static string con
        {
            //get { return "Server=localhost\\SQLEXPRESS;Database=nedco;User Id=sa;Password=123;"; }  
            //redo
            get { return ConfigurationManager.ConnectionStrings["cstr"].ConnectionString; }

        }

    }
}