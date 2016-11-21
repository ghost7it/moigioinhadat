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
    [RoutePrefix("khachthue")]
    public class KhachThueController : BaseController
    {
        [Route("danh-sach-khach-thue", Name = "KhachThueIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Khach)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("them-moi-nhu-cau/{id?}", Name = "NhuCauCreate")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> CreateNhuCau(long id)
        {

            SetViewBag();
            //return View();
            var article = await _repository.GetRepository<Khach>().ReadAsync(id);
            var khachthueviewmodel = new KhachThueCreatingViewModel();

            if (article != null)
            {
                //khachthueviewmodel.TenKhach = article.TenKhach;
                khachthueviewmodel.TenNguoiLienHeVaiTro = article.TenNguoiLienHeVaiTro;
                khachthueviewmodel.GhiChu = article.GhiChu;
                //khachthueviewmodel.LinhVuc = article.LinhVuc;
                //khachthueviewmodel.PhanKhuc = article.PhanKhuc;
                khachthueviewmodel.SoDienThoai = article.SoDienThoai.ToString();
                //khachthueviewmodel.SPChinh = article.SPChinh.ToString();
            }
            return PartialView("CreateNhuCau", khachthueviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("them-moi-nhu-cau/{id?}")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> CreateNhuCau(KhachThueCreatingViewModel model, long id)
        {
            if (ModelState.IsValid)
            {
                NhuCauThue nhucauthue = new NhuCauThue();
                nhucauthue.BeNgangLotLong = string.IsNullOrEmpty(model.BeNgangLotLong) ? 0 : float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                nhucauthue.DiChungChu = model.DiChungChu == "1" ? true : false;
                nhucauthue.DienTichDat = string.IsNullOrEmpty(model.DienTichDat) ? 0 : float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                nhucauthue.DienTichDatSuDungTang1 = string.IsNullOrEmpty(model.DienTichDatSuDungTang1) ? 0 : float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                nhucauthue.DuongId = Convert.ToInt64(model.DuongId);
                var item1 = await _repository.GetRepository<Duong>().ReadAsync(nhucauthue.DuongId);
                if (item1 != null) { nhucauthue.DuongName = item1.Name; }
                nhucauthue.GhiChu = StringHelper.KillChars(model.GhiChuNhuCau);
                nhucauthue.GiaThueBQ = string.IsNullOrEmpty(model.GiaThueBQ) ? 0 : Convert.ToDecimal(model.GiaThueBQ);
                nhucauthue.Ham = model.Ham == "1" ? true : false;
                nhucauthue.KhachId = id;
                nhucauthue.MatBangId = Convert.ToInt32(model.MatBangId);
                nhucauthue.MatTienTreoBien = string.IsNullOrEmpty(model.MatTienTreoBien) ? 0 : float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                nhucauthue.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : DateTime.ParseExact(model.NgayCNHenLienHeLai, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                nhucauthue.NguoiTaoId = AccountId;
                nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                nhucauthue.NoiThatKhachThueCuId = Convert.ToInt32(model.NoiThatKhachThueCuId);
                nhucauthue.QuanId = Convert.ToInt64(model.QuanId);
                var item = await _repository.GetRepository<Quan>().ReadAsync(nhucauthue.QuanId);
                if (item != null) { nhucauthue.QuanName = item.Name; }
                nhucauthue.SoNha = StringHelper.KillChars(model.SoNha);
                nhucauthue.SoTang = Convert.ToInt32(model.SoTang);
                nhucauthue.TenToaNha = StringHelper.KillChars(model.TenToaNha);
                nhucauthue.ThangMay = model.ThangMay == "1" ? true : false;
                nhucauthue.TongDienTichSuDung = string.IsNullOrEmpty(model.TongDienTichSuDung) ? 0 : float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                nhucauthue.TongGiaThue = string.IsNullOrEmpty(model.TongGiaThue) ? 0 : Convert.ToDecimal(model.TongGiaThue);
                nhucauthue.NgayTao = DateTime.Now;
                nhucauthue.TrangThai = 0; //Chờ duyệt
                nhucauthue.NguoiPhuTrachId = AccountId;

                int resultnhucauthue = 0;
                try
                {
                    resultnhucauthue = await _repository.GetRepository<NhuCauThue>().CreateAsync(nhucauthue, AccountId);
                }
                catch (Exception ex)
                {
                }
                if (resultnhucauthue > 0)
                {
                    TempData["Success"] = "Thêm mới dữ liệu nhu cầu thuê thành công!";
                }
                return RedirectToAction("Index");
            }
            else
            {
                //ViewBag.DangMatBang = await _repository.GetRepository<MatBang>().GetAllAsync();
                ModelState.AddModelError(string.Empty, "Thêm mới dữ liệu nhu cầu thuê không thành công! Vui lòng kiểm tra và thử lại!");
                return View(model);
            }
        }


        [Route("them-moi-khach-thue", Name = "KhachThueCreate")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> Create()
        {
            //ViewBag.CategoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
            SetViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("them-moi-khach-thue")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> Create(KhachThueCreatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                Khach khach = new Khach();

                khach.GhiChu = StringHelper.KillChars(model.GhiChu);
                //khach.LinhVuc = StringHelper.KillChars(model.LinhVuc);
                khach.NgayTao = DateTime.Now;
                khach.NguoiTaoId = AccountId;
                //khach.PhanKhuc = StringHelper.KillChars(model.PhanKhuc);
                khach.SoDienThoai = StringHelper.KillChars(model.SoDienThoai);
                //khach.SPChinh = StringHelper.KillChars(model.SPChinh);
                //khach.TenKhach = StringHelper.KillChars(model.TenKhach);
                khach.TenNguoiLienHeVaiTro = StringHelper.KillChars(model.TenNguoiLienHeVaiTro);
                khach.TrangThai = 0; //Chờ duyệt
                int result = 0;
                try
                {
                    result = await _repository.GetRepository<Khach>().CreateAsync(khach, AccountId);
                }
                catch { }
                if (result > 0)
                {
                    var khachNewerId = khach.Id;
                    NhuCauThue nhucauthue = new NhuCauThue();
                    nhucauthue.BeNgangLotLong = string.IsNullOrEmpty(model.BeNgangLotLong) ? 0 : float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                    nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                    nhucauthue.DiChungChu = model.DiChungChu == "1" ? true : false;
                    nhucauthue.DienTichDat = string.IsNullOrEmpty(model.DienTichDat) ? 0 : float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                    nhucauthue.DienTichDatSuDungTang1 = string.IsNullOrEmpty(model.DienTichDatSuDungTang1) ? 0 : float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                    nhucauthue.DuongId = Convert.ToInt64(model.DuongId);
                    var item1 = await _repository.GetRepository<Duong>().ReadAsync(nhucauthue.DuongId);
                    if (item1 != null) { nhucauthue.DuongName = item1.Name; }
                    nhucauthue.GhiChu = StringHelper.KillChars(model.GhiChuNhuCau);
                    nhucauthue.GiaThueBQ = string.IsNullOrEmpty(model.GiaThueBQ) ? 0 : Convert.ToDecimal(model.GiaThueBQ);
                    nhucauthue.Ham = model.Ham == "1" ? true : false;
                    nhucauthue.KhachId = khachNewerId;
                    nhucauthue.MatBangId = Convert.ToInt32(model.MatBangId);
                    nhucauthue.MatTienTreoBien = string.IsNullOrEmpty(model.MatTienTreoBien) ? 0 : float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                    nhucauthue.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : DateTime.ParseExact(model.NgayCNHenLienHeLai, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    nhucauthue.NguoiTaoId = AccountId;
                    nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                    nhucauthue.NoiThatKhachThueCuId = Convert.ToInt32(model.NoiThatKhachThueCuId);
                    nhucauthue.QuanId = Convert.ToInt64(model.QuanId);
                    var item = await _repository.GetRepository<Quan>().ReadAsync(nhucauthue.QuanId);
                    if (item != null) { nhucauthue.QuanName = item.Name; }
                    nhucauthue.SoNha = StringHelper.KillChars(model.SoNha);
                    nhucauthue.SoTang = Convert.ToInt32(model.SoTang);
                    nhucauthue.TenToaNha = StringHelper.KillChars(model.TenToaNha);
                    nhucauthue.ThangMay = model.ThangMay == "1" ? true : false;
                    nhucauthue.TongDienTichSuDung = string.IsNullOrEmpty(model.TongDienTichSuDung) ? 0 : float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                    nhucauthue.TongGiaThue = string.IsNullOrEmpty(model.TongGiaThue) ? 0 : Convert.ToDecimal(model.TongGiaThue);
                    nhucauthue.NgayTao = DateTime.Now;
                    nhucauthue.TrangThai = 0; //Chờ duyệt
                    int resultnhucauthue = 0;
                    try
                    {
                        resultnhucauthue = await _repository.GetRepository<NhuCauThue>().CreateAsync(nhucauthue, AccountId);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (resultnhucauthue > 0)
                    {
                        TempData["Success"] = "Nhập dữ liệu khách thuê mới thành công!";
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.DangMatBang = await _repository.GetRepository<MatBang>().GetAllAsync();
                ModelState.AddModelError(string.Empty, "Nhập dữ liệu khách thuê mới không thành công! Vui lòng kiểm tra và thử lại!");
                return View(model);
            }


        }

        [Route("sua-khach/{id?}/{nhucauId?}", Name = "KhachThueUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> UpdateKhach(long id, long nhucauId)
        {

            SetViewBag();
            //return View();
            var article = await _repository.GetRepository<Khach>().ReadAsync(id);
            var articleNhuCau = await _repository.GetRepository<NhuCauThue>().ReadAsync(nhucauId);
            var khachthueviewmodel = new KhachThueCreatingViewModel();

            if (article != null)
            {
                khachthueviewmodel.TenKhach = article.TenKhach;
                khachthueviewmodel.TenNguoiLienHeVaiTro = article.TenNguoiLienHeVaiTro;
                khachthueviewmodel.GhiChu = article.GhiChu;
                khachthueviewmodel.LinhVuc = article.LinhVuc;
                khachthueviewmodel.PhanKhuc = article.PhanKhuc;
                khachthueviewmodel.SoDienThoai = article.SoDienThoai.ToString();
                khachthueviewmodel.SPChinh = article.SPChinh.ToString();
            }
            if (articleNhuCau != null)
            {
                khachthueviewmodel.MatBangId = articleNhuCau.MatBangId.ToString();
                khachthueviewmodel.QuanId = articleNhuCau.QuanId.ToString();
                var duong = _repository.GetRepository<Duong>().GetAll().Where(o => o.QuanId == articleNhuCau.QuanId).OrderBy(o => o.Name).ToList();
                ViewBag.DuongDropdownlist = new SelectList(duong, "Id", "Name", khachthueviewmodel.DuongId);

                khachthueviewmodel.DuongId = articleNhuCau.DuongId.ToString();
                khachthueviewmodel.SoNha = articleNhuCau.SoNha;
                khachthueviewmodel.TenToaNha = articleNhuCau.TenToaNha;
                khachthueviewmodel.MatTienTreoBien = articleNhuCau.MatTienTreoBien.ToString();
                khachthueviewmodel.BeNgangLotLong = articleNhuCau.BeNgangLotLong.ToString();
                khachthueviewmodel.DienTichDat = articleNhuCau.DienTichDat.ToString();
                khachthueviewmodel.DienTichDatSuDungTang1 = articleNhuCau.DienTichDatSuDungTang1.ToString();
                khachthueviewmodel.CapDoTheoDoiId = articleNhuCau.CapDoTheoDoiId.ToString();
                khachthueviewmodel.SoTang = articleNhuCau.SoTang.ToString();
                khachthueviewmodel.TongDienTichSuDung = articleNhuCau.TongDienTichSuDung.ToString();
                khachthueviewmodel.DiChungChu = articleNhuCau.DiChungChu ? "1" : "0";
                khachthueviewmodel.Ham = articleNhuCau.Ham ? "1" : "0";
                khachthueviewmodel.ThangMay = articleNhuCau.ThangMay ? "1" : "0";
                khachthueviewmodel.NoiThatKhachThueCuId = articleNhuCau.NoiThatKhachThueCuId.ToString();
                khachthueviewmodel.TongGiaThue = articleNhuCau.TongGiaThue.ToString();
                khachthueviewmodel.GiaThueBQ = articleNhuCau.GiaThueBQ.ToString();
                khachthueviewmodel.NgayCNHenLienHeLai = string.Format("{0:dd/MM/yyyy}", articleNhuCau.NgayCNHenLienHeLai);
                khachthueviewmodel.GhiChuNhuCau = articleNhuCau.GhiChu;
            }
            return PartialView("UpdateKhach", khachthueviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("sua-khach/{id?}/{nhucauId?}")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> UpdateKhach(KhachThueCreatingViewModel model, long id, long nhucauId)
        {
            int result = 0;
            int result1 = 0;
            if (ModelState.IsValid)
            {
                var article = await _repository.GetRepository<Khach>().ReadAsync(id);
                if (article != null)
                {
                    article.TenKhach = model.TenKhach;
                    article.TenNguoiLienHeVaiTro = model.TenNguoiLienHeVaiTro;
                    article.GhiChu = model.GhiChu;
                    article.LinhVuc = model.LinhVuc;
                    article.PhanKhuc = model.PhanKhuc;
                    article.SoDienThoai = model.SoDienThoai;
                    article.SPChinh = model.SPChinh;
                    try
                    {
                        result = await _repository.GetRepository<Khach>().UpdateAsync(article, AccountId);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (result > 0)
                    {
                        var nhucauthue = await _repository.GetRepository<NhuCauThue>().ReadAsync(nhucauId);
                        if (nhucauthue != null)
                        {
                            nhucauthue.BeNgangLotLong = string.IsNullOrEmpty(model.BeNgangLotLong) ? 0 : float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                            nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                            nhucauthue.DiChungChu = model.DiChungChu == "1" ? true : false;
                            nhucauthue.DienTichDat = string.IsNullOrEmpty(model.DienTichDat) ? 0 : float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                            nhucauthue.DienTichDatSuDungTang1 = string.IsNullOrEmpty(model.DienTichDatSuDungTang1) ? 0 : float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                            nhucauthue.DuongId = Convert.ToInt64(model.DuongId);
                            var item1 = await _repository.GetRepository<Duong>().ReadAsync(nhucauthue.DuongId);
                            if (item1 != null) { nhucauthue.DuongName = item1.Name; }
                            nhucauthue.GhiChu = StringHelper.KillChars(model.GhiChuNhuCau);
                            nhucauthue.GiaThueBQ = string.IsNullOrEmpty(model.GiaThueBQ) ? 0 : Convert.ToDecimal(model.GiaThueBQ);
                            nhucauthue.Ham = model.Ham == "1" ? true : false;
                            nhucauthue.KhachId = id;
                            nhucauthue.MatBangId = Convert.ToInt32(model.MatBangId);
                            nhucauthue.MatTienTreoBien = string.IsNullOrEmpty(model.MatTienTreoBien) ? 0 : float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                            nhucauthue.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : DateTime.ParseExact(model.NgayCNHenLienHeLai, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            nhucauthue.NguoiTaoId = AccountId;
                            nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                            nhucauthue.NoiThatKhachThueCuId = Convert.ToInt32(model.NoiThatKhachThueCuId);
                            nhucauthue.QuanId = Convert.ToInt64(model.QuanId);
                            var item = await _repository.GetRepository<Quan>().ReadAsync(nhucauthue.QuanId);
                            if (item != null) { nhucauthue.QuanName = item.Name; }
                            nhucauthue.SoNha = StringHelper.KillChars(model.SoNha);
                            nhucauthue.SoTang = Convert.ToInt32(model.SoTang);
                            nhucauthue.TenToaNha = StringHelper.KillChars(model.TenToaNha);
                            nhucauthue.ThangMay = model.ThangMay == "1" ? true : false;
                            nhucauthue.TongDienTichSuDung = string.IsNullOrEmpty(model.TongDienTichSuDung) ? 0 : float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                            nhucauthue.TongGiaThue = string.IsNullOrEmpty(model.TongGiaThue) ? 0 : Convert.ToDecimal(model.TongGiaThue);
                            nhucauthue.NgayTao = DateTime.Now;
                            nhucauthue.TrangThai = 0; //Chờ duyệt
                            try
                            {
                                result1 = await _repository.GetRepository<NhuCauThue>().UpdateAsync(nhucauthue, AccountId);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                if (result1 > 0)
                {
                    TempData["Success"] = "Cập nhật dữ liệu khách thuê  thành công!";
                }
                return RedirectToAction("Index");
            }
            else
            {
                //ViewBag.DangMatBang = await _repository.GetRepository<MatBang>().GetAllAsync();
                ModelState.AddModelError(string.Empty, "Cập nhật dữ liệu khách thuê không thành công! Vui lòng kiểm tra và thử lại!");
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

        /* protected void SetViewBag()
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
         */

        [Route("danh-sach-khach-thue-json", Name = "GetKhachThueJson")]
        public ActionResult GetKhachThueJson()
        {
            string drawReturn = "1";

            byte status;

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
            //var articles = _repository.GetRepository<Khach>().GetAll(ref paging,
            //                                                       orderKey,
            //                                                       o => (key == null ||
            //                                                             key == "" ||
            //                                                             o.TenKhach.Contains(key) ||
            //                                                             o.SoDienThoai.Contains(key)))
            //                                                             .Join(_repository.GetRepository<NhuCauThue>().GetAll(), b => b.Id, e => e.KhachId, (b, e) => new { Khach = b, NhuCauThue = e == null ? null : e })
            //                                                             .Join(_repository.GetRepository<Quan>().GetAll(), g => g.NhuCauThue.QuanId, f => f.Id, (g, f) => new { NhuCauThue = g, Quan = f == null ? null : f })
            //                                                             .Join(_repository.GetRepository<Duong>().GetAll(), k => k.NhuCauThue.NhuCauThue.DuongId, y => y.Id, (k, y) => new { NhuCauThue = k, Duong = y == null ? null : y }).ToList();

            var articles = _repository.GetRepository<Khach>().GetAll(ref paging,
                                                                  orderKey,
                                                                  o => (key == null ||
                                                                        key == "" ||
                                                                        o.TenNguoiLienHeVaiTro.Contains(key) ||
                                                                        o.SoDienThoai.Contains(key)))
                                                                        .LeftJoin(                                           /// Source Collection
                                                                            _repository.GetRepository<NhuCauThue>().GetAll(),/// Inner Collection
                                                                            p => p.Id,                                       /// PK
                                                                            a => a.KhachId,                                  /// FK
                                                                            (p, a) => new { Khach = p, NhuCauThue = a }).ToList();

            //var nhucauthueList = _repository.GetRepository<NhuCauThue>().GetAll().ToList();
            //var quanList = _repository.GetRepository<Quan>().GetAll().ToList();
            //var duongList = _repository.GetRepository<Duong>().GetAll().ToList();


            //var articles1 = (from p in articles
            //                 join c in nhucauthueList on p.Id equals c.KhachId into j1
            //                 from j2 in j1.DefaultIfEmpty()
            //                 join d in quanList on j2.QuanId equals d.Id into j3
            //                 from j4 in j3.DefaultIfEmpty()
            //                 join e in duongList on j2.DuongId equals e.Id into j5
            //                 from j6 in j5.DefaultIfEmpty()
            //                 select new { NhuCauThueId = j2.Id, TenKhach = p.TenKhach, Id = p.Id, TongGiaThue = j2.TongGiaThue, Quan = j4.Name, Duong = j6.Name, SoDienThoai = p.SoDienThoai, TrangThai = j2.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt" }).ToList();

            //var articles3 = (from khach1 in articles2
            //                 from quan in quanList
            //                     .Where(s => s.Id == khach1.Nhucauthue.QuanId)
            //                     .DefaultIfEmpty()
            //                 select new { Quan = quan, Khach = khach1 }).ToList();

            //var articles4 = (from khach2 in articles3
            //                 from duong in duongList
            //                     .Where(s => s.Id == khach2.Khach.Nhucauthue.DuongId)
            //                     .DefaultIfEmpty()
            //                 select new { Duong = duong, Khach = khach2 }).ToList(); 


            return Json(new
            {
                draw = drawReturn,
                recordsTotal = paging.TotalRecord,
                recordsFiltered = paging.TotalRecord,
                data = articles.Select(o => new
                {
                    o.Khach.Id,
                    o.Khach.TenNguoiLienHeVaiTro,
                    o.Khach.SoDienThoai,
                    NhuCauThueId = o.NhuCauThue == null ? 0 : o.NhuCauThue.Id,
                    Quan = o.NhuCauThue == null ? "" : o.NhuCauThue.QuanName,
                    Duong = o.NhuCauThue == null ? "" : o.NhuCauThue.DuongName,
                    TongGiaThue = o.NhuCauThue == null ? "" : o.NhuCauThue.TongGiaThue.ToString(),
                    TrangThai = o.NhuCauThue == null ? "" : (o.NhuCauThue.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt")
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("xet-trang-thai-khach-thue/{ids?}/{status?}", Name = "KhachThueSetKhachThueStatus")]
        public async Task<ActionResult> SetKhachThueStatus(string ids, byte status)
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
                        bool result = await SetKhachThueStatus(articleId, status);
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
        private async Task<bool> SetKhachThueStatus(long articleId, byte status)
        {
            var article = await _repository.GetRepository<NhuCauThue>().ReadAsync(articleId);
            if (article == null)
                return false;
            article.TrangThai = status;

            int result = await _repository.GetRepository<NhuCauThue>().UpdateAsync(article, AccountId);

            if (result > 0)
            {
                //TODO: HuyTQ - Lưu lịch sử thay đổi
                return true;
            }
            return false;
        }

        [HttpPost]
        [Route("xoa-khach-thue/{ids?}", Name = "KhachThueDeleteKhachThues")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Khach)]
        public async Task<ActionResult> DeleteKhachThues(string ids, bool isKhach)
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
                        bool result = await DeleteArticles(articleId, isKhach);
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

        private async Task<bool> DeleteArticles(long articleId, bool isKhach)
        {
            var result = 0;
            var result1 = 0;
            if (isKhach)
            {
                var article = await _repository.GetRepository<Khach>().ReadAsync(articleId);
                //var article1 = await _repository.ExecuteSqlCommandAsync<NhuCauThue>().ReadAsync(articleId);
                var article1 = _repository.GetRepository<NhuCauThue>().GetAll().Where(o => o.KhachId == articleId).Select(o => o.Id).ToList();

                if (article1.Any())
                {
                    foreach (var item in article1)
                    {
                        result1 = await _repository.GetRepository<NhuCauThue>().DeleteAsync(item, AccountId);
                    }
                }
                if (article == null)
                    return false;

                result = await _repository.GetRepository<Khach>().DeleteAsync(article, AccountId);
            }
            else
            {
                var article = await _repository.GetRepository<NhuCauThue>().ReadAsync(articleId);
                if (article == null)
                    return false;
                result = await _repository.GetRepository<NhuCauThue>().DeleteAsync(article, AccountId);
            }
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
        [Route("xem-chi-tiet-khach-thue-modal/{id?}/{nhucauId?}", Name = "KhachDetailModal")]
        public async Task<ActionResult> DetailModal(long id, long nhucauId)
        {
            var article = await _repository.GetRepository<Khach>().ReadAsync(id);
            var nhucauthue = await _repository.GetRepository<NhuCauThue>().ReadAsync(nhucauId);
            var khachthueviewmodel = new KhachThueCreatingViewModel();

            if (article != null)
            {
                //khachthueviewmodel.TenKhach = article.TenKhach;
                khachthueviewmodel.TenNguoiLienHeVaiTro = article.TenNguoiLienHeVaiTro;
                khachthueviewmodel.GhiChu = article.GhiChu;
                //khachthueviewmodel.LinhVuc = article.LinhVuc;
                //khachthueviewmodel.PhanKhuc = article.PhanKhuc;
                khachthueviewmodel.SoDienThoai = article.SoDienThoai.ToString();
                //khachthueviewmodel.SPChinh = article.SPChinh.ToString();
            }
            if (nhucauthue != null)
            {
                khachthueviewmodel.MatBangId = (await _repository.GetRepository<MatBang>().ReadAsync(nhucauthue.MatBangId)).Name;
                khachthueviewmodel.BeNgangLotLong = nhucauthue.BeNgangLotLong.ToString();
                khachthueviewmodel.CapDoTheoDoiId = (await _repository.GetRepository<CapDoTheoDoi>().ReadAsync(nhucauthue.CapDoTheoDoiId)).Name;
                khachthueviewmodel.DiChungChu = nhucauthue.DiChungChu == true ? "Có" : "Không";
                khachthueviewmodel.DienTichDat = nhucauthue.DienTichDat.ToString();
                khachthueviewmodel.DienTichDatSuDungTang1 = nhucauthue.DienTichDatSuDungTang1.ToString();
                khachthueviewmodel.DuongName = nhucauthue.DuongName;
                khachthueviewmodel.GhiChuNhuCau = nhucauthue.GhiChu;
                khachthueviewmodel.GiaThueBQ = nhucauthue.GiaThueBQ.ToString();
                khachthueviewmodel.Ham = nhucauthue.Ham == true ? "Có" : "Không";
                khachthueviewmodel.MatTienTreoBien = nhucauthue.MatTienTreoBien.ToString();
                khachthueviewmodel.NgayCNHenLienHeLai = string.Format("{0:dd/MM/yyyy}", nhucauthue.NgayCNHenLienHeLai);
                khachthueviewmodel.NoiThatKhachThueCuId = (await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(nhucauthue.NoiThatKhachThueCuId)).Name;
                khachthueviewmodel.QuanName = nhucauthue.QuanName;
                khachthueviewmodel.SoNha = nhucauthue.SoNha.ToString();
                khachthueviewmodel.SoTang = nhucauthue.SoTang.ToString();
                khachthueviewmodel.TenToaNha = nhucauthue.TenToaNha;
                khachthueviewmodel.ThangMay = nhucauthue.ThangMay == true ? "Có" : "Không";
                khachthueviewmodel.TongDienTichSuDung = nhucauthue.TongDienTichSuDung.ToString();
                khachthueviewmodel.TongGiaThue = nhucauthue.TongGiaThue.ToString();
                khachthueviewmodel.TrangThaiNhuCau = nhucauthue.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt";
            }
            return PartialView("_DetailModal", khachthueviewmodel);
        }

        public string GetPhanCong(long khachId, long nhaId, long nhucauthueId)
        {
            var phancong = _repository.GetRepository<QuanLyCongViec>().GetAll().SingleOrDefault(o => o.NhuCauThueId == nhucauthueId && o.KhachId == khachId && o.NhaId == nhaId);
            if (phancong != null)
                return "hidden";
            return "block";
        }

        public string GetNhanVienPhuTrach(long khachId, long nhaId, long nhucauthueId)
        {
            var phancong = _repository.GetRepository<QuanLyCongViec>().GetAll().SingleOrDefault(o => o.NhuCauThueId == nhucauthueId && o.KhachId == khachId && o.NhaId == nhaId);
            if (phancong != null)
            {
                var nhanvien = _repository.GetRepository<Account>().GetAll().SingleOrDefault(o => o.Id == phancong.NhanVienPhuTrachId);
                if (nhanvien != null) return nhanvien.Name;
            }
            return "";
        }

        [Route("tim-nha-cho-khach/{id?}/{nhucauId?}", Name = "TimNha")]
        public async Task<ActionResult> TimNha(long id, long nhucauId)
        {
            try
            {
                var nhanvienchamsocname = "";
                var khachthue = await _repository.GetRepository<Khach>().ReadAsync(id);
                var nhucauthue = await _repository.GetRepository<NhuCauThue>().ReadAsync(nhucauId);

                //if (phancong != null)
                //{
                //    var nhanvien = _repository.GetRepository<Account>().GetAll().SingleOrDefault(o => o.Id == phancong.NhanVienPhuTrachId);
                //    if (nhanvien != null) nhanvienchamsocname = nhanvien.Name;
                //}

                var result = _repository.GetRepository<Nha>().GetAll().Where(o => o.MatBangId.Equals(nhucauthue.MatBangId) || o.QuanId.Equals(nhucauthue.QuanId) || o.DuongId.Equals(nhucauthue.DuongId)).ToList();
                //.Join(_repository.GetRepository<NhuCauThue>().GetAll(), b => b., c => c.Id, (b, c) => new { NhuCauThue = b, Nha = c }).ToList();

                var data = result.Select(o => new NhaUpdatingViewModel
                {
                    Id = o.Id,
                    TenToaNha = o.TenToaNha,
                    SoNha = o.SoNha,
                    SoDienThoai = o.SoDienThoai,
                    KhachId = khachthue == null ? 0 : khachthue.Id,
                    NhuCauThueId = nhucauthue == null ? 0 : nhucauthue.Id,
                    TenNguoiLienHeVaiTro = o.TenNguoiLienHeVaiTro,
                    DienTichDat = o.DienTichDat == null ? 0 : o.DienTichDat,
                    TongGiaThue = o.TongGiaThue == null ? 0 : o.TongGiaThue,
                    DaPhanCong = GetPhanCong(khachthue == null ? 0 : khachthue.Id, o.Id, nhucauthue == null ? 0 : nhucauthue.Id),
                    NhanVienPhuTrachName = GetNhanVienPhuTrach(khachthue == null ? 0 : khachthue.Id, o.Id, nhucauthue == null ? 0 : nhucauthue.Id)
                });
                //aaa
                //Phân công nhân viên chăm sóc
                int NhanVienChamSocRoleGroupId = Convert.ToInt32(WebConfigurationManager.AppSettings["NhanVienChamSocRoleGroupId"]);

                var account = _repository.GetRepository<AccountRole>().GetAll().Where(o => o.RoleId == NhanVienChamSocRoleGroupId)
                  .Join(_repository.GetRepository<Account>().GetAll(), b => b.AccountId, c => c.Id, (b, c) => new { AccountRole = b, Account = c }).ToList();

                var obj = new List<object>();

                foreach (var item in account)
                {
                    obj.Add(new { ID = item.Account.Id, Name = item.Account.Name });
                }

                var listAccount = new SelectList(obj, "ID", "Name", 1);

                ViewBag.Accounts = listAccount;

                return PartialView("TimNhaModal", data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //aa

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //[ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Nha)]
        [Route("them-moi-phan-cong/{nhaId?}/{khachId?}/{nhucauthueId?}/{phutrachId?}", Name = "PhanCongCreate")]
        public async Task<ActionResult> CreatePhanCong(NhaUpdatingViewModel model, long nhaId, long khachId, long nhucauthueId, long phutrachId)
        {
            //if (ModelState.IsValid)
            //{
            QuanLyCongViec item = new QuanLyCongViec();
            item.NhanVienPhuTrachId = phutrachId;
            item.KhachId = khachId;
            item.NgayTao = DateTime.Now;
            item.NguoiTaoId = AccountId;
            item.NhaId = nhaId;
            item.NhuCauThueId = nhucauthueId;
            item.TrangThai = 0; //Chờ gửi thông tin cho người phụ trách
            int resultphancong = 0;
            try
            {
                resultphancong = await _repository.GetRepository<QuanLyCongViec>().CreateAsync(item, AccountId);
            }
            catch (Exception ex)
            {
            }
            if (resultphancong > 0)
            {
                TempData["Success"] = "Phân công công việc thành công!";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Phân công công việc không thành công! Vui lòng kiểm tra và thử lại!");
                return View();
            }
            return RedirectToAction("Index");
        }

    }
}