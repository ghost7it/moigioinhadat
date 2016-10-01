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
    [RoutePrefix("danh-muc-tinh-tp")]
    public class ProvinceController : BaseController
    {
        [Route("quan-ly-danh-muc-tinh-tp", Name = "ProvinceIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Province)]
        public ActionResult Index()
        {
            return View();
        }
        [Route("danh-sach-tinh-tp", Name = "ProvinceListProvince")]
        public ActionResult ListProvince()
        {
            IEnumerable<Province> provinces = _repository.GetRepository<Province>().GetAll();
            return PartialView("_ListProvince", provinces);
        }
        [Route("danh-sach-quan-huyen", Name = "ProvinceListDistrict")]
        public ActionResult ListDistrict(long id)
        {
            ViewBag.ProvinceId = id;
            Province province = _repository.GetRepository<Province>().Read(id);
            if (province != null)
                ViewBag.ProvinceName = province.Name;
            IEnumerable<District> districts = _repository.GetRepository<District>().GetAll(o => o.ProvinceId == id);
            return PartialView("_ListDistrict", districts);
        }
        [Route("danh-sach-tinh-thanh-pho-json", Name = "ProvinceGetProvinceJson")]
        public ActionResult GetProvinceJson(string parent)
        {
            if (string.IsNullOrEmpty(parent) || parent == "#")
            {
                var json = new
                {
                    id = "root_#",
                    text = "Danh sách tỉnh/thành phố - quận/huyện",
                    icon = "fa fa-folder icon-lg icon-state-success",
                    state = new
                    {
                        opened = true,
                        disabled = false,
                        selected = false
                    },
                    children = true,
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }
        [Route("danh-sach-quan-huyen-json", Name = "ProvinceGetDistrictJson")]
        public async Task<JsonResult> GetDistrictJson(string parent)
        {
            var tmp = parent.Split('_');
            if (tmp[0] == "root")
            {
                var provinces = await _repository.GetRepository<Province>().GetAllAsync();
                if (provinces != null && provinces.Any())
                {
                    var json = provinces.OrderBy(o => o.Name).Select(o => new
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
                        children = o.Districts != null && o.Districts.Any() ? true : false,
                    }).ToList();
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                long id = Convert.ToInt64(tmp[1]);
                var districts = await _repository.GetRepository<District>().GetAllAsync(o => o.ProvinceId == id);
                if (districts != null && districts.Any())
                {
                    var json = districts.OrderBy(o => o.Name).Select(o => new
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
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("cap-nhat-tinh-tp", Name = "ProvinceCreateOrUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Province)]
        public async Task<JsonResult> CreateOrUpdate(Province model)
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
                        Province province = new Province();
                        province.Name = model.Name;
                        result = await _repository.GetRepository<Province>().CreateAsync(province, AccountId);
                        id = province.Id;
                    }
                    else
                    {
                        Province province = await _repository.GetRepository<Province>().ReadAsync(model.Id);
                        province.Name = model.Name;
                        result = await _repository.GetRepository<Province>().UpdateAsync(province, AccountId);
                        id = province.Id;
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
        [Route("xoa-tinh-tp", Name = "ProvinceDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Province)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Province province = await _repository.GetRepository<Province>().ReadAsync(id);
                if (province != null)
                {
                    if (province.Districts != null && province.Districts.Any())
                    {
                        return Json(new { success = false, message = "Vui lòng xóa quận/huyện trước!" }, JsonRequestBehavior.AllowGet);
                    }
                    int result = await _repository.GetRepository<Province>().DeleteAsync(province, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được tỉnh/thành phố! Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy tỉnh/thành phố!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("cap-nhat-quan-huyen", Name = "DistrictCreateOrUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Province)]
        public async Task<JsonResult> CreateOrUpdateDistrict(District model)
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
                        District district = new District();
                        district.Name = model.Name;
                        district.ProvinceId = model.ProvinceId;
                        result = await _repository.GetRepository<District>().CreateAsync(district, AccountId);
                        id = district.Id;
                    }
                    else
                    {
                        District province = await _repository.GetRepository<District>().ReadAsync(model.Id);
                        province.Name = model.Name;
                        result = await _repository.GetRepository<District>().UpdateAsync(province, AccountId);
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
        [Route("xoa-quan-huyen", Name = "DistrictDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Province)]
        public async Task<JsonResult> DeleteDistrict(long id)
        {
            try
            {
                District district = await _repository.GetRepository<District>().ReadAsync(id);
                if (district != null)
                {
                    int result = await _repository.GetRepository<District>().DeleteAsync(district, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được quận/huyện! Vui lòng kiểm tra lại." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy quận/huyện!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}