using Common;
using Entities.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("danh-muc-hoc-vi")]
    public class DegreeController : BaseController
    {
        [Route("quan-ly-danh-muc-hoc-vi", Name = "DegreeIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Degree)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Degree> degrees = await _repository.GetRepository<Degree>().GetAllAsync();
            return View(degrees);
        }
        [Route("nhap-hoc-vi", Name = "DegreeCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Degree)]
        public async Task<JsonResult> Create(Degree model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    model.Code = StringHelper.KillChars(model.Code);
                    int result = await _repository.GetRepository<Degree>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được học vị!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-hoc-vi", Name = "DegreeUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Degree)]
        public async Task<JsonResult> Update(Degree model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Degree degree = await _repository.GetRepository<Degree>().ReadAsync(model.Id);
                    if (degree == null)
                        return Json(new { success = false, message = "Không tìm thấy học vị!" }, JsonRequestBehavior.AllowGet);
                    degree.Name = StringHelper.KillChars(model.Name);
                    degree.Code = StringHelper.KillChars(model.Code);
                    int result = await _repository.GetRepository<Degree>().UpdateAsync(degree, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được học vị!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-hoc-vi/{id?}", Name = "DegreeDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Degree)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Degree categoryType = await _repository.GetRepository<Degree>().ReadAsync(id);
                if (categoryType != null)
                {
                    int result = await _repository.GetRepository<Degree>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được học vị!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy học vị!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}