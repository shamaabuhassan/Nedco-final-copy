using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AreaController : Controller
    {
        // GET: Area


        [HttpPost]
        public ActionResult Index(Area area)
        {

            int? resultt = 0;
            int rc;
            Area[] areas = new Area[] { };


            if (ModelState.IsValid )
            { //checking model state
                if (area.Type == "s")
                {
                    ViewBag.areas = areas;
                }
                if (area.Type == "c")
                {

                    areas = Area.getareabytype(new AreaParameters { Type = "s" }, out rc);
                    ViewBag.areas = areas;
                }
                if (area.Type == "t")
                {

                    areas = Area.getareabytype(new AreaParameters { Type = "c" }, out rc);
                    ViewBag.areas = areas;

                }
                //check whether id is already exists in the database or not

                resultt = area.SaveData();
            
                
                if (resultt == 4)
                {
                    ModelState.AddModelError("Name", "name is exist ");
                    ViewBag.result = resultt;
                    ViewBag.type = area.Type;
                    ViewBag.areas=areas;
                    return View(area);
                }
                else if (resultt == 2)
                {
                    ModelState.AddModelError("Id", "id must be 3 digits");
                    ViewBag.result = resultt;
                    ViewBag.type = area.Type;
                    ViewBag.areas = areas;
                    return View(area);
                }
                else if (resultt == 3)
                {
                    ModelState.AddModelError("Id", "id is exist");
                    ViewBag.result = resultt;
                    ViewBag.type = area.Type;
                    ViewBag.areas = areas;
                    return View(area);
                }
                else
                {

                   // ViewBag.result = resultt;
                    //ViewBag.type = area.Type;
                    //ViewBag.areas = areas;
                    return RedirectToAction("Index", "Area",new { result = resultt});
                   // return View();
                }
            }

            else
            {
                ViewBag.result = resultt;
                ViewBag.type = area.Type;
                if (area.Type == "s")
                {
                    ViewBag.areas = areas; 
                }
                if (area.Type == "c")
                {
                   
                    areas = Area.getareabytype(new AreaParameters { Type = "s" }, out rc);
                    ViewBag.areas = areas;
                }
                if (area.Type == "t")
                {
                    
                    areas = Area.getareabytype(new AreaParameters { Type = "c" }, out rc);
                    ViewBag.areas = areas;

                }
               
                return View(area);
            }

        }


        public ActionResult Index(string type, int? Id, string Name, int? ParentId, string Type, int? result)
        {
            if (Session["employee"] == null)
            {
                return RedirectToAction("index", "Employees");
            }

            
            else
            {
                if (result == 1)
                {
                    ViewBag.result = result;
                    return View();
                }
                Area[] areas = new Area[] { };
                if (type == "s")
                {
                    ViewBag.areas = areas;
                    ViewBag.type = type;
                }
                if (type == "c")
                { int rc;
                    areas = Area.getareabytype(new AreaParameters { Type = "s" }, out rc);
                    ViewBag.areas = areas;
                    ViewBag.type = type;
                }
                if (type == "t")
                {
                    int rc;
                    areas = Area.getareabytype(new AreaParameters { Type = "c" }, out rc);
                    ViewBag.areas = areas;
                    ViewBag.type = type;
                   
                }
                Area area = new Area(Id, Name, ParentId, Type);
                return View(area);
            }
        }

        //[HttpPost]
        //public JsonResult Getparent(string type)
        //{
        //  int state = 0;
        //    int rc = 0;
        //    if (type == "s" || type == "S") {

        //      /*  ViewBag.state = 0;
        //        ViewBag.first = 0;*/
        //        return Json("this is parent");
        //    }
        //    else  if (type == "c" || type == "C") {
        //        Area[] areas = Area.getarea(new AreaParameters {Type= "s"},out rc);
        //        /*ViewBag.areas = areas;
        //        ViewBag.state = state;
        //        ViewBag.first = 0;*/
        //        return Json(areas);

        //    }
        //    else if (type == "t" || type == "T") {
        //        Area[] areas = Area.getarea(new AreaParameters { Type = "c" }, out rc);
        //       /* ViewBag.areas = areas;
        //        ViewBag.state = state;
        //        ViewBag.first = 0;*/
        //        return Json(areas);
        //    }
        //    else{
        //        return Json(state);
        //    }

        //}

        //[HttpPost]
        //    public JsonResult SaveNew(string type, int? id, string name){

        //Area area=new Area(id,name,0,type);
        //       int result= area.SaveData();
        //        return Json(result);
        //    }


        //[HttpPost]
        //    public JsonResult SaveNewParent(string type, int? id, string name, int? parentid){

        //Area area=new Area(id,name,parentid,type);
        //       int result= area.SaveData();
        //        return Json(result);
        //    }


        //Id
        //Name
        //public ActionResult Save(int? Id, string Name, int? ParentId,string Type)
        //{
            
        //    Area area = new Area(Id, Name, ParentId, Type);
        //   // int? result = area.SaveData();
        //   // ViewBag.result = result;
        //    return RedirectToAction("Index", "Area",new { result = result });
        //}
    }
}