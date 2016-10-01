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
    [RoutePrefix("danh-muc-chuc-vu")]
    public class PositionController : BaseController
    {
        [Route("quan-ly-danh-muc-chuc-vu", Name = "PositionIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Position)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Position> positions = await _repository.GetRepository<Position>().GetAllAsync();
            return View(positions);
        }
        [Route("nhap-chuc-vu", Name = "PositionCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Position)]
        public async Task<JsonResult> Create(Position model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    model.IsApproved = true;
                    int result = await _repository.GetRepository<Position>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được chức vụ!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-chuc-vu", Name = "PositionUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Position)]
        public async Task<JsonResult> Update(Position model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Position position = await _repository.GetRepository<Position>().ReadAsync(model.Id);
                    if (position == null)
                        return Json(new { success = false, message = "Không tìm thấy chức vụ!" }, JsonRequestBehavior.AllowGet);
                    position.Name = StringHelper.KillChars(model.Name);
                    position.IsApproved = true;
                    int result = await _repository.GetRepository<Position>().UpdateAsync(position, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được chức vụ!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-chuc-vu/{id?}", Name = "PositionDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Position)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Position categoryType = await _repository.GetRepository<Position>().ReadAsync(id);
                if (categoryType != null)
                {
                    int result = await _repository.GetRepository<Position>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được chức vụ!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy chức vụ!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}