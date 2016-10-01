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
    [RoutePrefix("danh-muc-quoc-gia")]
    public class CountryController : BaseController
    {
        [Route("quan-ly-danh-muc-quoc-gia", Name = "CountryIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Country)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Country> country = await _repository.GetRepository<Country>().GetAllAsync();
            return View(country);
        }
        [Route("nhap-quoc-gia", Name = "CountryCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Country)]
        public async Task<JsonResult> Create(Country model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    model.Code = StringHelper.KillChars(model.Code);
                    int result = await _repository.GetRepository<Country>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được quốc gia!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-quoc-gia", Name = "CountryUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Country)]
        public async Task<JsonResult> Update(Country model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Country country = await _repository.GetRepository<Country>().ReadAsync(model.Id);
                    if (country == null)
                        return Json(new { success = false, message = "Không tìm thấy quốc gia!" }, JsonRequestBehavior.AllowGet);
                    country.Name = StringHelper.KillChars(model.Name);
                    country.Code = StringHelper.KillChars(model.Code);
                    int result = await _repository.GetRepository<Country>().UpdateAsync(country, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được quốc gia!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-quoc-gia/{id?}", Name = "CountryDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Country)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Country categoryType = await _repository.GetRepository<Country>().ReadAsync(id);
                if (categoryType != null)
                {
                    int result = await _repository.GetRepository<Country>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được quốc gia!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy quốc gia!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}