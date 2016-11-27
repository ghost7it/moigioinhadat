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
            //Check role để get default page
            var currentRole = AccountRoles;

            //Ưu tiên Admin -> Nhân viên nhập liệu -> Nhân viên chăm sóc
            foreach (var item in AccountRoles)
            {
                //Admin
                if (item.RoleId == 1)
                {
                    return RedirectToRoute("PhanCongCongViecIndex");
                }
                //Nhập liệu
                else if (item.RoleId == 2)
                {
                    return RedirectToRoute("NhaIndex");
                }
                //Chăm sóc
                else if (item.RoleId == 3)
                {
                    return RedirectToRoute("DanhSachCongViecIndex");
                }
            }


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