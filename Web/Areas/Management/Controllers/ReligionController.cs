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
    [RoutePrefix("danh-muc-ton-giao")]
    public class ReligionController : BaseController
    {
        [Route("quan-ly-danh-muc-ton-giao", Name = "ReligionIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Religion)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Religion> religions = await _repository.GetRepository<Religion>().GetAllAsync();
            return View(religions);
        }
        [Route("nhap-ton-giao", Name = "ReligionCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Religion)]
        public async Task<JsonResult> Create(Religion model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    int result = await _repository.GetRepository<Religion>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được tôn giáo!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-ton-giao", Name = "ReligionUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Religion)]
        public async Task<JsonResult> Update(Religion model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Religion religion = await _repository.GetRepository<Religion>().ReadAsync(model.Id);
                    if (religion == null)
                        return Json(new { success = false, message = "Không tìm thấy tôn giáo!" }, JsonRequestBehavior.AllowGet);
                    religion.Name = StringHelper.KillChars(model.Name);
                    int result = await _repository.GetRepository<Religion>().UpdateAsync(religion, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được tôn giáo!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-ton-giao/{id?}", Name = "ReligionDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Religion)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Religion categoryType = await _repository.GetRepository<Religion>().ReadAsync(id);
                if (categoryType != null)
                {
                    int result = await _repository.GetRepository<Religion>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được tôn giáo!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy tôn giáo!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}