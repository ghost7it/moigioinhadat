using System.Web.Mvc;

namespace Web.Areas.Management
{
    public class ManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Management";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "Management_default2",
            //    "quan-ly/{action}/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    new[] { "Web.Areas.Management.Controllers" }
            //);
            context.MapRoute(
                "Management_default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Web.Areas.Management.Controllers" }
            );
        }
    }
}