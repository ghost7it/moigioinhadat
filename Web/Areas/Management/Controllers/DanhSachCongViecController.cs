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
    [RoutePrefix("quan-ly-cong-viec")]
    public class DanhSachCongViecController : BaseController
    {
        ListFieldHidden _lstfieldhidden = new ListFieldHidden();
        [Route("danh-sach-cong-viec", Name = "DanhSachCongViecIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.DanhSachCongViec)]
        public ActionResult Index()
        {
            return View();
        }

        protected void SetViewBag(bool isCreate)
        {
            //Mặt bằng
            var matBang = _repository.GetRepository<MatBang>().GetAll();
            ViewBag.MatBang = matBang.ToList().ToSelectList();

            //Địa chỉ quận - đường
            NhaCreatingViewModel model = new NhaCreatingViewModel();
            var quan = _repository.GetRepository<Quan>().GetAll().OrderBy(o => o.Name).ToList();
            if (isCreate)
            {
                ViewBag.QuanDropdownlist = new SelectList(quan, "Id", "Name", model.QuanId);
                ViewBag.DuongDropdownlist = new SelectList(_repository.GetRepository<Duong>().GetAll(o => o.QuanId == model.QuanId).OrderBy(o => o.Name).ToList(), "Id", "Name", model.DuongId);
            }
            else
            {
                var quanUpdate = _repository.GetRepository<Duong>().GetAll();
                ViewBag.QuanDropdownlist = quan.ToList().ToSelectList();

                var duong = _repository.GetRepository<Duong>().GetAll();
                ViewBag.DuongDropdownlist = duong.ToList().ToSelectList();
            }

            var listYesOrNo = new SelectList(new[] 
            {
                new { ID = "0", Name = "Không" },
                new { ID = "1", Name = "Có" },
            }, "ID", "Name", 1);

            ViewBag.ListYesOrNo = listYesOrNo;

            //Nội thất khách thuê cũ
            var noiThatKhachThueCu = _repository.GetRepository<NoiThatKhachThueCu>().GetAll();
            ViewBag.NoiThatKhachThueCu = noiThatKhachThueCu.ToList().ToSelectList();

            //Đánh giá phù hợp với
            var danhGiaPhuHopVoi = _repository.GetRepository<DanhGiaPhuHopVoi>().GetAll();
            ViewBag.DanhGiaPhuHopVoi = danhGiaPhuHopVoi.ToList().ToSelectList();

            //Cấp độ theo dõi
            var capDoTheoDoi = _repository.GetRepository<CapDoTheoDoi>().GetAll();
            ViewBag.CapDoTheoDoi = capDoTheoDoi.ToList().ToSelectList();
        }

        [Route("danh-sach-cong-viec-json", Name = "DanhSachCongViecGetDanhSachCongViecJson")]
        public ActionResult GetDanhSachCongViecJson(int status)
        {
            try
            {
                string drawReturn = "1";

                int skip = 0;
                int take = 10;

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

                var articles = _repository.GetRepository<QuanLyCongViec>().GetAll(ref paging,
                                                                                  orderKey,
                                                                                  o => (key == null ||
                                                                                  key == "") &&
                                                                                  (o.TrangThai == status) &&
                                                                                  (o.TrangThai != 0) && 
                                                                                  (o.NhanVienPhuTrachId == AccountId))
                                                                          .Join(_repository.GetRepository<NhuCauThue>().GetAll(), b => b.KhachId, e => e.Id, (b, e) => new { QuanLyCongViec = b, NhuCauThue = e })
                                                                          .Join(_repository.GetRepository<Quan>().GetAll(), b => b.NhuCauThue.QuanId, e => e.Id, (b, e) => new { NhuCauThue = b, Quan = e })
                                                                          .Join(_repository.GetRepository<Duong>().GetAll(), b => b.NhuCauThue.NhuCauThue.DuongId, g => g.Id, (b, g) => new { NhuCauThue = b, Duong = g })
                                                                          .Join(_repository.GetRepository<CapDoTheoDoi>().GetAll(), b => b.NhuCauThue.NhuCauThue.NhuCauThue.CapDoTheoDoiId, y => y.Id, (b, y) => new { NhuCauThue = b, CapDoTheoDoi = y })
                                                                          .Join(_repository.GetRepository<Khach>().GetAll(), b => b.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.KhachId, y => y.Id, (b, y) => new { QuanLyCongViec = b, Khach = y }).ToList();


                return Json(new
                {
                    draw = drawReturn,

                    recordsTotal = paging.TotalRecord,
                    recordsFiltered = paging.TotalRecord,
                    data = articles.Select(o => new
                    {
                        o.QuanLyCongViec.NhuCauThue.NhuCauThue.NhuCauThue.QuanLyCongViec.Id,
                        IsWarning = o.QuanLyCongViec.NhuCauThue.NhuCauThue.NhuCauThue.QuanLyCongViec.NgayHoanThanh <= DateTime.Now ? true : false,
                        o.QuanLyCongViec.NhuCauThue.NhuCauThue.NhuCauThue.QuanLyCongViec.NgayHoanThanh,
                        o.Khach.TenNguoiLienHeVaiTro,
                        o.Khach.SoDienThoai,
                        Quan = o.QuanLyCongViec.NhuCauThue.NhuCauThue.Quan.Name,
                        Duong = o.QuanLyCongViec.NhuCauThue.Duong.Name,
                        o.QuanLyCongViec.NhuCauThue.NhuCauThue.NhuCauThue.QuanLyCongViec.NoiDungCongViec,
                        TrangThai = o.QuanLyCongViec.NhuCauThue.NhuCauThue.NhuCauThue.QuanLyCongViec.TrangThai == 1 ? "Chưa hoàn thành" : "Đã hoàn thành"
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("xet-trang-thai-danh-sach-cong-viec/{ids?}/{status?}", Name = "DanhSachCongViecSetDanhSachCongViecStatus")]
        public async Task<ActionResult> SetDanhSachCongViecStatus(string ids, byte status)
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
                        bool result = await SetDanhSachCongViecStatus(articleId, status);
                        if (result)
                            succeed++;
                    }
                return Json(new { success = true, message = string.Format(@"Đã ghi nhận thành công trạng thái {0} bản ghi.", succeed) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Xét trạng thái của bài viết
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task<bool> SetDanhSachCongViecStatus(long articleId, byte status)
        {
            var article = await _repository.GetRepository<QuanLyCongViec>().ReadAsync(articleId);
            if (article == null)
                return false;
            article.TrangThai = status;

            int result = await _repository.GetRepository<QuanLyCongViec>().UpdateAsync(article, AccountId);

            if (result > 0)
            {
                //TODO: HuyTQ - Lưu lịch sử thay đổi
                return true;
            }
            return false;
        }

        /// <summary>
        /// Xem chi tiết bài viết theo modal dialog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("xem-chi-tiet-cong-viec-modal/{id?}", Name = "DanhSachCongViecDetailModal")]
        public async Task<ActionResult> DetailModal(long id)
        {
            var article = await _repository.GetRepository<QuanLyCongViec>().ReadAsync(id);
            if (article != null)
            {
                var khach = await _repository.GetRepository<Khach>().ReadAsync(article.KhachId);
                if (khach != null)
                {
                    ViewBag.TenKhach = khach.TenNguoiLienHeVaiTro;
                    ViewBag.SoDienThoaiKhach = khach.SoDienThoai;
                }

                var nhucauthue = await _repository.GetRepository<NhuCauThue>().ReadAsync(article.NhuCauThueId);
                if (nhucauthue != null)
                {
                    ViewBag.QuanName = nhucauthue.QuanName;
                    ViewBag.DuongName = nhucauthue.DuongName;
                    string[] matbangarr = nhucauthue.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (matbangarr.Count() > 0)
                    {
                        for (var i = 0; i < matbangarr.Count(); i++)
                        {
                            if (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i])) != null)
                            {
                                ViewBag.LoaiMatBang += (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i]))).Name;
                                if (matbangarr[i] != matbangarr[matbangarr.Count() - 1])
                                {
                                    ViewBag.LoaiMatBang += @"</br>";
                                }
                            }
                        }
                    }
                    ViewBag.MatTienTreoBien = nhucauthue.MatTienTreoBien + " (m)";
                    ViewBag.BeNgangLotLong = nhucauthue.BeNgangLotLong + " (m)";
                    ViewBag.DienTichDat = nhucauthue.DienTichDat + " (m2)";
                    ViewBag.DienTichDatSuDungTang1 = nhucauthue.DienTichDatSuDungTang1 + " (m2)";
                    ViewBag.TongDienTichSuDung = nhucauthue.TongDienTichSuDung + " (m2)";
                    ViewBag.SoTang = nhucauthue.SoTang;
                    ViewBag.DiChungChu = nhucauthue.DiChungChu ? "Có" : "Không";
                    ViewBag.Ham = nhucauthue.Ham ? "Có" : "Không";
                    ViewBag.ThangMay = nhucauthue.ThangMay ? "Có" : "Không";
                    var noithat = await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(nhucauthue.NoiThatKhachThueCuId);
                    if (noithat != null)
                    {
                        ViewBag.NoiThat = noithat.Name;
                    }

                    ViewBag.TongGiaThue = nhucauthue.TongGiaThue + " (triệu VNĐ)";
                    ViewBag.GiaThueBQ = nhucauthue.GiaThueBQ + " (triệu VNĐ/m2)";
                }

                var nha = await _repository.GetRepository<Nha>().ReadAsync(article.NhaId);
                if (nha != null)
                {
                    string[] matbangarr = nha.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (matbangarr.Count() > 0)
                    {
                        for (var i = 0; i < matbangarr.Count(); i++)
                        {
                            if (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i])) != null)
                            {
                                ViewBag.LoaiMatBang1 += (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i]))).Name;
                                if (matbangarr[i] != matbangarr[matbangarr.Count() - 1])
                                {
                                    ViewBag.LoaiMatBang1 += @"</br>";
                                }
                            }
                        }
                    }
                    var quan = await _repository.GetRepository<Quan>().ReadAsync(nha.QuanId);
                    if (quan != null)
                    {
                        ViewBag.QuanName1 = quan.Name;
                    }
                    var duong = await _repository.GetRepository<Duong>().ReadAsync(nha.DuongId);
                    if (duong != null)
                    {
                        ViewBag.DuongName1 = duong.Name;
                    }

                    ViewBag.SoNha = nha.SoNha;
                    ViewBag.TenToaNha = nha.TenToaNha;
                    ViewBag.TenChuNha = nha.TenNguoiLienHeVaiTro;
                    ViewBag.SoDienThoai1 = nha.SoDienThoai;
                    ViewBag.MatTienTreoBien1 = nha.MatTienTreoBien + " (m)";
                    ViewBag.BeNgangLotLong1 = nha.BeNgangLotLong + " (m)";
                    ViewBag.DienTichDat1 = nha.DienTichDat + " (m2)";
                    ViewBag.DienTichDatSuDungTang11 = nha.DienTichDatSuDungTang1 + " (m2)";
                    ViewBag.SoTang1 = nha.SoTang;
                    ViewBag.TongDienTichSuDung1 = nha.TongDienTichSuDung + " (m2)";
                    ViewBag.DiChungChu1 = nha.DiChungChu ? "Có" : "Không";
                    ViewBag.Ham1 = nha.Ham ? "Có" : "Không";
                    ViewBag.ThangMay1 = nha.ThangMay ? "Có" : "Không";
                    var noithat1 = await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(nha.NoiThatKhachThueCuId);
                    if (noithat1 != null)
                    {
                        ViewBag.NoiThat1 = noithat1.Name;
                    }

                    ViewBag.TongGiaThue1 = nha.TongGiaThue + " (triệu VNĐ)";
                    ViewBag.GiaThueBQ1 = nha.GiaThueBQ + " (triệu VNĐ/m2)";
                }

                var acc = await _repository.GetRepository<Account>().ReadAsync(article.NhanVienPhuTrachId);
                if (acc != null)
                {
                    ViewBag.AccoutName = acc.Name;
                }

                //Hide field by config
                int isHideSoNha = 0; int isHideTenToaNha = 0; int isHideSoDienThoai = 0; int isHideNguoiLienHe = 0;
                int isHideDienTichDat = 0; int isHideDienTichDatSuDungTang1 = 0; int isHideTongDienTichSuDung = 0;
                int isHideSoTang = 0; int isHideDiChungChu = 0; int isHideHam = 0; int isHideThangMay = 0;

                List<FieldHidden> listNha = _lstfieldhidden.lst;
                if (!string.IsNullOrEmpty(article.NhaHiddenField))
                {
                    foreach (var item in article.NhaHiddenField.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        switch (item)
                        {
                            case "SoNha":
                                isHideSoNha = 1;
                                break;
                            case "TenToaNha":
                                isHideTenToaNha = 1;
                                break;
                            case "SoDienThoai":
                                isHideSoDienThoai = 1;
                                break;
                            case "NguoiLienHe":
                                isHideNguoiLienHe = 1;
                                break;
                            case "DienTichDat":
                                isHideDienTichDat = 1;
                                break;
                            case "DienTichDatSuDungTang1":
                                isHideDienTichDatSuDungTang1 = 1;
                                break;
                            case "TongDienTichSuDung":
                                isHideTongDienTichSuDung = 1;
                                break;
                            case "SoTang":
                                isHideSoTang = 1;
                                break;
                            case "DiChungChu":
                                isHideDiChungChu = 1;
                                break;
                            case "Ham":
                                isHideHam = 1;
                                break;
                            case "ThangMay":
                                isHideThangMay = 1;
                                break;
                        }
                    }

                }

                ViewBag.IsHideSoNha = isHideSoNha;
                ViewBag.IsHideTenToaNha = isHideTenToaNha;
                ViewBag.isHideSoDienThoai = isHideSoDienThoai;
                ViewBag.isHideNguoiLienHe = isHideNguoiLienHe;
                ViewBag.IsHideDienTichDat = isHideDienTichDat;
                ViewBag.IsHideDienTichDatSuDungTang1 = isHideDienTichDatSuDungTang1;
                ViewBag.IsHideTongDienTichSuDung = isHideTongDienTichSuDung;
                ViewBag.IsHideSoTang = isHideSoTang;
                ViewBag.IsHideDiChungChu = isHideDiChungChu;
                ViewBag.IsHideHam = isHideHam;
                ViewBag.IsHideThangMay = isHideThangMay;

                ViewBag.NoiDungCongViec = article.NoiDungCongViec;

            }
            return PartialView("_DetailModal");
        }
    }
}