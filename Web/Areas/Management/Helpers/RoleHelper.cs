using Common;
using Entities.Enums;
using Entities.Models;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace Web.Areas.Management.Helpers
{
    public class RoleHelper
    {

        public static bool CheckPermission(ModuleEnum moduleEnum, ActionEnum actionEnum)
        {
            try
            {
                CacheFactory _cacheFactory = new CacheFactory();
                if (System.Web.HttpContext.Current.Session["AccountId"] == null || System.Web.HttpContext.Current.Session["AccountId"].ToString() == "") return false;
                long accountId = Convert.ToInt64(System.Web.HttpContext.Current.Session["AccountId"].ToString());

                //IRepository _repository = DependencyResolver.Current.GetService<IRepository>();
                //var accountRoles = _repository.GetRepository<AccountRole>().GetAll(o => o.AccountId == accountId);
                var accountRoles = _cacheFactory.GetCache("AccountRoles_" + accountId) as List<AccountRole>;
                if (accountRoles == null || !accountRoles.Any()) return false;

                //var moduleRoles = _repository.GetRepository<ModuleRole>()
                //  .GetAll(o => (accountRoles.Any(p => p.RoleId == o.RoleId)));
                var moduleRoles1 = _cacheFactory.GetCache("ModuleRoles") as List<ModuleRole>;
                var moduleRoles = moduleRoles1.Where(o => accountRoles.Any(p => p.RoleId == o.RoleId));
                if (moduleRoles == null || !moduleRoles.Any()) return false;

                string moduleEnumString = moduleEnum.ToString();
                var tempModuleEnum = moduleRoles.Where(
                    o => o.ModuleCode.Equals(moduleEnumString, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (!tempModuleEnum.Any()) return false;

                switch (actionEnum)
                {
                    case ActionEnum.Read:
                        return tempModuleEnum.FirstOrDefault(a => a.Read == 1) != null;
                    case ActionEnum.Create:
                        return tempModuleEnum.FirstOrDefault(a => a.Create == 1) != null;
                    case ActionEnum.Update:
                        return tempModuleEnum.FirstOrDefault(a => a.Update == 1) != null;
                    case ActionEnum.Delete:
                        return tempModuleEnum.FirstOrDefault(a => a.Delete == 1) != null;
                    case ActionEnum.Verify:
                        return tempModuleEnum.FirstOrDefault(a => a.Verify == 1) != null;
                    case ActionEnum.Publish:
                        return tempModuleEnum.FirstOrDefault(a => a.Publish == 1) != null;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}