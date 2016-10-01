using System.Web.Mvc;
using System.Web.Routing;
namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapRoute(name: "DefaultCaptchaRoute1", url: "SuperCaptcha/InitCaptcha", defaults: new { controller = "SuperCaptcha", action = "InitCaptcha" }, namespaces: new[] { "Web.Controllers" });
            //routes.MapRoute(name: "DefaultCaptchaRoute2", url: "SuperCaptcha/NewCaptcha", defaults: new { controller = "SuperCaptcha", action = "NewCaptcha" }, namespaces: new[] { "Web.Controllers" });
            //routes.MapRoute(name: "DefaultCaptchaRoute3", url: "SuperCaptcha/ValidateCaptcha", defaults: new { controller = "SuperCaptcha", action = "ValidateCaptcha" }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute(name: "DefaultCaptchaRoute", url: "DefaultCaptcha/Generate", defaults: new { controller = "DefaultCaptcha", action = "Generate" }, namespaces: new[] { "Web.Controllers" });
            routes.MapRoute(name: "DefaultCaptchaRouteRefresh", url: "DefaultCaptcha/Refresh", defaults: new { controller = "DefaultCaptcha", action = "Refresh" }, namespaces: new[] { "Web.Controllers" });
            routes.MapMvcAttributeRoutes();//Attribute Routing
            AreaRegistration.RegisterAllAreas();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Web.Controllers" }
            );
        }
    }
}
