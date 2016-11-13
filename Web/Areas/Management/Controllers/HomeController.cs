using System;
using System.Collections.Generic;
using System.IO;
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
            //DownloadFile(Server.MapPath("/Content/PDFFile/Thongtinnha_13112016_022706.pdf"));
            return View();
        }
        void DownloadFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/pdf";
                Response.WriteFile(file.FullName);
                Response.End();
            }
        }
    }
}