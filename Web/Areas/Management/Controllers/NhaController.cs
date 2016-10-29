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
using System.Web.Mvc;
using Web.Areas.Management.Filters;
using Web.Areas.Management.Helpers;
using Web.Helpers;

namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("nha")]
    public class NhaController : BaseController
    {
        [Route("danh-sach-nha", Name = "NhaIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Nha)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("them-moi-nha", Name = "NhaCreate")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Create()
        {
            SetViewBag();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("them-moi-nha")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Create(NhaCreatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                Nha nha = new Nha();

                nha.MatBangId = Convert.ToInt64(model.MatBangId);
                nha.QuanId = Convert.ToInt64(model.QuanId);
                nha.DuongId = Convert.ToInt64(model.DuongId);
                nha.SoNha = StringHelper.KillChars(model.SoNha);
                nha.TenToaNha = StringHelper.KillChars(model.TenToaNha);
                nha.MatTienTreoBien = string.IsNullOrEmpty(model.MatTienTreoBien) ? 0 : float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                nha.BeNgangLotLong = string.IsNullOrEmpty(model.BeNgangLotLong) ? 0 : float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                nha.DienTichDat = string.IsNullOrEmpty(model.DienTichDat) ? 0 : float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                nha.DienTichDatSuDungTang1 = string.IsNullOrEmpty(model.DienTichDatSuDungTang1) ? 0 : float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                nha.SoTang = string.IsNullOrEmpty(model.SoTang) ? 0 : Convert.ToInt32(model.SoTang);
                nha.TongDienTichSuDung = string.IsNullOrEmpty(model.TongDienTichSuDung) ? 0 : float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                nha.DiChungChu = model.DiChungChu == "1" ? true : false;
                nha.Ham = model.Ham == "1" ? true : false;
                nha.ThangMay = model.ThangMay == "1" ? true : false;
                nha.NoiThatKhachThueCuId = Convert.ToInt32(model.NoiThatKhachThueCuId);
                nha.DanhGiaPhuHopVoiId = Convert.ToInt32(model.DanhGiaPhuHopVoiId);
                nha.TongGiaThue = string.IsNullOrEmpty(model.TongGiaThue) ? 0 : Convert.ToDecimal(model.TongGiaThue);
                nha.GiaThueBQ = string.IsNullOrEmpty(model.GiaThueBQ) ? 0 : Convert.ToDecimal(model.GiaThueBQ);
                nha.TenNguoiLienHeVaiTro = StringHelper.KillChars(model.TenNguoiLienHeVaiTro);
                nha.SoDienThoai = StringHelper.KillChars(model.SoDienThoai);
                nha.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : Convert.ToDateTime(model.NgayCNHenLienHeLai);
                nha.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                nha.GhiChu = StringHelper.KillChars(model.GhiChu);
                nha.NgayTao = DateTime.Now;
                nha.NguoiTaoId = AccountId;
                nha.TrangThai = 0; //Chờ duyệt

                int result = 0;
                try
                {
                    result = await _repository.GetRepository<Nha>().CreateAsync(nha, AccountId);
                }
                catch { }
                if (result > 0)
                {
                    TempData["Success"] = "Nhập dữ liệu nhà mới thành công!";
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.DangMatBang = await _repository.GetRepository<MatBang>().GetAllAsync();
                ModelState.AddModelError(string.Empty, "Nhập dữ liệu nhà mới không thành công! Vui lòng kiểm tra và thử lại!");
                return View(model);
            }
        }

        protected void SetViewBag()
        {
            //Mặt bằng
            var matBang = _repository.GetRepository<MatBang>().GetAll();
            ViewBag.MatBang = matBang.ToList().ToSelectList();

            //Địa chỉ quận - đường
            NhaCreatingViewModel model = new NhaCreatingViewModel();
            var quan = _repository.GetRepository<Quan>().GetAll().OrderBy(o => o.Name).ToList();
            ViewBag.QuanDropdownlist = new SelectList(quan, "Id", "Name", model.QuanId);
            ViewBag.DuongDropdownlist = new SelectList(_repository.GetRepository<Duong>().GetAll(o => o.QuanId == model.QuanId).OrderBy(o => o.Name).ToList(), "Id", "Name", model.DuongId);

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


        [Route("danh-sach-nha-json", Name = "NhaGetNhaJson")]
        public ActionResult GetNhaJson(byte status)
        {
            string drawReturn = "1";

            //byte status;

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
                byte.TryParse(objectStatus.ToString(), out status);
            Paging paging = new Paging()
            {
                TotalRecord = 0,
                Skip = skip,
                Take = take,
                OrderDirection = orderDir
            };
            var articles = _repository.GetRepository<Nha>().GetAll(ref paging,
                                                                   orderKey,
                                                                   o => (key == null ||
                                                                         key == "" ||
                                                                         o.TenNguoiLienHeVaiTro.Contains(key) ||
                                                                         o.SoDienThoai.Contains(key)))
                                                                         .Join(_repository.GetRepository<Quan>().GetAll(), b => b.QuanId, e => e.Id, (b, e) => new { Nha = b, Quan = e })
                                                                         .Join(_repository.GetRepository<Duong>().GetAll(), b => b.Nha.DuongId, g => g.Id, (b, g) => new { Nha = b, Duong = g })
                                                                         .Join(_repository.GetRepository<CapDoTheoDoi>().GetAll(), b => b.Nha.Nha.CapDoTheoDoiId, y => y.Id, (b, y) => new { Nha = b, CapDoTheoDoi = y }).ToList();

            return Json(new
            {
                draw = drawReturn,
                recordsTotal = paging.TotalRecord,
                recordsFiltered = paging.TotalRecord,
                data = articles.Select(o => new
                {
                    o.Nha.Nha.Nha.Id,
                    Quan = o.Nha.Nha.Quan.Name,
                    Duong = o.Nha.Duong.Name,
                    o.Nha.Nha.Nha.TenNguoiLienHeVaiTro,
                    o.Nha.Nha.Nha.SoDienThoai,
                    o.Nha.Nha.Nha.TongGiaThue,
                    CapDoTheoDoi = o.CapDoTheoDoi.Name,
                    TrangThai = o.Nha.Nha.Nha.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt"
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("cap-nhat-nha/{id?}", Name = "NhaUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Update(long id)
        {
            Nha nha = await _repository.GetRepository<Nha>().ReadAsync(id);

            SetViewBag();

            NhaUpdatingViewModel model = new NhaUpdatingViewModel()
            {
                Id = nha.Id,
                SoNha = nha.SoNha,
                TenToaNha = nha.TenToaNha,
                MatTienTreoBien = nha.MatTienTreoBien,
                BeNgangLotLong = nha.BeNgangLotLong,
                DienTichDat = nha.DienTichDat,
                DienTichDatSuDungTang1 = nha.DienTichDatSuDungTang1,
                SoTang = nha.SoTang,
                TongDienTichSuDung = nha.TongDienTichSuDung,
                DiChungChu = nha.DiChungChu,
                Ham = nha.Ham,
                ThangMay = nha.ThangMay,
                TongGiaThue = nha.TongGiaThue,
                GiaThueBQ = nha.GiaThueBQ,
                TenNguoiLienHeVaiTro = nha.TenNguoiLienHeVaiTro,
                SoDienThoai = nha.SoDienThoai,
                NgayCNHenLienHeLai = nha.NgayCNHenLienHeLai.HasValue ? nha.NgayCNHenLienHeLai.Value.ToString("dd/MM/yyyy") : "",
                GhiChu = nha.GhiChu,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("cap-nhat-nha")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Update(long id, NhaUpdatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                Nha nha = await _repository.GetRepository<Nha>().ReadAsync(id);

                nha.MatBangId = model.MatBangId;
                nha.QuanId = model.QuanId;
                nha.DuongId = model.DuongId;
                nha.SoNha = StringHelper.KillChars(model.SoNha);
                nha.TenToaNha = StringHelper.KillChars(model.TenToaNha);
                nha.MatTienTreoBien = model.MatTienTreoBien;
                nha.BeNgangLotLong = model.BeNgangLotLong;
                nha.DienTichDat = model.DienTichDat;
                nha.DienTichDatSuDungTang1 = model.DienTichDatSuDungTang1;
                nha.SoTang = model.SoTang;
                nha.TongDienTichSuDung = model.TongDienTichSuDung;
                nha.DiChungChu = model.DiChungChu;
                nha.Ham = model.Ham;
                nha.ThangMay = model.ThangMay;
                nha.NoiThatKhachThueCuId = model.NoiThatKhachThueCuId;
                nha.DanhGiaPhuHopVoiId = model.DanhGiaPhuHopVoiId;
                nha.TongGiaThue = model.TongGiaThue;
                nha.GiaThueBQ = model.GiaThueBQ;
                nha.TenNguoiLienHeVaiTro = model.TenNguoiLienHeVaiTro;
                nha.SoDienThoai = model.SoDienThoai;
                nha.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : Convert.ToDateTime(model.NgayCNHenLienHeLai);
                nha.CapDoTheoDoiId = model.CapDoTheoDoiId;
                nha.GhiChu = model.GhiChu;

                int result = await _repository.GetRepository<Nha>().UpdateAsync(nha, AccountId);

                if (result > 0)
                {
                    //TODO: HuyTQ - Ghi lịch sử

                    TempData["Success"] = "Cập nhật bài viết thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật bài viết không thành công! Vui lòng kiểm tra và thử lại!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
                return View(model);
            }
        }

        [HttpPost]
        [Route("xet-trang-thai-nha/{ids?}/{status?}", Name = "NhaSetNhaStatus")]
        public async Task<ActionResult> SetNhaStatus(string ids, byte status)
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
                        bool result = await SetNhaStatus(articleId, status);
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
        private async Task<bool> SetNhaStatus(long articleId, byte status)
        {
            var article = await _repository.GetRepository<Nha>().ReadAsync(articleId);
            if (article == null)
                return false;
            article.TrangThai = status;

            int result = await _repository.GetRepository<Nha>().UpdateAsync(article, AccountId);

            if (result > 0)
            {
                //TODO: HuyTQ - Lưu lịch sử thay đổi
                return true;
            }
            return false;
        }

        [HttpPost]
        [Route("xoa-nha/{ids?}", Name = "NhaDeleteNhas")]
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
                        bool result = await DeleteArticles(articleId);
                        if (result)
                            succeed++;
                    }
                return Json(new { success = true, message = string.Format(@"Đã xóa thành công {0} bảng ghi.", succeed) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không xóa được bài viết. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<bool> DeleteArticles(long articleId)
        {
            var article = await _repository.GetRepository<Nha>().ReadAsync(articleId);
            if (article == null)
                return false;

            int result = await _repository.GetRepository<Nha>().DeleteAsync(article, AccountId);

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
        [Route("xem-chi-tiet-nha-modal/{id?}", Name = "NhaDetailModal")]
        public async Task<ActionResult> DetailModal(long id)
        {
            var article = await _repository.GetRepository<Nha>().ReadAsync(id);

            //var account = await _repository.GetRepository<Account>().ReadAsync(article.NguoiTaoId);

            ViewBag.MatBang = (await _repository.GetRepository<MatBang>().ReadAsync(article.MatBangId)).Name;
            ViewBag.Quan = (await _repository.GetRepository<Quan>().ReadAsync(article.QuanId)).Name;
            ViewBag.Duong = (await _repository.GetRepository<Duong>().ReadAsync(article.DuongId)).Name;
            ViewBag.NoiThatKhachThueCu = (await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(article.NoiThatKhachThueCuId)).Name;
            ViewBag.NoiThatKhachThueCu = (await _repository.GetRepository<DanhGiaPhuHopVoi>().ReadAsync(article.DanhGiaPhuHopVoiId)).Name;
            ViewBag.CapDoTheoDoi = (await _repository.GetRepository<CapDoTheoDoi>().ReadAsync(article.CapDoTheoDoiId)).Name;

            //ViewBag.CreateBy = account.Name;
            //ViewBag.ArticleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
            return PartialView("_DetailModal", article);
        }
    }
}