using Entities.Models;
using Interface;
using System.Web.Mvc;
using System.Linq;
using Common;
namespace Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IRepository _repository = DependencyResolver.Current.GetService<IRepository>();
        protected readonly CacheFactory _cacheFactory = new CacheFactory();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.SystemInformation = SystemInformation;
        }
        public SystemInformation SystemInformation
        {
            get
            {
                SystemInformation obj = _cacheFactory.GetCache("SystemInformation") as SystemInformation;
                SystemInformation _SystemInformation;
                if (obj == null)
                {
                    var tmp = _repository.GetRepository<SystemInformation>().GetAll().FirstOrDefault();
                    if (tmp == null)
                        _SystemInformation = new SystemInformation();
                    else
                        _SystemInformation = tmp;
                }
                else
                    _SystemInformation = obj;
                return _SystemInformation;
            }
            set { _cacheFactory.SaveCache("SystemInformation", value); }
        }
    }
}