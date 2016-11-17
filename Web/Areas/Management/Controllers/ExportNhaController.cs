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
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.Drawing;


namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("export-pdf")]
    public class ExportNhaController : BaseController
    {
        [Route("danh-sach-nha-export", Name = "ExportNhaIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Export)]
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

        [Route("danh-sach-nha-export-json", Name = "ExportNhaGetNhaJson")]
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

            var articles = _repository.GetRepository<Nha>().GetAll(ref paging,
                                                                   orderKey,
                                                                   o => (key == null ||
                                                                         key == "" ||
                                                                         o.TenNguoiLienHeVaiTro.Contains(key) ||
                                                                         o.SoDienThoai.Contains(key)) &&
                                                                         (status == 3 ? true : o.TrangThai == status))
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
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Xem chi tiết bài viết theo modal dialog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("xem-chi-tiet-nha-modal/{id?}", Name = "ExportNhaDetailModal")]
        public async Task<ActionResult> DetailModal(long id)
        {
            var article = await _repository.GetRepository<Nha>().ReadAsync(id);

            //var account = await _repository.GetRepository<Account>().ReadAsync(article.NguoiTaoId);

            ViewBag.MatBang = (await _repository.GetRepository<MatBang>().ReadAsync(article.MatBangId)).Name;
            ViewBag.Quan = (await _repository.GetRepository<Quan>().ReadAsync(article.QuanId)).Name;
            ViewBag.Duong = (await _repository.GetRepository<Duong>().ReadAsync(article.DuongId)).Name;
            ViewBag.NoiThatKhachThueCu = (await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(article.NoiThatKhachThueCuId)).Name;
            ViewBag.DanhGiaPhuHopVoi = (await _repository.GetRepository<DanhGiaPhuHopVoi>().ReadAsync(article.DanhGiaPhuHopVoiId)).Name;
            ViewBag.CapDoTheoDoi = (await _repository.GetRepository<CapDoTheoDoi>().ReadAsync(article.CapDoTheoDoiId)).Name;

            //ViewBag.CreateBy = account.Name;
            //ViewBag.ArticleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
            return PartialView("_DetailModal", article);
        }

        //[HttpPost]
        //[Route("xoa-nha/{ids?}", Name = "NhaExportNhas")]
        //[ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Nha)]
        //public async Task<ActionResult> ExportNhas(string ids)
        //{
        //    try
        //    {
        //        byte succeed = 0;
        //        string[] articleIds = Regex.Split(ids, ",");
        //        if (articleIds != null && articleIds.Any())
        //            foreach (var item in articleIds)
        //            {
        //                long articleId = 0;
        //                long.TryParse(item, out articleId);
        //                bool result = await ExportNha(articleId);
        //                if (result)
        //                    succeed++;
        //            }
        //        return Json(new { success = true, message = string.Format(@"Đã export thành công {0} bản ghi.", succeed) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Không export được bài viết. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        [Route("export-nha/{ids?}", Name = "NhaExportNhas")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Nha)]
        public async Task<ActionResult> ExportNha(long ids)
        {
            var article = await _repository.GetRepository<Nha>().ReadAsync(ids);
            if (article == null)
                return View();
            //PdfDocument pdf = new PdfDocument();
            //pdf.Info.Title = "Export dữ liệu nhà";
            //PdfPage pdfPage = pdf.AddPage();
            //XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            //XFont font = new XFont("Arial", 13, XFontStyle.Regular);
            //graph.DrawString("File Test", font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
            //string pdfFilename = "firstpage" + string.Format("{0:ddMMyyyy_HHmmss}", DateTime.Now) + ".pdf";
            //pdf.Save(Server.MapPath("/Content/PDFFile/" + pdfFilename));
            //Process.Start(Server.MapPath("/Content/PDFFile/" + pdfFilename));

            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Export dữ liệu nhà";
            PdfPage pdfPage = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(pdfPage);
            //XRect rect;
            //XPen pen; 
            double x = 40, y = 70;
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            XFont fontH1 = new XFont("Arial", 16, XFontStyle.Bold, options);
            XFont font = new XFont("Arial", 11, XFontStyle.Regular, options);
            XFont fontItalic = new XFont("Arial", 12, XFontStyle.BoldItalic);
            double ls = font.GetHeight(gfx);
            // Draw some text
            gfx.DrawString("Dữ liệu nhà của " + article.TenNguoiLienHeVaiTro,
            fontH1, XBrushes.Black, x, x);
            gfx.DrawString("Tên người liên hệ: " + article.TenNguoiLienHeVaiTro, font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Số điện thoại: " + article.SoDienThoai,
            font, XBrushes.Black, x, y);
            y += 2 * ls;
            //// Draw a pie
            //pen = new XPen(XColors.DarkOrange, 1.5);
            //pen.DashStyle = XDashStyle.Dot;
            //gfx.DrawPie(pen, XBrushes.Blue, x + 360, y, 100, 60, -130, 135);
            // Draw some more text
            //y += 60 + 2 * ls;
            gfx.DrawString("Số nhà: " + article.SoNha, font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Tên tòa nhà: " + article.TenToaNha, font, XBrushes.Black, x, y);
            y += ls;
            var loaimatbang = await _repository.GetRepository<MatBang>().ReadAsync(article.MatBangId);
            gfx.DrawString("Loại mặt bằng: " + (loaimatbang != null?loaimatbang.Name:""), font, XBrushes.Black, x, y);
            y += ls * 1.1;
            double y1 = y;
            var quan = await _repository.GetRepository<Quan>().ReadAsync(article.QuanId);
            gfx.DrawString("Quận: " + (quan != null ? quan.Name : ""), font, XBrushes.Black, x, y);
            //x += 10;
            y += ls * 1.1;
            var duong = await _repository.GetRepository<Duong>().ReadAsync(article.DuongId);
            gfx.DrawString("Đường: " + (duong != null ? duong.Name : ""), font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Mặt tiền treo biển: " + article.MatTienTreoBien + " (m)", font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Bề ngang lọt lòng: " + article.BeNgangLotLong + " (m)", font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Diện tích đất: " + article.DienTichDat + " (m2)", font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Diện tích đất sử dụng tầng 1: " + article.DienTichDatSuDungTang1 + " (m2)", font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Tổng diện tích sử dụng: " + article.TongDienTichSuDung + " (m2)", font, XBrushes.Black, x, y);
            y += ls;
            gfx.DrawString("Đi chung chủ: "  + (article.DiChungChu?"Có":"Không"), font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            gfx.DrawString("Hầm: " + (article.Ham ? "Có" : "Không"), font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            gfx.DrawString("Thang máy: " + (article.ThangMay? "Có" : "Không"), font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            gfx.DrawString("Số tầng: " + article.SoTang, font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            var noithat = await _repository.GetRepository<NoiThatKhachThueCu>().ReadAsync(article.NoiThatKhachThueCuId);
            gfx.DrawString("Nội thất khách thuê cũ: " + (noithat != null ? noithat.Name : ""), font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            gfx.DrawString("Tổng giá thuê: "+ article.TongGiaThue + " (VNĐ)", font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            gfx.DrawString("Giá thuê bình quân: " + article.GiaThueBQ + " (VNĐ)", font, XBrushes.Black, x + 300, y1);
            y1 += ls;
            gfx.DrawString("Ghi chú: " + article.GhiChu , font, XBrushes.Black, x, y);
            y += ls * 1.5;
            gfx.DrawString("Ảnh mô tả", font, XBrushes.Firebrick, x, y);
            y += ls * 1;
            if (!string.IsNullOrEmpty(article.ImageDescription1))
            {
                AddImage(gfx, pdfPage, Server.MapPath(article.ImageDescription1), x, y);
                y += ls * 1;
            }
            if (!string.IsNullOrEmpty(article.ImageDescription2))
            {
                AddImage(gfx, pdfPage, Server.MapPath(article.ImageDescription2), x, y);
                y += ls * 1;
            }
            if (!string.IsNullOrEmpty(article.ImageDescription3))
            {
                AddImage(gfx, pdfPage, Server.MapPath(article.ImageDescription3), x, y);
                y += ls * 1;
            }
            if (!string.IsNullOrEmpty(article.ImageDescription4))
            {
                AddImage(gfx, pdfPage, Server.MapPath(article.ImageDescription4), x, y);
                y += ls * 1;
            }
            string pdfFilename = "ThongTinNha__" + string.Format("{0:ddMMyyyy_HHmmss}", DateTime.Now) + ".pdf";
            pdf.Save(Server.MapPath("/Content/PDFFile/" + pdfFilename));
            Process.Start(Server.MapPath("/Content/PDFFile/" + pdfFilename));
            //DownloadFile(Server.MapPath("/Content/PDFFile/Thongtinnha_13112016_022706.pdf"));
            return View();
        }

        void DownloadFile(string filePath)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
              if (file.Exists)
              {
                 Response.Clear();
                 Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                 Response.AddHeader("Content-Length", file.Length.ToString());
                 Response.ContentType = "application/pdf";
                 Response.WriteFile(file.FullName);
                 Response.End();
              }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        void AddImage(XGraphics gfx, PdfPage page, string imagePath, double xPosition, double yPosition)
        {
            if (System.IO.File.Exists(imagePath))
            {
                XImage xImage = XImage.FromFile(imagePath);
                gfx.DrawImage(xImage, xPosition, yPosition, xImage.PixelWidth, xImage.PixelWidth);
            }
            //throw new FileNotFoundException(String.Format("Could not find image {0}.", imagePath));
        }
        
 
    }
}