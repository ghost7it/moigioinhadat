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
    [RoutePrefix("danh-muc-dan-toc")]
    public class NationController : BaseController
    {
        [Route("quan-ly-danh-muc-dan-toc", Name = "NationIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Nation)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Nation> nations = await _repository.GetRepository<Nation>().GetAllAsync();
            return View(nations);
        }
        [Route("nhap-dan-toc", Name = "NationCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Nation)]
        public async Task<JsonResult> Create(Nation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    int result = await _repository.GetRepository<Nation>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được dân tộc!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-dan-toc", Name = "NationUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Nation)]
        public async Task<JsonResult> Update(Nation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Nation nation = await _repository.GetRepository<Nation>().ReadAsync(model.Id);
                    if (nation == null)
                        return Json(new { success = false, message = "Không tìm thấy dân tộc!" }, JsonRequestBehavior.AllowGet);
                    nation.Name = StringHelper.KillChars(model.Name);
                    int result = await _repository.GetRepository<Nation>().UpdateAsync(nation, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được dân tộc!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-dan-toc/{id?}", Name = "NationDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Nation)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Nation categoryType = await _repository.GetRepository<Nation>().ReadAsync(id);
                if (categoryType != null)
                {
                    int result = await _repository.GetRepository<Nation>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được dân tộc!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy dân tộc!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}