using Interface;
using System.Web.Mvc;
using System;
using System.Web;
using Common;
using Entities.Models;
using System.Linq;
using System.Collections.Generic;
namespace Web.Areas.Management.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IRepository _repository = DependencyResolver.Current.GetService<IRepository>();
        protected readonly CacheFactory _cacheFactory = new CacheFactory();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Email"] == null || Session["Email"].ToString() == "")
            {
                System.Web.HttpContext.Current.Response.RedirectToRoute("Login");
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
        }
        public long AccountId
        {
            get
            {
                object objAccountId = Session["AccountId"];
                long _AccountId = -1;
                if (objAccountId == null)
                    _AccountId = -1;
                else
                    _AccountId = Convert.ToInt64(objAccountId.ToString());
                return _AccountId;
            }
            set { Session["AccountId"] = value; }
        }

        public List<AccountRole> AccountRoles
        {
            get
            {
                List<AccountRole> obj = _cacheFactory.GetCache("AccountRoles_" + AccountId) as List<AccountRole>;

                List<AccountRole> _AccountRoles;
                if (obj == null)
                {
                    var tmp = _repository.GetRepository<AccountRole>().GetAll(o => o.AccountId == AccountId).ToList();
                    if (tmp == null)
                        _AccountRoles = new List<AccountRole>();
                    else
                        _AccountRoles = tmp;
                }
                else
                    _AccountRoles = obj;
                return _AccountRoles;
            }
            set { _cacheFactory.SaveCache("AccountRoles_" + AccountId, value); }
        }

        //public List<ModuleRole> ModuleRoles
        //{
        //    get
        //    {
        //        List<ModuleRole> obj = _cacheFactory.GetCache("ModuleRoles") as List<ModuleRole>;

        //        List<ModuleRole> _ModuleRoles;
        //        if (obj == null)
        //        {
        //            var tmp = _repository.GetRepository<ModuleRole>().GetAll().ToList();
        //            if (tmp == null)
        //                _ModuleRoles = new List<ModuleRole>();
        //            else
        //                _ModuleRoles = tmp;
        //        }
        //        else
        //            _ModuleRoles = obj;
        //        return _ModuleRoles;
        //    }
        //    set { _cacheFactory.SaveCache("ModuleRoles", value); }
        //}
    }
}