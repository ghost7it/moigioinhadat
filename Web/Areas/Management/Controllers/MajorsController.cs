using Common;
using Entities.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("danh-muc-nganh-khoa")]    
    public class MajorsController : BaseController
    {
        [Route("quan-ly-danh-muc-nganh-khoa", Name = "MajorsIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Majors)]
        public ActionResult Index()
        {
            return View();
        }
        [Route("danh-sach-nganh", Name = "MajorsListMajors")]
        public ActionResult ListMajors(long id)
        {
            ViewBag.OrganizationId = id;
            Organization organization = _repository.GetRepository<Organization>().Read(id);
            if (organization != null)
                ViewBag.OrganizationName = organization.Name;
            IEnumerable<Majors> majorss = _repository.GetRepository<Majors>().GetAll(o => o.OrganizationId == id);
            return PartialView("_ListMajors", majorss);
        }
        [Route("danh-sach-khoa", Name = "MajorsListCourse")]
        public ActionResult ListCourse(long id)
        {
            ViewBag.MajorsId = id;
            Majors majors = _repository.GetRepository<Majors>().Read(id);
            if (majors != null)
                ViewBag.MajorsName = majors.Name;
            IEnumerable<Course> courses = _repository.GetRepository<Course>().GetAll(o => o.MajorsId == id);
            return PartialView("_ListCourse", courses);
        }
        [Route("danh-sach-truong-json", Name = "MajorsGetOrganizationJson")]
        public async Task<JsonResult> GetOrganizationJson(string parent)
        {
            var organizations = await _repository.GetRepository<Organization>().GetAllAsync(o => o.ParentOrganization.IsSystemOwner == true);
            var json = organizations.OrderBy(o => o.Name).Select(o => new
            {
                id = "root_" + o.Id,
                text = o.Name,
                icon = "fa fa-folder icon-lg icon-state-success",
                state = new
                {
                    opened = false,
                    disabled = false,
                    selected = false
                },
                children = o.Majors != null && o.Majors.Any() ? true : false,
            }).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [Route("danh-sach-nganh-khoa-json", Name = "MajorsGetMajorsCourseJson")]
        public async Task<JsonResult> GetMajorsCourseJson(string parent)
        {
            var tmp = parent.Split('_');
            long id = Convert.ToInt64(tmp[1]);
            if (tmp[0] == "root")
            {
                var majorss = await _repository.GetRepository<Majors>().GetAllAsync(o => o.OrganizationId == id);
                var json = majorss.OrderBy(o => o.Name).Select(o => new
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
                    children = o.Courses != null && o.Courses.Any() ? true : false,
                }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var curses = await _repository.GetRepository<Course>().GetAllAsync(o => o.MajorsId == id);
                var json = curses.OrderBy(o => o.Name).Select(o => new
                {
                    id = "subnode_" + o.Id,
                    parent = parent,
                    text = o.Name,
                    icon = "fa fa-folder icon-lg icon-state-success",
                    state = new
                    {
                        opened = false,
                        disabled = true,
                        selected = false
                    },
                    children = false,
                }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Route("cap-nhat-nganh-hoc", Name = "MajorsCreateOrUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Majors)]
        public async Task<JsonResult> CreateOrUpdate(Majors model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    long id = 0;
                    int result = 0;
                    model.Code = StringHelper.KillChars(model.Code);
                    model.Name = StringHelper.KillChars(model.Name);
                    if (model.Id == 0)
                    {
                        Majors majors = new Majors();
                        majors.Code = model.Code;
                        majors.Name = model.Name;
                        majors.OrganizationId = model.OrganizationId;
                        result = await _repository.GetRepository<Majors>().CreateAsync(majors, AccountId);
                        id = majors.Id;
                    }
                    else
                    {
                        Majors majors = await _repository.GetRepository<Majors>().ReadAsync(model.Id);
                        majors.Name = model.Name;
                        majors.Code = model.Code;
                        majors.OrganizationId = model.OrganizationId;
                        result = await _repository.GetRepository<Majors>().UpdateAsync(majors, AccountId);
                        id = majors.Id;
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
        [Route("xoa-nganh-hoc", Name = "MajorsDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Majors)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Majors majors = await _repository.GetRepository<Majors>().ReadAsync(id);
                if (majors != null)
                {
                    if (majors.Courses != null && majors.Courses.Any())
                    {
                        return Json(new { success = false, message = "Vui lòng xóa khóa học trước!" }, JsonRequestBehavior.AllowGet);
                    }
                    int result = await _repository.GetRepository<Majors>().DeleteAsync(majors, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được ngành! Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy ngành!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("cap-nhat-khoa-hoc", Name = "CourseCreateOrUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Majors)]
        public async Task<JsonResult> CreateOrUpdateCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    long id = 0;
                    int result = 0;
                    model.Code = StringHelper.KillChars(model.Code);
                    model.Name = StringHelper.KillChars(model.Name);
                    if (model.Id == 0)
                    {
                        Course course = new Course();
                        course.Code = model.Code;
                        course.Name = model.Name;
                        course.StartYear = model.StartYear;
                        course.FinishYear = model.FinishYear;
                        course.MajorsId = model.MajorsId;
                        result = await _repository.GetRepository<Course>().CreateAsync(course, AccountId);
                        id = course.Id;
                    }
                    else
                    {
                        Course course = await _repository.GetRepository<Course>().ReadAsync(model.Id);
                        course.Code = model.Code;
                        course.Name = model.Name;
                        course.StartYear = model.StartYear;
                        course.FinishYear = model.FinishYear;
                        result = await _repository.GetRepository<Course>().UpdateAsync(course, AccountId);
                        id = course.Id;
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
        [Route("xoa-khoa-hoc", Name = "CourseDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Majors)]
        public async Task<JsonResult> DeleteCourse(long id)
        {
            try
            {
                Course course = await _repository.GetRepository<Course>().ReadAsync(id);
                if (course != null)
                {
                    int result = await _repository.GetRepository<Course>().DeleteAsync(course, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được khóa! Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy khóa!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}