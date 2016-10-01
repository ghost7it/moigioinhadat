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
    [RoutePrefix("loai-chuyen-muc")]
    public class CategoryTypeController : BaseController
    {
        [Route("quan-ly-loai-chuyen-muc", Name = "CategoryTypeIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.CategoryType)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<CategoryType> categoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
            return View(categoryTypes);
        }
        [Route("nhap-loai-chuyen-muc", Name = "CategoryTypeCreate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.CategoryType)]
        public async Task<JsonResult> Create(CategoryType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = StringHelper.KillChars(model.Name);
                    model.Description = StringHelper.KillChars(model.Description);
                    int result = await _repository.GetRepository<CategoryType>().CreateAsync(model, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không nhập được loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
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
        [Route("cap-nhat-loai-chuyen-muc", Name = "CategoryTypeUpdate")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.CategoryType)]
        public async Task<JsonResult> Update(CategoryType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CategoryType categoryType = await _repository.GetRepository<CategoryType>().ReadAsync(model.Id);
                    if (categoryType == null)
                        return Json(new { success = false, message = "Không tìm thấy loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
                    categoryType.Name = StringHelper.KillChars(model.Name);
                    categoryType.Description = StringHelper.KillChars(model.Description);
                    int result = await _repository.GetRepository<CategoryType>().UpdateAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, id = model.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không cập nhật được loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
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
        [Route("xoa-loai-chuyen-muc/{id?}", Name = "CategoryTypeDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.CategoryType)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                CategoryType categoryType = await _repository.GetRepository<CategoryType>().ReadAsync(id);
                if (categoryType != null)
                {
                    if (categoryType.Categories != null && categoryType.Categories.Any())
                    {
                        //Nếu loại chuyên mục đang có các chuyên mục thì không cho xóa
                        return Json(new { success = false, message = "Bạn phải xóa các chuyên mục trước!" });
                    }
                    int result = await _repository.GetRepository<CategoryType>().DeleteAsync(categoryType, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}