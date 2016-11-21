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

                var nha = _repository.GetRepository<Nha>().GetAll()
                                    .Join(_repository.GetRepository<Quan>().GetAll(), b => b.QuanId, e => e.Id, (b, e) => new { Nha = b, Quan = e })
                                    .Join(_repository.GetRepository<CapDoTheoDoi>().GetAll(), b => b.Nha.CapDoTheoDoiId, y => y.Id, (b, y) => new { Nha = b, CapDoTheoDoi = y }).ToList();

                var duplicateKeys = nha.GroupBy(x => x)
                         .Where(group => group.Count() > 1)
                         .Select(group => group.Key);

                return Json(new
                {
                    draw = drawReturn,
                    recordsTotal = paging.TotalRecord,
                    recordsFiltered = paging.TotalRecord,
                    data = duplicateKeys.Select(o => new
                    {
                        o.Nha.Nha.Id,
                        o.Nha.Quan.Name,
                        o.Nha.Nha.TenNguoiLienHeVaiTro,
                        o.Nha.Nha.SoDienThoai,
                        o.Nha.Nha.TongGiaThue,
                        CapDoTheoDoi = o.CapDoTheoDoi.Name,
                        TrangThai = o.Nha.Nha.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt"
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}