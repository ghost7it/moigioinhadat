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
    [RoutePrefix("danh-muc-hoc-ham")]
    public class RankController : BaseController
    {
        [Route("quan-ly-danh-muc-hoc-ham", Name = "RankIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Rank)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Rank> ranks = await _repository.GetRepository<Rank>().GetAllAsync();
            return View(ranks);
        }
        [Route("nhap-hoc-ham", Name = "RankCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Rank)]
        public async Task<JsonResult> Create(Rank model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    model.Code = StringHelper.KillChars(model.Code);
                    int result = await _repository.GetRepository<Rank>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được học hàm!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-hoc-ham", Name = "RankUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Rank)]
        public async Task<JsonResult> Update(Rank model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Rank rank = await _repository.GetRepository<Rank>().ReadAsync(model.Id);
                    if (rank == null)
                        return Json(new { success = false, message = "Không tìm thấy học hàm!" }, JsonRequestBehavior.AllowGet);
                    rank.Name = StringHelper.KillChars(model.Name);
                    rank.Code = StringHelper.KillChars(model.Code);
                    int result = await _repository.GetRepository<Rank>().UpdateAsync(rank, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được học hàm!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-hoc-ham/{id?}", Name = "RankDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Rank)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Rank categoryType = await _repository.GetRepository<Rank>().ReadAsync(id);
                if (categoryType != null)
                {
                    int result = await _repository.GetRepository<Rank>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được học hàm!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy học hàm!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}