using Common;
using Entities.Enums;
using Entities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("danh-muc-don-vi")]
    public class OrganizationController : BaseController
    {
        [Route("quan-ly-danh-muc-don-vi", Name = "OrganizationIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Organization)]
        public ActionResult Index()
        {
            return View();
        }
        [Route("danh-sach-don-vi-json", Name = "OrganizationGetOrganizationJson")]
        public async Task<JsonResult> GetOrganizationJson(string parent)
        {
            var tmp = parent.Split('_');
            if (string.IsNullOrEmpty(parent) || parent == "#")
            {
                var organizations = await _repository.GetRepository<Organization>().GetAllAsync(o => o.OrganizationId == null || o.OrganizationId == 0);

                var json = organizations.OrderBy(o => o.Name).Select(o => new
                {
                    id = "node_" + o.Id,
                    text = o.Name,
                    icon = "fa fa-folder icon-lg icon-state-success",
                    state = new
                    {
                        opened = false,
                        disabled = false,
                        selected = false
                    },
                    children = o.Organizations != null && o.Organizations.Any() ? true : false,
                }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                long id = Convert.ToInt64(tmp[1]);
                var categories = await _repository.GetRepository<Organization>().GetAllAsync(o => o.OrganizationId == id);
                var json = categories.OrderBy(o => o.Name).Select(o => new
                {
                    id = "node_" + o.Id,
                    parent = parent,
                    text = o.Name,
                    icon = "fa fa-folder icon-lg icon-state-success",
                    state = new
                    {
                        opened = false,
                        disabled = false,
                        selected = false
                    },
                    children = o.Organizations != null && o.Organizations.Any() ? true : false,
                }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Route("cap-nhat-don-vi", Name = "OrganizationCreateOrUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Organization)]
        public async Task<JsonResult> CreateOrUpdate(Organization model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    long id = 0;
                    int result = 0;
                    model.Name = StringHelper.KillChars(model.Name);
                    if (model.Id == 0)
                    {
                        Organization organization = new Organization();
                        organization.Name = model.Name;
                        organization.IsApproved = true;
                        organization.OrganizationId = model.OrganizationId;
                        result = await _repository.GetRepository<Organization>().CreateAsync(organization, AccountId);
                        id = organization.Id;
                    }
                    else
                    {
                        Organization organization = await _repository.GetRepository<Organization>().ReadAsync(model.Id);
                        organization.Name = model.Name;
                        organization.OrganizationId = model.OrganizationId;
                        result = await _repository.GetRepository<Organization>().UpdateAsync(organization, AccountId);
                        id = organization.Id;
                    }
                    if (result > 0)
                    {
                        return Json(new { success = true, id = id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không ghi nhận được dữ liệu! Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi:" + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ và chính xác các thông tin!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Route("xoa-don-vi", Name = "OrganizationDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Organization)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Organization organization = await _repository.GetRepository<Organization>().ReadAsync(id);
                if (organization == null)
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy đơn vị!" }, JsonRequestBehavior.AllowGet);
                if (organization.IsSystemOwner)
                    return Json(new { success = false, message = "Lỗi: Không được phép xóa đơn vị này!" }, JsonRequestBehavior.AllowGet);
                var root = GetRootOrganization(organization);
                if (root.IsSystemOwner)
                    return Json(new { success = false, message = "Lỗi: Không được phép xóa đơn vị này!" }, JsonRequestBehavior.AllowGet);
                if (organization.Majors != null && organization.Majors.Any())
                    return Json(new { success = false, message = "Lỗi: Không được phép xóa đơn vị này!" }, JsonRequestBehavior.AllowGet);
                if (organization.Organizations != null && organization.Organizations.Any())
                    return Json(new { success = false, message = "Lỗi: Vui lòng xóa đơn vị con trước!" }, JsonRequestBehavior.AllowGet);
                int result = await _repository.GetRepository<Organization>().DeleteAsync(organization, AccountId);
                if (result > 0)
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = "Lỗi: Không xóa được đơn vị!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi:" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Route("cap-nhat-don-vi-parent", Name = "OrganizationChangeParent")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Organization)]
        public async Task<JsonResult> ChangeParent(long id, long organizationId)
        {
            try
            {
                int result = 0;
                Organization organization = await _repository.GetRepository<Organization>().ReadAsync(id);
                organization.OrganizationId = organizationId;
                result = await _repository.GetRepository<Organization>().UpdateAsync(organization, AccountId);
                if (result > 0)
                {
                    return Json(new { success = true, id = id }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Không ghi nhận được dữ liệu! Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi:" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //Lấy đơn vị cấp cha cao nhất
        public Organization GetRootOrganization(Organization organization)
        {
            if (organization.ParentOrganization == null) return organization;
            else
            {
                return GetRootOrganization(organization.ParentOrganization);
            }
        }
    }
}