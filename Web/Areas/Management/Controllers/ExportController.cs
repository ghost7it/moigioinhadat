using Common;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
using Web.Areas.Management.Helpers;
using Web.Helpers;
using System.Transactions;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("quan-ly-du-lieu")]
    public class ExportController : BaseController
    {
        [Route("export-du-lieu", Name = "ExportIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Import)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("export-nha", Name = "ExportNha")]
        public ActionResult ExportNha()
        {
            var nha = _repository.GetRepository<Nha>().GetAll()
                                                       .Join(_repository.GetRepository<Quan>().GetAll(), b => b.QuanId, e => e.Id, (b, e) => new { Nha = b, Quan = e })
                                                       .Join(_repository.GetRepository<Duong>().GetAll(), b => b.Nha.DuongId, g => g.Id, (b, g) => new { Nha = b, Duong = g })
                //.Join(_repository.GetRepository<MatBang>().GetAll(), b => b.Nha.Nha.MatBangId, y => y.Id, (b, y) => new { Nha = b, MatBang = y })
                                                       .Join(_repository.GetRepository<NoiThatKhachThueCu>().GetAll(), b => b.Nha.Nha.NoiThatKhachThueCuId, k => k.Id, (b, k) => new { Nha = b, NoiThatKhachThueCu = k })
                //.Join(_repository.GetRepository<DanhGiaPhuHopVoi>().GetAll(), b => b.Nha.Nha.Nha.Nha.DanhGiaPhuHopVoiId, l => l.Id, (b, l) => new { Nha = b, DanhGiaPhuHopVoi = l })
                                                       .Join(_repository.GetRepository<CapDoTheoDoi>().GetAll(), b => b.Nha.Nha.Nha.CapDoTheoDoiId, m => m.Id, (b, m) => new { Nha = b, CapDoTheoDoi = m }).ToList();

            var data = nha.Select(
            p => new
            {
                MatBang = p.Nha.Nha.Nha.Nha.MatBangId,
                Quan = p.Nha.Nha.Nha.Quan.Name,
                Duong = p.Nha.Nha.Duong.Name,
                p.Nha.Nha.Nha.Nha.SoNha,
                p.Nha.Nha.Nha.Nha.TenToaNha,
                p.Nha.Nha.Nha.Nha.MatTienTreoBien,
                p.Nha.Nha.Nha.Nha.BeNgangLotLong,
                p.Nha.Nha.Nha.Nha.DienTichDat,
                p.Nha.Nha.Nha.Nha.DienTichDatSuDungTang1,
                p.Nha.Nha.Nha.Nha.SoTang,
                p.Nha.Nha.Nha.Nha.TongDienTichSuDung,
                DiChungChu = p.Nha.Nha.Nha.Nha.DiChungChu == true ? "Có" : "Không",
                Ham = p.Nha.Nha.Nha.Nha.Ham == true ? "Có" : "Không",
                ThangMay = p.Nha.Nha.Nha.Nha.ThangMay == true ? "Có" : "Không",
                NoiThatKhachThueCu = p.Nha.NoiThatKhachThueCu.Name,
                DanhGiaPhuHopVoi = p.Nha.Nha.Nha.Nha.DanhGiaPhuHopVoiId,
                p.Nha.Nha.Nha.Nha.TongGiaThue,
                p.Nha.Nha.Nha.Nha.GiaThueBQ,
                p.Nha.Nha.Nha.Nha.TenNguoiLienHeVaiTro,
                p.Nha.Nha.Nha.Nha.SoDienThoai,
                p.Nha.Nha.Nha.Nha.NgayCNHenLienHeLai,
                CapDoTheoDoi = p.CapDoTheoDoi.Name,
                p.Nha.Nha.Nha.Nha.GhiChu
            });

            //Loop để get mặt bằng name, đánh giá phù hợp với name
            List<object> listData = new List<object>();

            foreach (var item in data)
            {
                string danhGiaPhuHopVoi = "";
                string matBang = "";

                //Mặt bằng
                if (!string.IsNullOrEmpty(item.MatBang))
                {
                    string[] matBangArr = item.MatBang.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item2 in matBangArr)
                    {
                        string name = (_repository.GetRepository<MatBang>().Read(Convert.ToInt32(item2))).Name;

                        matBang = matBang == "" ? name : matBang + ", " + name;
                    }
                }

                //Đánh giá phù hợp với
                if (!string.IsNullOrEmpty(item.DanhGiaPhuHopVoi))
                {
                    string[] danhGiaArr = item.DanhGiaPhuHopVoi.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item2 in danhGiaArr)
                    {
                        string name = (_repository.GetRepository<DanhGiaPhuHopVoi>().Read(Convert.ToInt32(item2))).Name;

                        danhGiaPhuHopVoi = danhGiaPhuHopVoi == "" ? name : danhGiaPhuHopVoi + ", " + name;
                    }
                }

                listData.Add(new
                {
                    MatBang = matBang,
                    Quan = item.Quan,
                    Duong = item.Duong,
                    item.SoNha,
                    item.TenToaNha,
                    item.MatTienTreoBien,
                    item.BeNgangLotLong,
                    item.DienTichDat,
                    item.DienTichDatSuDungTang1,
                    item.SoTang,
                    item.TongDienTichSuDung,
                    DiChungChu = item.DiChungChu,
                    Ham = item.Ham,
                    ThangMay = item.ThangMay,
                    NoiThatKhachThueCu = item.NoiThatKhachThueCu,
                    DanhGiaPhuHopVoi = danhGiaPhuHopVoi,
                    item.TongGiaThue,
                    item.GiaThueBQ,
                    item.TenNguoiLienHeVaiTro,
                    item.SoDienThoai,
                    item.NgayCNHenLienHeLai,
                    item.CapDoTheoDoi,
                    item.GhiChu
                });
            }

            GridView gridview = new GridView();
            gridview.DataSource = listData;
            gridview.DataBind();

            // Clear all the content from the current response
            Response.ClearContent();
            Response.Buffer = true;

            // set the header
            Response.AddHeader("content-disposition", "attachment; filename=nha_data_export.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            // create HtmlTextWriter object with StringWriter
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //set font
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);

                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return View();
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <returns></returns>
        [Route("export-khach", Name = "ExportKhach")]
        public ActionResult ExportKhach()
        {

            var khachVaNhuCauThue = _repository.GetRepository<NhuCauThue>().GetAll()
                                                       .Join(_repository.GetRepository<Quan>().GetAll(), b => b.QuanId, e => e.Id, (b, e) => new { NhuCauThue = b, Quan = e })
                                                       .Join(_repository.GetRepository<Duong>().GetAll(), b => b.NhuCauThue.DuongId, g => g.Id, (b, g) => new { NhuCauThue = b, Duong = g })
                //.Join(_repository.GetRepository<MatBang>().GetAll(), b => b.NhuCauThue.NhuCauThue.MatBangId.Contains, y => y.Id, (b, y) => new { NhuCauThue = b, MatBang = y })
                                                       .Join(_repository.GetRepository<NoiThatKhachThueCu>().GetAll(), b => b.NhuCauThue.NhuCauThue.NoiThatKhachThueCuId, k => k.Id, (b, k) => new { NhuCauThue = b, NoiThatKhachThueCu = k })
                                                       .Join(_repository.GetRepository<Khach>().GetAll(), b => b.NhuCauThue.NhuCauThue.NhuCauThue.KhachId, n => n.Id, (b, n) => new { NhuCauThue = b, Khach = n })
                                                       .Join(_repository.GetRepository<CapDoTheoDoi>().GetAll(), b => b.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.CapDoTheoDoiId, m => m.Id, (b, m) => new { NhuCauThue = b, CapDoTheoDoi = m }).ToList();

            var data = khachVaNhuCauThue.Select(
            p => new
            {
                //Khách
                TenNguoiLienHeVaiTro = p.NhuCauThue.Khach.TenNguoiLienHeVaiTro,
                SoDienThoai = p.NhuCauThue.Khach.SoDienThoai,
                GhiChuKhach = p.NhuCauThue.Khach.GhiChu,

                //Nhu Cầu Thuê
                MatBang = p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.MatBangId,
                Quan = p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.Quan.Name,
                Duong = p.NhuCauThue.NhuCauThue.NhuCauThue.Duong.Name,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.SoNha,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.TenToaNha,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.MatTienTreoBien,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.BeNgangLotLong,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.DienTichDat,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.DienTichDatSuDungTang1,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.SoTang,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.TongDienTichSuDung,
                DiChungChu = p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.DiChungChu == true ? "Có" : "Không",
                Ham = p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.DiChungChu == true ? "Có" : "Không",
                ThangMay = p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.DiChungChu == true ? "Có" : "Không",
                NoiThatKhachThueCu = p.NhuCauThue.NhuCauThue.NoiThatKhachThueCu.Name,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.TongGiaThue,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.GiaThueBQ,
                p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NgayCNHenLienHeLai,
                CapDoTheoDoi = p.CapDoTheoDoi.Name,
                GhiChuNhuCauThue = p.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.NhuCauThue.GhiChu,
            });

            //Loop để get mặt bằng name
            List<object> listData = new List<object>();

            foreach (var item in data)
            {
                string matBang = "";

                //Mặt bằng
                if (!string.IsNullOrEmpty(item.MatBang))
                {
                    string[] matBangArr = item.MatBang.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item2 in matBangArr)
                    {
                        string name = (_repository.GetRepository<MatBang>().Read(Convert.ToInt32(item2))).Name;

                        matBang = matBang == "" ? name : matBang + ", " + name;
                    }
                }

                listData.Add(new
                {
                    //Khách
                    item.TenNguoiLienHeVaiTro,
                    item.SoDienThoai,
                    item.GhiChuKhach,

                    //Nhu Cầu Thuê
                    MatBang = matBang,
                    item.Quan,
                    item.Duong,
                    item.SoNha,
                    item.TenToaNha,
                    item.MatTienTreoBien,
                    item.BeNgangLotLong,
                    item.DienTichDat,
                    item.DienTichDatSuDungTang1,
                    item.SoTang,
                    item.TongDienTichSuDung,
                    item.DiChungChu,
                    item.Ham,
                    item.ThangMay,
                    item.NoiThatKhachThueCu,
                    item.TongGiaThue,
                    item.GiaThueBQ,
                    item.NgayCNHenLienHeLai,
                    item.CapDoTheoDoi,
                    item.GhiChuNhuCauThue
                });
            }

            GridView gridview = new GridView();
            gridview.DataSource = listData;
            gridview.DataBind();

            // Clear all the content from the current response
            Response.ClearContent();
            Response.Buffer = true;

            // set the header
            Response.AddHeader("content-disposition", "attachment; filename=khach_data_export.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            // create HtmlTextWriter object with StringWriter
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //set font
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);

                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            return View();
        }



    }
}