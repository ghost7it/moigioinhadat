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
    [RoutePrefix("quan-ly-nha")]
    public class NhaController : BaseController
    {
        NhaUpdatingViewModel _modelBK = new NhaUpdatingViewModel();

        [Route("danh-sach-nha", Name = "NhaIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Nha)]
        public ActionResult Index()
        {
            var quanUpdate = _repository.GetRepository<Quan>().GetAll();
            ViewBag.QuanDropdownlist = quanUpdate.ToList().ToSelectList();

            var duong = _repository.GetRepository<Duong>().GetAll();
            ViewBag.DuongDropdownlist = duong.ToList().ToSelectList();

            //var matBang = _repository.GetRepository<MatBang>().GetAll();
            //ViewBag.MatBangDropdownlist = matBang.ToList().ToSelectList();

            ViewBag.HidenClass = RoleHelper.CheckPermission(ModuleEnum.PhanCongCongViec, ActionEnum.Read) ? "" : "hidden";
            return View();
        }

        [Route("them-moi-nha", Name = "NhaCreate")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Create()
        {
            SetViewBag(true);

            //Mặt bằng
            var matBang = _repository.GetRepository<MatBang>().GetAll();
            var model = new NhaCreatingViewModel();
            var listMatBangArr = new List<MatBangItem>();
            if (matBang.Any())
            {
                foreach (var item in matBang)
                {
                    listMatBangArr.Add(new MatBangItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                }
            }
            model.ListMatBangArr = listMatBangArr;

            //Đánh giá phù hợp với
            var danhGia = _repository.GetRepository<DanhGiaPhuHopVoi>().GetAll();
            var listDanhGiaArr = new List<DanhGiaPhuHopVoiItem>();
            if (danhGia.Any())
            {
                foreach (var item in danhGia)
                {
                    listDanhGiaArr.Add(new DanhGiaPhuHopVoiItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                }
            }
            model.ListDanhGiaPhuHopVoiArr = listDanhGiaArr;

            return View(model);
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

                decimal giaThueBQ = 0;

                if (!string.IsNullOrEmpty(model.TongGiaThue) && !string.IsNullOrEmpty(model.TongDienTichSuDung))
                {
                    giaThueBQ = Convert.ToDecimal(model.TongGiaThue) / Convert.ToDecimal(model.TongDienTichSuDung);
                }


                nha.MatBangId = model.MatBangId;
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
                nha.DanhGiaPhuHopVoiId = model.DanhGiaPhuHopVoiId;
                nha.TongGiaThue = string.IsNullOrEmpty(model.TongGiaThue) ? 0 : Convert.ToDecimal(model.TongGiaThue);
                nha.GiaThueBQ = giaThueBQ; //string.IsNullOrEmpty(model.GiaThueBQ) ? 0 : Convert.ToDecimal(model.GiaThueBQ);
                nha.TenNguoiLienHeVaiTro = StringHelper.KillChars(model.TenNguoiLienHeVaiTro);
                nha.SoDienThoai = StringHelper.KillChars(model.SoDienThoai);
                nha.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : DateTime.ParseExact(model.NgayCNHenLienHeLai, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                nha.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                nha.ImageDescription1 = StringHelper.KillChars(model.ImageDescription1);
                nha.ImageDescription2 = StringHelper.KillChars(model.ImageDescription2);
                nha.ImageDescription3 = StringHelper.KillChars(model.ImageDescription3);
                nha.ImageDescription4 = StringHelper.KillChars(model.ImageDescription4);
                nha.GhiChu = StringHelper.KillChars(model.GhiChu);
                nha.NgayTao = DateTime.Now;
                nha.NguoiTaoId = AccountId;
                nha.NguoiPhuTrachId = AccountId;
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

        protected void SetViewBag(bool isCreate)
        {
            //Mặt bằng
            //var matBang = _repository.GetRepository<MatBang>().GetAll();
            //ViewBag.MatBang = matBang.ToList().ToSelectList();

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
                var quanUpdate = _repository.GetRepository<Quan>().GetAll();
                ViewBag.QuanDropdownlist = quanUpdate.ToList().ToSelectList();

                var duong = _repository.GetRepository<Duong>().GetAll();
                ViewBag.DuongDropdownlist = duong.ToList().ToSelectList();
            }

            //var quan = _repository.GetRepository<Quan>().GetAll().OrderBy(o => o.Name).ToList();
            //ViewBag.QuanDropdownlist = new SelectList(quan, "Id", "Name", model.QuanId);
            //ViewBag.DuongDropdownlist = new SelectList(_repository.GetRepository<Duong>().GetAll(o => o.QuanId == model.QuanId).OrderBy(o => o.Name).ToList(), "Id", "Name", model.DuongId);

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
            //var danhGiaPhuHopVoi = _repository.GetRepository<DanhGiaPhuHopVoi>().GetAll();
            //ViewBag.DanhGiaPhuHopVoi = danhGiaPhuHopVoi.ToList().ToSelectList();

            //Cấp độ theo dõi
            var capDoTheoDoi = _repository.GetRepository<CapDoTheoDoi>().GetAll();
            ViewBag.CapDoTheoDoi = capDoTheoDoi.ToList().ToSelectList();
        }

        [Route("danh-sach-nha-json", Name = "NhaGetNhaJson")]
        //public ActionResult GetNhaJson(int status, decimal giaTu, decimal giaDen)
        public ActionResult GetNhaJson(int status)
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

                //Ghi chú
                string ghiChu = "";
                string objectGhiChu = Request.Params["objectGhiChu"];
                if (!string.IsNullOrEmpty(objectGhiChu))
                    ghiChu = objectGhiChu.ToString();

                //Quận
                long quanId = 0;
                string objectQuan = Request.Params["objectQuan"];
                if (!string.IsNullOrEmpty(objectQuan))
                    long.TryParse(objectQuan.ToString(), out quanId);
                //Đường
                long duongId = 0;
                string objectDuong = Request.Params["objectDuong"];
                if (!string.IsNullOrEmpty(objectDuong))
                    long.TryParse(objectDuong.ToString(), out duongId);

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

                //Giá thuê
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

                //DTSD tầng 1
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

                //Trạng thái bài viết
                string objectStatus = Request.Params["objectStatus"];//Lọc trạng thái bài viết
                if (!string.IsNullOrEmpty(objectStatus))
                    int.TryParse(objectStatus.ToString(), out status);

                //Tổng DTSD
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

                var articles = _repository.GetRepository<Nha>().GetAll(ref paging,
                                                                       orderKey,
                                                                       o => (key == null ||
                                                                             key == "") &&
                                                                             (o.GhiChu.Contains(ghiChu) || ghiChu == "") &&
                                                                             (o.QuanId == quanId || quanId == 0) &&
                                                                             (o.DuongId == duongId || duongId == 0) &&
                                                                             (matTienTu <= o.MatTienTreoBien && o.MatTienTreoBien <= matTienDen) &&
                                                                              (giaThueTu <= o.TongGiaThue && o.TongGiaThue <= giaThueDen) &&
                                                                              (dtsdt1Tu <= o.DienTichDatSuDungTang1 && o.DienTichDatSuDungTang1 <= dtsdt1Den) &&
                                                                             (o.TrangThai == status) &&
                                                                              (tongDTSDTu <= o.TongDienTichSuDung && o.TongDienTichSuDung <= tongDTSDDen) &&
                                                                             (isAdmin ? 1 == 1 : o.NguoiPhuTrachId == AccountId))
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
                        TrangThai = o.Nha.Nha.Nha.TrangThai == 0 ? "Chờ duyệt" : "Đã duyệt",
                        MatTien = o.Nha.Nha.Nha.MatTienTreoBien,
                        DTSDT1 = o.Nha.Nha.Nha.DienTichDatSuDungTang1,
                        TongDTSD = o.Nha.Nha.Nha.TongDienTichSuDung
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Route("cap-nhat-nha/{id?}", Name = "NhaUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Update(long id)
        {
            Nha nha = await _repository.GetRepository<Nha>().ReadAsync(id);

            SetViewBag(false);
            //SetViewBag();

            var matBang = _repository.GetRepository<MatBang>().GetAll();
            var listMatBangArr = new List<MatBangItem>();

            if (matBang.Any())
            {
                foreach (var item in matBang)
                {
                    listMatBangArr.Add(new MatBangItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                }
            }

            if (!string.IsNullOrEmpty(nha.MatBangId))
            {
                string[] arrmatbangid = nha.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

            var danhGia = _repository.GetRepository<DanhGiaPhuHopVoi>().GetAll();
            var listDanhGiaArr = new List<DanhGiaPhuHopVoiItem>();
            if (danhGia.Any())
            {
                foreach (var item in danhGia)
                {
                    listDanhGiaArr.Add(new DanhGiaPhuHopVoiItem { FieldKey = item.Id, FieldName = item.Name, IsSelected = false });
                }
            }

            if (!string.IsNullOrEmpty(nha.DanhGiaPhuHopVoiId))
            {
                string[] arrdanhgiaid = nha.DanhGiaPhuHopVoiId.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrdanhgiaid.Count() > 0)
                {
                    for (var i = 0; i < arrdanhgiaid.Count(); i++)
                    {
                        foreach (var item in listDanhGiaArr.Where(w => w.FieldKey == Convert.ToInt32(arrdanhgiaid[i])))
                        {
                            item.IsSelected = true;
                        }
                    }
                }
            }  

            NhaUpdatingViewModel model = new NhaUpdatingViewModel()
            {
                Id = nha.Id,
                SoNha = nha.SoNha,
                //MatBangId = nha.MatBangId,
                ListMatBangArr = listMatBangArr,
                ListDanhGiaPhuHopVoiArr = listDanhGiaArr,
                QuanId = nha.QuanId.ToString(),
                DuongId = nha.DuongId.ToString(),
                TenToaNha = nha.TenToaNha,
                NoiThatKhachThueCuId = nha.NoiThatKhachThueCuId.ToString(),
                //DanhGiaPhuHopVoiId = nha.DanhGiaPhuHopVoiId.ToString(),
                CapDoTheoDoiId = nha.CapDoTheoDoiId.ToString(),
                MatTienTreoBien = nha.MatTienTreoBien.ToString(),
                BeNgangLotLong = nha.BeNgangLotLong.ToString(),
                DienTichDat = nha.DienTichDat.ToString(),
                DienTichDatSuDungTang1 = nha.DienTichDatSuDungTang1.ToString(),
                SoTang = nha.SoTang.ToString(),
                TongDienTichSuDung = nha.TongDienTichSuDung.ToString(),
                DiChungChu = nha.DiChungChu ? "1" : "0",
                Ham = nha.Ham ? "1" : "0",
                ThangMay = nha.ThangMay ? "1" : "0",
                TongGiaThue = nha.TongGiaThue.ToString(),
                GiaThueBQ = nha.GiaThueBQ.ToString(),
                TenNguoiLienHeVaiTro = nha.TenNguoiLienHeVaiTro,
                SoDienThoai = nha.SoDienThoai,
                NgayCNHenLienHeLai = nha.NgayCNHenLienHeLai.HasValue ? nha.NgayCNHenLienHeLai.Value.ToString("dd/MM/yyyy") : "",
                ImageDescription1 = nha.ImageDescription1,
                ImageDescription2 = nha.ImageDescription2,
                ImageDescription3 = nha.ImageDescription3,
                ImageDescription4 = nha.ImageDescription4,
                GhiChu = nha.GhiChu,
            };

            //Backup model để so sánh xem có update gì không, sau đó lưu vào lịch sử
            _modelBK = model;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("cap-nhat-nha")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> Update(long id, NhaUpdatingViewModel model, NhaUpdatingViewModel _modelBK)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Nha nha = await _repository.GetRepository<Nha>().ReadAsync(id); 

                    decimal giaThueBQ = 0;

                    if (!string.IsNullOrEmpty(model.TongGiaThue) && !string.IsNullOrEmpty(model.TongDienTichSuDung))
                    {
                        giaThueBQ = Convert.ToDecimal(model.TongGiaThue) / Convert.ToDecimal(model.TongDienTichSuDung);
                    }

                    nha.MatBangId = model.MatBangId;
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
                    nha.DanhGiaPhuHopVoiId = model.DanhGiaPhuHopVoiId; 
                    nha.TongGiaThue = string.IsNullOrEmpty(model.TongGiaThue) ? 0 : Convert.ToDecimal(model.TongGiaThue);
                    nha.GiaThueBQ = giaThueBQ; //string.IsNullOrEmpty(model.GiaThueBQ) ? 0 : Convert.ToDecimal(model.GiaThueBQ);
                    nha.TenNguoiLienHeVaiTro = model.TenNguoiLienHeVaiTro;
                    nha.SoDienThoai = model.SoDienThoai;
                    nha.NgayCNHenLienHeLai = string.IsNullOrEmpty(model.NgayCNHenLienHeLai) ? (DateTime?)null : Convert.ToDateTime(model.NgayCNHenLienHeLai); //DateTime.ParseExact(model.NgayCNHenLienHeLai, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    nha.CapDoTheoDoiId = Convert.ToInt32(model.CapDoTheoDoiId);
                    nha.ImageDescription1 = model.ImageDescription1;
                    nha.ImageDescription2 = model.ImageDescription2;
                    nha.ImageDescription3 = model.ImageDescription3;
                    nha.ImageDescription4 = model.ImageDescription4;
                    nha.GhiChu = model.GhiChu;

                    int result = await _repository.GetRepository<Nha>().UpdateAsync(nha, AccountId);

                    //So sánh để tìm ra nội dung thay đổi
                    if (result > 0)
                    {
                        //string noiDungThayDoi = "";
                        //string strOld = "";
                        //string strNew = "";

                        ////Mặt bằng
                        //foreach (var item in _modelBK.ListMatBangArr)
                        //{
                        //    strOld = strOld == "" ? item.FieldName : strOld + ", " + item.FieldName;
                        //}

                        //foreach (var item in nha.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        //{
                        //    string name = _repository.GetRepository<MatBang>().GetAll().Where(t => t.Id == Convert.ToInt32(item)).Select(o => o.Name).ToString();
                        //    strNew = strNew == "" ? name : strNew + ", " + name;
                        //}

                        //if (strNew != strOld)
                        //{
                        //    noiDungThayDoi = "Sửa mặt bằng từ: " + strOld + " thành: " + strNew;
                        //}

                        ////Quận
                        //strOld = strNew = "";

                        //if (_modelBK.QuanId != nha.QuanId.ToString())
                        //{
                        //    strOld = _repository.GetRepository<Quan>().GetAll().Where(t => t.Id == Convert.ToInt32(_modelBK.QuanId)).Select(o => o.Name).ToString();
                        //    strNew = _repository.GetRepository<Quan>().GetAll().Where(t => t.Id == Convert.ToInt32(nha.QuanId)).Select(o => o.Name).ToString();

                        //    noiDungThayDoi = noiDungThayDoi == "" ? "Sửa quận từ: " + strOld + " thành: " + strNew : noiDungThayDoi + "<br/>" + "Sửa quận từ: " + strOld + " thành: " + strNew;
                        //}

                        ////Đường
                        //strOld = strNew = "";

                        //if (_modelBK.DuongId != nha.DuongId.ToString())
                        //{
                        //    strOld = _repository.GetRepository<Duong>().GetAll().Where(t => t.Id == Convert.ToInt32(_modelBK.DuongId)).Select(o => o.Name).ToString();
                        //    strNew = _repository.GetRepository<Duong>().GetAll().Where(t => t.Id == Convert.ToInt32(nha.DuongId)).Select(o => o.Name).ToString();

                        //    noiDungThayDoi = noiDungThayDoi == "" ? "Sửa đường từ: " + strOld + " thành: " + strNew : noiDungThayDoi + "<br/>" + "Sửa đường từ: " + strOld + " thành: " + strNew;
                        //}

                        ////Số nhà
                        //strOld = strNew = "";

                        //if (_modelBK.SoNha != nha.SoNha)
                        //{
                        //    noiDungThayDoi = noiDungThayDoi == "" ? "Sửa số nhà từ: " + _modelBK.SoNha + " thành: " + nha.SoNha : noiDungThayDoi + "<br/>" + "Sửa số nhà từ: " + _modelBK.SoNha + " thành: " + nha.SoNha;
                        //}

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
                    //var quan = _repository.GetRepository<Quan>().GetAll().OrderBy(o => o.Name).ToList();
                    //ViewBag.QuanDropdownlist = new SelectList(quan, "Id", "Name", model.QuanId);
                    //ViewBag.DuongDropdownlist = new SelectList(_repository.GetRepository<Duong>().GetAll(o => o.QuanId == model.QuanId).OrderBy(o => o.Name).ToList(), "Id", "Name", model.DuongId);

                    ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                throw;
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
            if (!string.IsNullOrEmpty(article.MatBangId))
            {
                string[] matbangarr = article.MatBangId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (matbangarr.Count() > 0)
                {
                    for (var i = 0; i < matbangarr.Count(); i++)
                    {
                        if (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i])) != null)
                        {
                            ViewBag.MatBang += (await _repository.GetRepository<MatBang>().ReadAsync(Convert.ToInt64(matbangarr[i]))).Name;
                            if (matbangarr[i] != matbangarr[matbangarr.Count() - 1])
                            {
                                ViewBag.MatBang += @"</br>";
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(article.DanhGiaPhuHopVoiId))
            {
                string[] danhgiaarr = article.DanhGiaPhuHopVoiId.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (danhgiaarr.Count() > 0)
                {
                    for (var i = 0; i < danhgiaarr.Count(); i++)
                    {
                        if (await _repository.GetRepository<DanhGiaPhuHopVoi>().ReadAsync(Convert.ToInt32(danhgiaarr[i])) != null)
                        {
                            ViewBag.DanhGiaPhuHopVoi += (await _repository.GetRepository<DanhGiaPhuHopVoi>().ReadAsync(Convert.ToInt32(danhgiaarr[i]))).Name;
                            if (danhgiaarr[i] != danhgiaarr[danhgiaarr.Count() - 1])
                            {
                                ViewBag.DanhGiaPhuHopVoi += @"</br>";
                            }
                        }
                    }
                }
            }

            ViewBag.Quan = (await _repository.GetRepository<Quan>().ReadAsync(article.QuanId)).Name;
            ViewBag.Duong = (await _repository.GetRepository<Duong>().ReadAsync(article.DuongId)).Name;
            ViewBag.NoiThatKhachThueCu = (await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(article.NoiThatKhachThueCuId)).Name;
            //ViewBag.DanhGiaPhuHopVoi = (await _repository.GetRepository<DanhGiaPhuHopVoi>().ReadAsync(article.DanhGiaPhuHopVoiId)).Name;
            ViewBag.CapDoTheoDoi = (await _repository.GetRepository<CapDoTheoDoi>().ReadAsync(article.CapDoTheoDoiId)).Name;

            //ViewBag.CreateBy = account.Name;
            //ViewBag.ArticleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
            return PartialView("_DetailModal", article);
        }

        [Route("tim-khach-cho-nha/{id?}", Name = "TimKhach")]
        public async Task<ActionResult> TimKhach(long id)
        {
            try
            {
                ViewBag.NhaId = id;

                var nha = await _repository.GetRepository<Nha>().ReadAsync(id);

                var result = _repository.GetRepository<NhuCauThue>().GetAll()
                                                                    .Where(delegate(NhuCauThue nct)
                {
                    return (nct.MatTienTreoBien <= (nha.MatTienTreoBien * 0.75)) &&
                            (nct.DienTichDatSuDungTang1 <= (nha.DienTichDatSuDungTang1 * 0.75)) &&
                            (nct.TongDienTichSuDung <= (nha.TongDienTichSuDung * 0.75)) &&
                           (nct.QuanId.Equals(nha.QuanId) &&
                           (nct.DuongId.Equals(nha.DuongId)));
                })
                 .Join(_repository.GetRepository<Khach>().GetAll(), b => b.KhachId, c => c.Id, (b, c) => new { NhuCauThue = b, Khach = c }).ToList();

                var data = result.Select(o => new KhachThueUpdatingViewModel
                {
                    Id = o.NhuCauThue.Id,
                    KhachId = o.NhuCauThue.KhachId.ToString(),
                    TenKhach = o.Khach.TenNguoiLienHeVaiTro,
                    SoDienThoai = o.Khach.SoDienThoai,
                    QuanName = o.NhuCauThue.QuanName,
                    DuongName = o.NhuCauThue.DuongName,
                    DienTichDat = o.NhuCauThue.DienTichDat.ToString(),
                    TongGiaThue = o.NhuCauThue.TongGiaThue.ToString()
                });

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

                return PartialView("TimKhachModal", data);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Route("dang-tin-rao-vat/{id?}", Name = "RaoVatModal")]
        public async Task<ActionResult> RaoVat(long id)
        {
            try
            {
                var nha = await _repository.GetRepository<Nha>().ReadAsync(id);

                //string matBang = (await _repository.GetRepository<MatBang>().ReadAsync(nha.MatBangId)).Name;
                string quan = (await _repository.GetRepository<Quan>().ReadAsync(nha.QuanId)).Name;
                string duong = (await _repository.GetRepository<Duong>().ReadAsync(nha.DuongId)).Name;
                string noiThatKhachThueCu = (await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(nha.NoiThatKhachThueCuId)).Name;
                //string danhGiaPhuHopVoi = (await _repository.GetRepository<DanhGiaPhuHopVoi>().ReadAsync(nha.DanhGiaPhuHopVoiId)).Name;
                string capDoTheoDoi = (await _repository.GetRepository<CapDoTheoDoi>().ReadAsync(nha.CapDoTheoDoiId)).Name;

                string strContent = "Cho thuê nhà ở ";

                if (nha.DuongId != null || nha.DuongId != 0)
                {
                    strContent += "đường ";
                    strContent += duong + ", ";

                }

                if (nha.QuanId != null || nha.QuanId != 0)
                {
                    strContent += "quận ";
                    strContent += quan;
                }

                //if (nha.DanhGiaPhuHopVoiId != null || nha.DanhGiaPhuHopVoiId != 0)
                //{
                //    strContent += ", phù hợp với ";
                //    strContent += danhGiaPhuHopVoi + ". <br/>";
                //}

                if (nha.MatTienTreoBien != null || nha.MatTienTreoBien != 0)
                {
                    strContent += "Mặt tiền treo biển là ";
                    strContent += nha.MatTienTreoBien + " m. <br/>";
                }

                if (nha.TongDienTichSuDung != null || nha.TongDienTichSuDung != 0)
                {
                    strContent += "Tổng diện tích đất sử dụng: ";
                    strContent += nha.TongDienTichSuDung + " m2. <br/>";
                }

                if (nha.TongGiaThue != null || nha.TongGiaThue != 0)
                {
                    strContent += "Tổng giá thuê: ";
                    strContent += nha.TongGiaThue + " VNĐ.";
                }

                ViewBag.Content = strContent;

                return PartialView("RaoVatModal");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Route("phan-cong/{id?}/{khachId?}/{nhaId?}/{accountId?}", Name = "PhanCong")]
        public async Task<ActionResult> PhanCong(long id, long khachId, long nhaId, long accountId) //id: NhuCauThueId
        {
            if (ModelState.IsValid)
            {
                QuanLyCongViecViewModel quanLyCongViecModel = new QuanLyCongViecViewModel();

                QuanLyCongViec qlcv = new QuanLyCongViec();

                qlcv.NhanVienPhuTrachId = accountId;
                qlcv.KhachId = khachId;
                qlcv.NhaId = nhaId;
                qlcv.NhuCauThueId = id;
                qlcv.NgayTao = DateTime.Now;
                qlcv.NguoiTaoId = AccountId;

                qlcv.NhaHiddenField = "SoNha,SoDienThoai,NguoiLienHe";
                qlcv.NgayHoanThanh = DateTime.Now.AddDays(Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["HanHoanThanh"]));

                qlcv.TrangThai = 0; //Chờ duyệt

                int result = 0;
                try
                {
                    result = await _repository.GetRepository<QuanLyCongViec>().CreateAsync(qlcv, AccountId);
                }
                catch { }
                if (result > 0)
                {
                    TempData["Success"] = "Phân công thành công!";
                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Phân công không thành công! Vui lòng kiểm tra và thử lại!");
                return View();
            }
        }
    }
}