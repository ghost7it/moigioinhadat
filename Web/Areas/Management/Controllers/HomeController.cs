using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]    
    public class HomeController : BaseController
    {
        [Route(Name = "ManagementHome")]
        public ActionResult Index()
        {
            return View();
        }
    }
}