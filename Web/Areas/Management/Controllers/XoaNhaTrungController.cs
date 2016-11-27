using Common;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
using Web.Areas.Management.Helpers;
using Web.Helpers;

namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("quan-ly-du-lieu")]
    public class XoaNhaTrungController : BaseController
    {
        [Route("xoa-nha-trung", Name = "XoaNhaTrungIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.XoaNhaTrung)]
        public ActionResult Index()
        {
            return View();
        }


        [Route("danh-sach-nha-trung-json", Name = "XoaNhaTrungGetXoaNhaTrungJson")]
        public ActionResult GetNhaJson()
        {
            try
            {
                string drawReturn = "1";

                int skip = 0;
                int take = 10;
                int status = 0;

                string start = Request.Params["start"];//Đang hiển thị từ bản ghi thứ mấy
                string length = Request.Params["length"];//Số bản ghi mỗi trang
                string draw = Request.Params["draw"];//Số lần request bằng ajax (hình như chống cache)
                string key = Request.Params["search[value]"];//Ô tìm kiếm            
                string orderDir = Request.Params["order[0][dir]"];//Trạng thái sắp xếp xuôi hay ngược: asc/desc
                orderDir = string.IsNullOrEmpty(orderDir) ? "asc" : orderDir;
                string orderColumn = Request.Params["order[0][column]"];//Cột nào đang được sắp xếp (cột thứ mấy trong html table)
                orderColumn = string.IsNullOrEmpty(orderColumn) ? "1" : orderColumn;
                string orderKey = Request.Params["columns[" + orderColumn + "][data]"];//Lấy tên của cột đang được sắp xếp
                orderKey = string.IsNullOrEmpty(orderKey) ? "UpdateDate" : orderKey;

                if (!string.IsNullOrEmpty(start))
                    skip = Convert.ToInt16(start);
                if (!string.IsNullOrEmpty(length))
                    take = Convert.ToInt16(length);
                if (!string.IsNullOrEmpty(draw))
                    drawReturn = draw;

                string objectStatus = Request.Params["objectStatus"];//Lọc trạng thái bài viết
                if (!string.IsNullOrEmpty(objectStatus))
                    int.TryParse(objectStatus.ToString(), out status);
                Paging paging = new Paging()
                {
                    TotalRecord = 0,
                    Skip = skip,
                    Take = take,
                    OrderDirection = orderDir
                };

                //Get all nhà
                var nha = _repository.GetRepository<Nha>().GetAll()
                                    .Join(_repository.GetRepository<Quan>().GetAll(), b => b.QuanId, e => e.Id, (b, e) => new { Nha = b, Quan = e })
                                       .Join(_repository.GetRepository<Duong>().GetAll(), b => b.Nha.DuongId, e => e.Id, (b, e) => new { Nha = b, Duong = e }).ToList();

                List<object> objResult = new List<object>();

                for (int i = nha.Count - 1; i >= 0; i--)
                {
                    string soDienThoaiCheck = nha[i].Nha.Nha.SoDienThoai;
                    long quanCheck = nha[i].Nha.Nha.QuanId;
                    long duongCheck = nha[i].Nha.Nha.DuongId;

                    //Get các bản records duplicate theo cặp
                    var itemDuplicateList = nha.Where(t => t.Nha.Nha.SoDienThoai == soDienThoaiCheck &&
                                                      t.Nha.Nha.QuanId == quanCheck &&
                                                      t.Nha.Nha.DuongId == duongCheck).ToList();

                    //Kiểm tra nếu có trùng thì ném vào list kết quả
                    if (itemDuplicateList.Count > 1)
                    {
                        foreach (var itemDup in itemDuplicateList)
                        {
                            objResult.Add(new
                            {
                                Id = itemDup.Nha.Nha.Id,
                                TenNguoiLienHeVaiTro = itemDup.Nha.Nha.TenNguoiLienHeVaiTro,
                                SoDienThoai = itemDup.Nha.Nha.SoDienThoai,
                                Quan = itemDup.Nha.Quan.Name,
                                Duong = itemDup.Duong.Name,
                                TrangThai = itemDup.Nha.Nha.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt"
                            });


                        }
                        //Xóa thằng đã lấy được ra khỏi list kiểm tra
                        nha.RemoveAll(t => t.Nha.Nha.SoDienThoai == soDienThoaiCheck &&
                                      t.Nha.Nha.QuanId == quanCheck &&
                                      t.Nha.Nha.DuongId == duongCheck);
                    }
                }

                return Json(new
                {
                    draw = drawReturn,
                    recordsTotal = objResult.Count,
                    recordsFiltered = objResult.Count,
                    data = objResult
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost]
        [Route("xoa-nha/{ids?}", Name = "NhaTrungDeleteNhaTrung")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> DeleteNhas(string ids)
        {
            try
            {
                byte succeed = 0;
                string[] articleIds = Regex.Split(ids, ",");
                if (articleIds != null && articleIds.Any())
                    foreach (var item in articleIds)
                    {
                        long articleId = 0;
                        long.TryParse(item, out articleId);
                        bool result = await DeleteNha(articleId);
                        if (result)
                            succeed++;
                    }
                return Json(new { success = true, message = string.Format(@"Đã xóa thành công {0} bản ghi.", succeed) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không xóa được bài viết. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<bool> DeleteNha(long articleId)
        {
            var article = await _repository.GetRepository<Nha>().ReadAsync(articleId);
            if (article == null)
                return false;

            int result = await _repository.GetRepository<Nha>().DeleteAsync(article, AccountId);

            if (result > 0)
            {
                return true;
            }

            return false;
        }
    }
}