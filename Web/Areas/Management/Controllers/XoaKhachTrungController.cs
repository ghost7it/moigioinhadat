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
    public class XoaKhachTrungController : BaseController
    {
        [Route("xoa-khach-trung", Name = "XoaKhachTrungIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.XoaKhachTrungLap)]
        public ActionResult Index()
        {
            return View();
        }


        [Route("danh-sach-khach-trung-json", Name = "XoaKhachTrungGetXoaKhachTrungJson")]
        public ActionResult GetKhachJson()
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

                //Get all khách và nhu cầu thuê
                var khach = _repository.GetRepository<Khach>().GetAll()
                                .Join(_repository.GetRepository<NhuCauThue>().GetAll(), b => b.Id, e => e.KhachId, (b, e) => new { Khach = b, NhuCauThue = e }).ToList();

                var khachCopy = _repository.GetRepository<Khach>().GetAll()
                                .Join(_repository.GetRepository<NhuCauThue>().GetAll(), b => b.Id, e => e.KhachId, (b, e) => new { Khach = b, NhuCauThue = e }).ToList();

                List<object> objResult = new List<object>();

                //for (int i = khach.Count - 1; i >= 0; i--)
                for (int i = 0; i < khach.Count; i++)
                {
                    string soDienThoaiCheck = khach[i].Khach.SoDienThoai;
                    long quanCheck = khach[i].NhuCauThue.QuanId;
                    long duongCheck = khach[i].NhuCauThue.DuongId;

                    //Get các bản records duplicate theo cặp
                    var itemDuplicateList = khachCopy.Where(t => t.Khach.SoDienThoai == soDienThoaiCheck &&
                                                      t.NhuCauThue.QuanId == quanCheck &&
                                                      t.NhuCauThue.DuongId == duongCheck).ToList();

                    //Kiểm tra nếu có trùng thì ném vào list kết quả
                    if (itemDuplicateList.Count > 1)
                    {
                        foreach (var itemDup in itemDuplicateList)
                        {
                            objResult.Add(new
                            {
                                Id = itemDup.Khach.Id,
                                NhuCauThueId = itemDup.NhuCauThue.Id, 
                                TenNguoiLienHeVaiTro = itemDup.Khach.TenNguoiLienHeVaiTro,
                                SoDienThoai = itemDup.Khach.SoDienThoai,
                                Quan = itemDup.NhuCauThue.QuanName,
                                Duong = itemDup.NhuCauThue.DuongName,
                                TrangThai = itemDup.NhuCauThue.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt"
                            });


                        }
                        //Xóa thằng đã lấy được ra khỏi list kiểm tra
                        khachCopy.RemoveAll(t => t.Khach.SoDienThoai == soDienThoaiCheck &&
                                      t.NhuCauThue.QuanId == quanCheck &&
                                      t.NhuCauThue.DuongId == duongCheck);
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
        [Route("xoa-khach/{ids?}", Name = "KhachTrungDeleteKhachTrung")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> DeleteKhachs(string ids) //id của nhu cầu thuê
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
                        bool result = await DeleteKhach(articleId);
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

        private async Task<bool> DeleteKhach(long articleId)
        {
            var article = await _repository.GetRepository<NhuCauThue>().ReadAsync(articleId);
            if (article == null)
                return false;

            int result = await _repository.GetRepository<NhuCauThue>().DeleteAsync(article, AccountId);

            if (result > 0)
            {
                return true;
            }

            return false;
        }
    }
}