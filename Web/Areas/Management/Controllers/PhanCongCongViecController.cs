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
    [RoutePrefix("phan-cong-cong-viec")]
    public class PhanCongCongViecController : BaseController
    {
        [Route("danh-sach-phan-cong-cong-viec", Name = "PhanCongCongViecIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.PhanCongCongViec)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("sua-phan-cong-cong-viec/{id?}", Name = "PhanCongCongViecUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.PhanCongCongViec)]
        public async Task<ActionResult> UpdatePhanCongCongViec(long id)
        {
            var article = await _repository.GetRepository<QuanLyCongViec>().ReadAsync(id);
            var quanlycongviecviewmodel = new QuanLyCongViecViewModel();
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
                    var matbang = await _repository.GetRepository<MatBang>().ReadAsync(nhucauthue.MatBangId);
                    if (matbang != null)
                    {
                        ViewBag.LoaiMatBang = matbang.Name;
                    }
                    ViewBag.MatTienTreoBien = nhucauthue.MatTienTreoBien + " (m)";
                    ViewBag.BeNgangLotLong = nhucauthue.BeNgangLotLong + " (m)";
                    ViewBag.DienTichDat = nhucauthue.DienTichDat + " (m2)";
                    ViewBag.DienTichDatSuDungTang1 = nhucauthue.DienTichDatSuDungTang1 + " (m2)";
                    ViewBag.TongDienTichSuDung = nhucauthue.TongDienTichSuDung + " (m2)";
                    ViewBag.SoTang = nhucauthue.SoTang;
                    ViewBag.DiChungChu = nhucauthue.DiChungChu? "Có" : "Không";
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
                    var matbang1 = await _repository.GetRepository<MatBang>().ReadAsync(nha.MatBangId);
                    if (matbang1 != null)
                    {
                        ViewBag.LoaiMatBang1 = matbang1.Name;
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

                quanlycongviecviewmodel.NoiDungCongViec = article.NoiDungCongViec;
                // load nhanvienphutrach
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
                quanlycongviecviewmodel.NhanVienPhuTrachId = article.NhanVienPhuTrachId;
               
            }
            return View("UpdatePhanCongCongViec", quanlycongviecviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("sua-phan-cong-cong-viec/{id?}")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.PhanCongCongViec)]
        public async Task<ActionResult> UpdatePhanCongCongViec(KhachThueCreatingViewModel model, long id, long nhucauId)
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

        [Route("danh-sach-phan-cong-json", Name = "GetPhanCongJson")]
        public ActionResult GetPhanCongJson(int status)
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

                long userID = 0;
                long nhaID = 0;
                long khachID = 0;
                if (!string.IsNullOrEmpty(key))
                {
                    var user = _repository.GetRepository<Account>().GetAll().FirstOrDefault(o => o.Name.Contains(key));
                    if (user != null)
                    {
                        userID = user.Id;
                    }
                    var khach = _repository.GetRepository<Khach>().GetAll().FirstOrDefault(o => o.TenNguoiLienHeVaiTro.Contains(key));
                    if (khach != null)
                    {
                        khachID = khach.Id;
                    }
                    var nha = _repository.GetRepository<Nha>().GetAll().FirstOrDefault(o => o.SoNha.Contains(key) || o.TenToaNha.Contains(key));
                    if (nha != null)
                    {
                        nhaID = nha.Id;
                    }
                }
                var articles = _repository.GetRepository<QuanLyCongViec>().GetAll(ref paging,
                                                                      orderKey,
                                                                      o => (key == null ||
                                                                            key == "" ||
                                                                             o.NhanVienPhuTrachId == userID ||
                                                                             o.NhaId == nhaID ||
                                                                             o.KhachId == khachID
                                                                             ) &&
                                                                            (o.TrangThai == status))
                                                                            .Join(_repository.GetRepository<Account>().GetAll(), b => b.NhanVienPhuTrachId, e => e.Id, (b, e) => new { QuanLyCongViec = b, Account = e })
                                                                            .Join(_repository.GetRepository<Khach>().GetAll(), b => b.QuanLyCongViec.KhachId, g => g.Id, (b, g) => new { QuanLyCongViec = b, Khach = g })
                                                                            .Join(_repository.GetRepository<Nha>().GetAll(), b => b.QuanLyCongViec.QuanLyCongViec.NhaId, y => y.Id, (b, y) => new { QuanLyCongViec = b, Nha = y }).ToList();
                return Json(new
                {
                    draw = drawReturn,
                    recordsTotal = paging.TotalRecord,
                    recordsFiltered = paging.TotalRecord,
                    data = articles.Select(o => new
                    {
                        o.Nha.TenToaNha,
                        o.QuanLyCongViec.QuanLyCongViec.QuanLyCongViec.Id,
                        TenNhanVienPhuTrach = o.QuanLyCongViec.QuanLyCongViec.Account.Name,
                        TenKhach = o.QuanLyCongViec.Khach.TenNguoiLienHeVaiTro == null ? "" : o.QuanLyCongViec.Khach.TenNguoiLienHeVaiTro,
                        TrangThai = o.QuanLyCongViec.QuanLyCongViec.QuanLyCongViec.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt"

                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("xet-trang-thai-phan-cong-cong-viec/{ids?}/{status?}", Name = "PhanCongCongViecSetPhanCongCongViecStatus")]
        public async Task<ActionResult> SetPhanCongCongViecStatus(string ids, byte status)
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
                        bool result = await SetPhanCongCongViecStatus(articleId, status);
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
        private async Task<bool> SetPhanCongCongViecStatus(long articleId, byte status)
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

        [HttpPost]
        [Route("xoa-phan-cong-cong-viec/{ids?}", Name = "PhanCongCongViecDeletePhanCongCongViecs")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.PhanCongCongViec)]
        public async Task<ActionResult> DeletePhanCongCongViecs(string ids)
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
                        bool result = await DeletePhanCongCongViecs(articleId);
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

        private async Task<bool> DeletePhanCongCongViecs(long articleId)
        {
            var result = 0;

            var article = await _repository.GetRepository<QuanLyCongViec>().ReadAsync(articleId);
            //var article1 = await _repository.ExecuteSqlCommandAsync<NhuCauThue>().ReadAsync(articleId);        
            result = await _repository.GetRepository<QuanLyCongViec>().DeleteAsync(article, AccountId);
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
        [Route("xem-chi-tiet-khach-thue-modal/{id?}/{nhucauId?}", Name = "KhachDetailModal1")]
        public async Task<ActionResult> DetailModal(long id, long nhucauId)
        {
            var article = await _repository.GetRepository<Khach>().ReadAsync(id);
            var nhucauthue = await _repository.GetRepository<NhuCauThue>().ReadAsync(nhucauId);
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
    }
}