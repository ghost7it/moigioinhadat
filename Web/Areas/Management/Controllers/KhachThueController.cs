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
    [RoutePrefix("quan-ly-khach-thue")]
    public class KhachThueController : BaseController
    {
        [Route("danh-sach-khach-thue", Name = "KhachThueIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Khach)]
        public ActionResult Index()
        {
            var quanUpdate = _repository.GetRepository<Quan>().GetAll();
            ViewBag.QuanDropdownlist = quanUpdate.ToList().ToSelectList();

            var duong = _repository.GetRepository<Duong>().GetAll();
            ViewBag.DuongDropdownlist = duong.ToList().ToSelectList();

            var matBang = _repository.GetRepository<MatBang>().GetAll();
            ViewBag.MatBangDropdownlist = matBang.ToList().ToSelectList();

            ViewBag.HidenClass = RoleHelper.CheckPermission(ModuleEnum.PhanCongCongViec, ActionEnum.Read) ? "" : "hidden";

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

                //Mặt bằng
                var matBang = _repository.GetRepository<MatBang>().GetAll();
                var listMatBangArr = new List<MatBangItem>();
                if (matBang.Any())
                {
                    foreach (var item in matBang)
                    {
                        listMatBangArr.Add(new MatBangItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                    }
                }
                khachthueviewmodel.ListMatBangArr = listMatBangArr;

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
                if (!string.IsNullOrEmpty(model.BeNgangLotLong))
                {
                    nhucauthue.BeNgangLotLong = float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                }

                nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                nhucauthue.DiChungChu = model.DiChungChu == "1" ? true : false;
                if (!string.IsNullOrEmpty(model.DienTichDat))
                {
                    nhucauthue.DienTichDat = float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                }
                if (!string.IsNullOrEmpty(model.DienTichDatSuDungTang1))
                {
                    nhucauthue.DienTichDatSuDungTang1 = float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                }

                nhucauthue.DuongId = Convert.ToInt64(model.DuongId);
                var item1 = await _repository.GetRepository<Duong>().ReadAsync(nhucauthue.DuongId);
                if (item1 != null) { nhucauthue.DuongName = item1.Name; }
                nhucauthue.GhiChu = StringHelper.KillChars(model.GhiChuNhuCau);
                if (!string.IsNullOrEmpty(model.GiaThueBQ))
                {
                    nhucauthue.GiaThueBQ = Convert.ToDecimal(model.GiaThueBQ);
                }

                nhucauthue.Ham = model.Ham == "1" ? true : false;
                nhucauthue.KhachId = id;
                nhucauthue.MatBangId = model.MatBangId;

                if (!string.IsNullOrEmpty(model.MatTienTreoBien))
                {
                    nhucauthue.MatTienTreoBien = float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                }

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

                if (!string.IsNullOrEmpty(model.TongDienTichSuDung))
                {
                    nhucauthue.TongDienTichSuDung = float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                }
                if (!string.IsNullOrEmpty(model.TongGiaThue))
                {
                    nhucauthue.TongGiaThue = Convert.ToDecimal(model.TongGiaThue);
                }

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
            //Mặt bằng
            var matBang = _repository.GetRepository<MatBang>().GetAll();
            KhachThueCreatingViewModel model = new KhachThueCreatingViewModel();
            var listMatBangArr = new List<MatBangItem>();
            if (matBang.Any())
            {
                foreach (var item in matBang)
                {
                    listMatBangArr.Add(new MatBangItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                }
            }
            model.ListMatBangArr = listMatBangArr;

            return View(model);
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
                khach.NguoiPhuTrachId = AccountId;
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
                    if (!string.IsNullOrEmpty(model.QuanDuongArr))
                    {
                        string[] arrQuanDuong = model.QuanDuongArr.Split(';');
                        if (arrQuanDuong.Count() > 0)
                        {
                            for (var i = 0; i < arrQuanDuong.Count(); i++)
                            {
                                string[] arrDetail = arrQuanDuong[i].Split(',');
                                if (arrDetail.Count() > 0)
                                {
                                    NhuCauThue nhucauthue = new NhuCauThue();
                                    if (!string.IsNullOrEmpty(model.BeNgangLotLong))
                                    {
                                        nhucauthue.BeNgangLotLong = float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                                    }
                                    nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                                    nhucauthue.DiChungChu = model.DiChungChu == "1" ? true : false;
                                    if (!string.IsNullOrEmpty(model.DienTichDat))
                                    {
                                        nhucauthue.DienTichDat = float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                                    }
                                    if (!string.IsNullOrEmpty(model.DienTichDatSuDungTang1))
                                    {
                                        nhucauthue.DienTichDatSuDungTang1 = float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                                    }

                                    nhucauthue.DuongId = Convert.ToInt64(arrDetail[1]);
                                    var item1 = await _repository.GetRepository<Duong>().ReadAsync(nhucauthue.DuongId);
                                    if (item1 != null) { nhucauthue.DuongName = item1.Name; }
                                    nhucauthue.GhiChu = StringHelper.KillChars(model.GhiChuNhuCau);
                                    if (!string.IsNullOrEmpty(model.GiaThueBQ))
                                    {
                                        nhucauthue.GiaThueBQ = Convert.ToDecimal(model.GiaThueBQ);
                                    }

                                    nhucauthue.Ham = model.Ham == "1" ? true : false;
                                    nhucauthue.KhachId = khachNewerId;
                                    nhucauthue.MatBangId = model.MatBangId;
                                    if (!string.IsNullOrEmpty(model.MatTienTreoBien))
                                    {
                                        nhucauthue.MatTienTreoBien = float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                                    }

                                    nhucauthue.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : DateTime.ParseExact(model.NgayCNHenLienHeLai, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    nhucauthue.NguoiTaoId = AccountId;
                                    nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                                    nhucauthue.NoiThatKhachThueCuId = Convert.ToInt32(model.NoiThatKhachThueCuId);
                                    nhucauthue.QuanId = Convert.ToInt64(arrDetail[0]);
                                    var item = await _repository.GetRepository<Quan>().ReadAsync(nhucauthue.QuanId);
                                    if (item != null) { nhucauthue.QuanName = item.Name; }
                                    nhucauthue.SoNha = StringHelper.KillChars(model.SoNha);
                                    nhucauthue.SoTang = Convert.ToInt32(model.SoTang);
                                    nhucauthue.TenToaNha = StringHelper.KillChars(model.TenToaNha);
                                    nhucauthue.ThangMay = model.ThangMay == "1" ? true : false;
                                    if (!string.IsNullOrEmpty(model.TongDienTichSuDung))
                                    {
                                        nhucauthue.TongDienTichSuDung = float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                                    }
                                    if (!string.IsNullOrEmpty(model.TongGiaThue))
                                    {
                                        nhucauthue.TongGiaThue = Convert.ToDecimal(model.TongGiaThue);
                                    }

                                    nhucauthue.NgayTao = DateTime.Now;
                                    nhucauthue.TrangThai = 0; //Chờ duyệt
                                    int resultnhucauthue = 0;
                                    var listMatBangArr = model.ListMatBangArr;
                                    if (listMatBangArr.Any())
                                    {
                                        var matbangidarr = "";
                                        foreach (MatBangItem mb in listMatBangArr)
                                        {
                                            if (mb.IsSelected)
                                            {
                                                matbangidarr += mb.FieldKey;
                                                if (mb != listMatBangArr[listMatBangArr.Count - 1])
                                                {
                                                    matbangidarr += ",";
                                                }
                                            }
                                        }
                                        nhucauthue.MatBangId = matbangidarr;
                                    }

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
                            }
                        }

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

            var matBang = _repository.GetRepository<MatBang>().GetAll();
            var listMatBangArr = new List<MatBangItem>();
            if (matBang.Any())
            {
                foreach (var item in matBang)
                {
                    listMatBangArr.Add(new MatBangItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                }
            }

            if (article != null)
            {
                khachthueviewmodel.TenKhach = article.TenKhach;
                khachthueviewmodel.TenNguoiLienHeVaiTro = article.TenNguoiLienHeVaiTro;
                khachthueviewmodel.GhiChu = article.GhiChu;
                //khachthueviewmodel.LinhVuc = article.LinhVuc;
                //khachthueviewmodel.PhanKhuc = article.PhanKhuc;
                khachthueviewmodel.SoDienThoai = article.SoDienThoai.ToString();
                //khachthueviewmodel.SPChinh = article.SPChinh.ToString();
            }
            if (articleNhuCau != null)
            {
                if (!string.IsNullOrEmpty(articleNhuCau.MatBangId))
                {
                    string[] arrmatbangid = articleNhuCau.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrmatbangid.Count() > 0)
                    {
                        for (var i = 0; i < arrmatbangid.Count(); i++)
                        {
                            foreach (var item in listMatBangArr.Where(w => w.FieldKey == Convert.ToInt64(arrmatbangid[i])))
                            {
                                item.IsSelected = true;
                            }
                        }
                    }
                }
                khachthueviewmodel.ListMatBangArr = listMatBangArr;
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
                    //article.LinhVuc = model.LinhVuc;
                    //article.PhanKhuc = model.PhanKhuc;
                    article.SoDienThoai = model.SoDienThoai;
                    //article.SPChinh = model.SPChinh;
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
                            if (!string.IsNullOrEmpty(model.BeNgangLotLong))
                            {
                                nhucauthue.BeNgangLotLong = float.Parse(model.BeNgangLotLong, CultureInfo.InvariantCulture.NumberFormat);
                            }

                            nhucauthue.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                            nhucauthue.DiChungChu = model.DiChungChu == "1" ? true : false;
                            if (!string.IsNullOrEmpty(model.DienTichDat))
                            {
                                nhucauthue.DienTichDat = float.Parse(model.DienTichDat, CultureInfo.InvariantCulture.NumberFormat);
                            }
                            if (!string.IsNullOrEmpty(model.DienTichDatSuDungTang1))
                            {
                                nhucauthue.DienTichDatSuDungTang1 = float.Parse(model.DienTichDatSuDungTang1, CultureInfo.InvariantCulture.NumberFormat);
                            }

                            nhucauthue.DuongId = Convert.ToInt64(model.DuongId);
                            var item1 = await _repository.GetRepository<Duong>().ReadAsync(nhucauthue.DuongId);
                            if (item1 != null) { nhucauthue.DuongName = item1.Name; }
                            nhucauthue.GhiChu = StringHelper.KillChars(model.GhiChuNhuCau);
                            if (!string.IsNullOrEmpty(model.GiaThueBQ))
                            {
                                nhucauthue.GiaThueBQ = Convert.ToDecimal(model.GiaThueBQ);
                            }

                            nhucauthue.Ham = model.Ham == "1" ? true : false;
                            nhucauthue.KhachId = id;
                            nhucauthue.MatBangId = model.MatBangId;
                            if (!string.IsNullOrEmpty(model.MatTienTreoBien))
                            {
                                nhucauthue.MatTienTreoBien = float.Parse(model.MatTienTreoBien, CultureInfo.InvariantCulture.NumberFormat);
                            }

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
                            if (!string.IsNullOrEmpty(model.TongDienTichSuDung))
                            {
                                nhucauthue.TongDienTichSuDung = float.Parse(model.TongDienTichSuDung, CultureInfo.InvariantCulture.NumberFormat);
                            }
                            if (!string.IsNullOrEmpty(model.TongGiaThue))
                            {
                                nhucauthue.TongGiaThue = Convert.ToDecimal(model.TongGiaThue);
                            }
                            //nhucauthue.NgayTao = DateTime.Now;
                            //nhucauthue.TrangThai = 0; //Chờ duyệt
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
            //var matBang = _repository.GetRepository<MatBang>().GetAll();
            //ViewBag.MatBang = matBang.ToList().ToSelectList();

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
        public ActionResult GetKhachThueJson(int status)
        {
            string drawReturn = "1";

            //int status = 0;

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

            string ghiChu = "";
            string objectGhiChu = Request.Params["objectGhiChu"];
            if (!string.IsNullOrEmpty(objectGhiChu))
                ghiChu = objectGhiChu.ToString();

            string tenKhach = "";
            string objectTenKhach = Request.Params["objectTenKhach"];
            if (!string.IsNullOrEmpty(objectTenKhach))
                tenKhach = objectTenKhach.ToString();

            long quanId = 0;
            string objectQuan = Request.Params["objectQuan"];
            if (!string.IsNullOrEmpty(objectQuan))
                long.TryParse(objectQuan.ToString(), out quanId);

            long duongId = 0;
            string objectDuong = Request.Params["objectDuong"];
            if (!string.IsNullOrEmpty(objectDuong))
                long.TryParse(objectDuong.ToString(), out duongId);

            //string matTienId = "";
            //string objectMatTien = Request.Params["objectMatTien"];
            //if (!string.IsNullOrEmpty(objectMatTien))
            //    matTienId = objectMatTien.ToString();

            //Mặt tièn
            float matTienTu = 0;
            string objectMatTienTu = Request.Params["objectMatTienTu"];
            if (!string.IsNullOrEmpty(objectMatTienTu))
                float.TryParse(objectMatTienTu.ToString(), out matTienTu);

            float matTienDen = 0;
            string objectMatTienDen = Request.Params["objectMatTienDen"];
            if (!string.IsNullOrEmpty(objectMatTienDen))
                float.TryParse(objectMatTienDen.ToString(), out matTienDen);

            if (matTienDen == 0)
            {
                matTienDen = float.MaxValue;
            }

            decimal giaThueTu = 0;
            string objectGiaThueTu = Request.Params["objectGiaThueTu"];
            if (!string.IsNullOrEmpty(objectGiaThueTu))
                decimal.TryParse(objectGiaThueTu.ToString(), out giaThueTu);

            decimal giaThueDen = 0;
            string objectGiaThueDen = Request.Params["objectGiaThueDen"];
            if (!string.IsNullOrEmpty(objectGiaThueDen))
                decimal.TryParse(objectGiaThueDen.ToString(), out giaThueDen);

            if (giaThueDen == 0)
            {
                giaThueDen = decimal.MaxValue;
            }

            float dtsdt1Tu = 0;
            string objectDTSDT1Tu = Request.Params["objectDTSDT1Tu"];
            if (!string.IsNullOrEmpty(objectDTSDT1Tu))
                float.TryParse(objectDTSDT1Tu.ToString(), out dtsdt1Tu);

            float dtsdt1Den = 0;
            string objectDTSDT1Den = Request.Params["objectDTSDT1Den"];
            if (!string.IsNullOrEmpty(objectDTSDT1Den))
                float.TryParse(objectDTSDT1Den.ToString(), out dtsdt1Den);

            if (dtsdt1Den == 0)
            {
                dtsdt1Den = float.MaxValue;
            }

            float tongDTSDTu = 0;
            string objectTongDTSDTu = Request.Params["objectTongDTSDTu"];
            if (!string.IsNullOrEmpty(objectTongDTSDTu))
                float.TryParse(objectTongDTSDTu.ToString(), out tongDTSDTu);

            float tongDTSDDen = 0;
            string objectTongDTSDDen = Request.Params["objectTongDTSDDen"];
            if (!string.IsNullOrEmpty(objectTongDTSDDen))
                float.TryParse(objectTongDTSDDen.ToString(), out tongDTSDDen);

            if (tongDTSDDen == 0)
            {
                tongDTSDDen = float.MaxValue;
            }

            Paging paging = new Paging()
            {
                TotalRecord = 0,
                Skip = skip,
                Take = take,
                OrderDirection = orderDir
            };

            bool isAdmin = AccountRoles.Any(t => t.RoleId == 1);

            var articles = _repository.GetRepository<Khach>().GetAll(ref paging,
                                                                  orderKey,
                                                                  o => (key == null ||
                                                                        key == "") && isAdmin ? 1 == 1 : o.NguoiPhuTrachId == AccountId)
                                                                        .LeftJoin(                                           /// Source Collection
                                                                            _repository.GetRepository<NhuCauThue>().GetAll(),/// Inner Collection
                                                                            p => p.Id,                                       /// PK
                                                                            a => a.KhachId,                                  /// FK
                                                                            (p, a) => new { Khach = p, NhuCauThue = a }).ToList();

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
                    TrangThai = o.NhuCauThue == null ? "" : (o.NhuCauThue.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt"),
                    TrangThaiId = o.NhuCauThue.TrangThai,
                    o.NhuCauThue.GhiChu,
                    o.NhuCauThue.QuanId,
                    o.NhuCauThue.DuongId,
                    o.NhuCauThue.MatBangId,
                    o.NhuCauThue.MatTienTreoBien,
                    o.NhuCauThue.TongGiaThue,
                    o.NhuCauThue.DienTichDatSuDungTang1,
                    o.NhuCauThue.TongDienTichSuDung
                })
                .Where(t => (t.GhiChu.Contains(ghiChu) || ghiChu == "") &&
                       (t.QuanId == quanId || quanId == 0) &&
                       (t.DuongId == duongId || duongId == 0) &&
                       (t.TrangThaiId == status) &&
                        (matTienTu <= t.MatTienTreoBien && t.MatTienTreoBien <= matTienDen) &&
                       (t.TenNguoiLienHeVaiTro.Contains(tenKhach) || tenKhach == "") &&
                       (giaThueTu <= t.TongGiaThue && t.TongGiaThue <= giaThueDen) &&
                       (dtsdt1Tu <= t.DienTichDatSuDungTang1 && t.DienTichDatSuDungTang1 <= dtsdt1Den) &&
                       (tongDTSDTu <= t.TongDienTichSuDung && t.TongDienTichSuDung <= tongDTSDDen))
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
                string[] matbangarr = nhucauthue.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (matbangarr.Count() > 0)
                {
                    for (var i = 0; i < matbangarr.Count(); i++)
                    {
                        khachthueviewmodel.MatBangId += "- " + (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i]))).Name;
                        if (matbangarr[i] != matbangarr[matbangarr.Count() - 1])
                        {
                            khachthueviewmodel.MatBangId += @"</br>";
                        }
                    }
                }
                //khachthueviewmodel.MatBangId = (await _repository.GetRepository<MatBang>().ReadAsync(nhucauthue.MatBangId)).Name;
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

                var result = _repository.GetRepository<Nha>().GetAll()
                                        .Where(o => o.MatTienTreoBien >= (nhucauthue.MatTienTreoBien * 0.75) &&
                                               o.DienTichDatSuDungTang1 >= (nhucauthue.DienTichDatSuDungTang1 * 0.75) &&
                                               o.TongDienTichSuDung >= (nhucauthue.TongDienTichSuDung * 0.75) &&
                                               o.QuanId.Equals(nhucauthue.QuanId) &&
                                               o.DuongId.Equals(nhucauthue.DuongId)).ToList();

                var data = result.Select(o => new NhaUpdatingViewModel
                {
                    Id = o.Id,
                    TenToaNha = o.TenToaNha,
                    SoNha = o.SoNha,
                    SoDienThoai = o.SoDienThoai,
                    KhachId = khachthue == null ? "0" : khachthue.Id.ToString(),
                    NhuCauThueId = nhucauthue == null ? "" : nhucauthue.Id.ToString(),
                    TenNguoiLienHeVaiTro = o.TenNguoiLienHeVaiTro,
                    DienTichDat = o.DienTichDat == null ? "0" : o.DienTichDat.ToString(),
                    TongGiaThue = o.TongGiaThue == null ? "0" : o.TongGiaThue.ToString(),
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
            item.NhaHiddenField = "SoNha,SoDienThoai,NguoiLienHe";
            item.NgayHoanThanh = DateTime.Now.AddDays(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["HanHoanThanh"]));

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