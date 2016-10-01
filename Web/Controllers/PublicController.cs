using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    //[RoutePrefix("chung")]
    public class PublicController : Controller
    {
       [Route("~/loi", Name = "FrontEndPublicIndex")]
        public ActionResult Index()
        {
            ViewBag.Exception = TempData["Exception"];
            return View();
        }
    }
}