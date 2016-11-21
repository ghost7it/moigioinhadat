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

namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("quan-ly-du-lieu")]
    public class ImportController : BaseController
    {
        [Route("import-du-lieu", Name = "ImportIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Import)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// import
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("import-nha-modal", Name = "ImportNhaModal")]
        public async Task<ActionResult> ImportNha()
        {

            return PartialView("ImportNhaModal");
        }

        [HttpPost]
        [Route("import-nha")]
        public ActionResult ImportNha(HttpPostedFileBase uploadFile)
        {
            StringBuilder strValidations = new StringBuilder(string.Empty);
            try
            {
                if (uploadFile.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/files"), Path.GetFileName(uploadFile.FileName));
                    uploadFile.SaveAs(filePath);
                    DataSet ds = new DataSet();

                    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";

                    using (OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            string sheetName = "Data$";
                            string query = "SELECT * FROM [" + sheetName + "]";
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                            adapter.Fill(ds, "Items");
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    Nha nha = new Nha();
                                    //fdsfdsf
                                    int resultCount = 0;
                                    int recordExcelCount = ds.Tables[0].Rows.Count;

                                    using (TransactionScope tscope = new TransactionScope())
                                    {
                                        foreach (DataRow r in ds.Tables[0].Rows)
                                        {
                                            string matBang = Convert.ToString(r["Mặt bằng"]);
                                            long matBangId = (_repository.GetRepository<MatBang>().Read(o => o.Name == matBang)).Id;

                                            string quan = Convert.ToString(r["Quận"]);
                                            long quanId = (_repository.GetRepository<Quan>().Read(o => o.Name == quan)).Id;

                                            string duong = Convert.ToString(r["Đường"]);
                                            long duongId = (_repository.GetRepository<Duong>().Read(o => o.Name == duong)).Id;

                                            string noiThatKhachThueCu = Convert.ToString(r["Nội thất khách thuê cũ"]);
                                            long noiThatKhachThueCuId = (_repository.GetRepository<NoiThatKhachThueCu>().Read(o => o.Name == noiThatKhachThueCu)).Id;


                                            string danhGiaPhuHopVoi = Convert.ToString(r["Đánh giá phù hợp với"]);
                                            long danhGiaPhuHopVoiId = (_repository.GetRepository<DanhGiaPhuHopVoi>().Read(o => o.Name == danhGiaPhuHopVoi)).Id;

                                            string capDoTheoDoi = Convert.ToString(r["Cấp độ theo dõi"]);
                                            int capDoTheoDoiId = (_repository.GetRepository<CapDoTheoDoi>().Read(o => o.Name == capDoTheoDoi)).Id;

                                            nha.MatBangId = matBangId;
                                            nha.QuanId = quanId;
                                            nha.DuongId = duongId;
                                            nha.SoNha = r["Số nhà"] == DBNull.Value ? "" : r["Số nhà"].ToString();
                                            nha.TenToaNha = r["Tên toàn nhà"] == DBNull.Value ? "" : (string)r["Tên toàn nhà"];
                                            nha.MatTienTreoBien = r["Mặt tiền treo biển"] == DBNull.Value ? 0 : float.Parse(r["Mặt tiền treo biển"].ToString());
                                            nha.BeNgangLotLong = r["Bề ngang lọt lòng"] == DBNull.Value ? 0 : float.Parse(r["Bề ngang lọt lòng"].ToString());
                                            nha.DienTichDat = r["Diện tích đất"] == DBNull.Value ? 0 : float.Parse(r["Diện tích đất"].ToString());
                                            nha.DienTichDatSuDungTang1 = r["Diện tích đất sử dụng tầng 1"] == DBNull.Value ? 0 : float.Parse(r["Diện tích đất sử dụng tầng 1"].ToString());
                                            nha.SoTang = r["Số tầng"] == DBNull.Value ? 0 : int.Parse(r["Số tầng"].ToString());
                                            nha.TongDienTichSuDung = r["Tổng diện tích sử dụng"] == DBNull.Value ? 0 : int.Parse(r["Tổng diện tích sử dụng"].ToString());
                                            nha.DiChungChu = (string)r["Đi chung chủ"] == "Có" ? true : false;
                                            nha.Ham = (string)r["Hầm"] == "Có" ? true : false;
                                            nha.ThangMay = (string)r["Thang máy"] == "Có" ? true : false;
                                            nha.NoiThatKhachThueCuId = noiThatKhachThueCuId;
                                            nha.DanhGiaPhuHopVoiId = danhGiaPhuHopVoiId;
                                            nha.TongGiaThue = r["Tổng giá thuê"] == DBNull.Value ? 0 : decimal.Parse(r["Tổng giá thuê"].ToString());
                                            nha.GiaThueBQ = r["Giá thuê BQ"] == DBNull.Value ? 0 : decimal.Parse(r["Giá thuê BQ"].ToString());
                                            nha.TenNguoiLienHeVaiTro = r["Tên người liên hệ - vai trò"] == DBNull.Value ? "" : (string)r["Tên người liên hệ - vai trò"];
                                            nha.SoDienThoai = r["Số điện thoại"] == DBNull.Value ? "" : r["Số điện thoại"].ToString();
                                            nha.NgayCNHenLienHeLai = r["Ngày CN hẹn liên hệ lại"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["Ngày CN hẹn liên hệ lại"]);
                                            nha.CapDoTheoDoiId = capDoTheoDoiId;
                                            nha.GhiChu = r["Ghi chú"] == DBNull.Value ? "" : (string)r["Ghi chú"];
                                            nha.NgayTao = DateTime.Now;
                                            nha.NguoiTaoId = AccountId;
                                            nha.TrangThai = 0; //Chờ duyệt

                                            int result = 0;
                                            try
                                            {
                                                result = _repository.GetRepository<Nha>().Create(nha, AccountId);
                                            }
                                            catch { }
                                            if (result > 0)
                                            {
                                                resultCount++;
                                            }

                                        }

                                        if (resultCount == recordExcelCount)
                                        {
                                            tscope.Complete();

                                            TempData["Success"] = "Import dữ liệu thành công!";

                                            return RedirectToAction("Index");
                                        }
                                        else
                                        {
                                            Transaction.Current.Rollback();
                                            tscope.Dispose();

                                            TempData["Error"] = "Import dữ liệu không thành công, vui lòng thử lại!";
                                            return RedirectToAction("Index");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Import dữ liệu không thành công, vui lòng thử lại!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// import
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("import-khach-modal", Name = "ImportKhachModal")]
        public async Task<ActionResult> ImportKhach()
        {

            return PartialView("ImportKhachModal");
        }


        [HttpPost]
        [Route("import-khach")]
        public ActionResult ImportKhach(HttpPostedFileBase uploadFile)
        {
            StringBuilder strValidations = new StringBuilder(string.Empty);
            try
            {
                if (uploadFile.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/files"), Path.GetFileName(uploadFile.FileName));
                    uploadFile.SaveAs(filePath);
                    DataSet ds = new DataSet();

                    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";

                    using (OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            string sheetName = "Data$";
                            string query = "SELECT * FROM [" + sheetName + "]";
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                            adapter.Fill(ds, "Items");

                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    Khach khach = new Khach();
                                    NhuCauThue nhuCauThue = new NhuCauThue();

                                    int resultCount = 0;
                                    int recordExcelCount = ds.Tables[0].Rows.Count;
                                    long khachIdIddentify = 0;

                                    //Bắt đầu trans
                                    using (TransactionScope tscope = new TransactionScope())
                                    {
                                        foreach (DataRow r in ds.Tables[0].Rows)
                                        {
                                            string matBang = Convert.ToString(r["Mặt bằng"]);
                                            int matBangId = (_repository.GetRepository<MatBang>().Read(o => o.Name == matBang)).Id;

                                            string quan = Convert.ToString(r["Quận"]);
                                            long quanId = (_repository.GetRepository<Quan>().Read(o => o.Name == quan)).Id;

                                            string duong = Convert.ToString(r["Đường"]);
                                            long duongId = (_repository.GetRepository<Duong>().Read(o => o.Name == duong)).Id;

                                            string noiThatKhachThueCu = Convert.ToString(r["Nội thất khách thuê cũ"]);
                                            int noiThatKhachThueCuId = (_repository.GetRepository<NoiThatKhachThueCu>().Read(o => o.Name == noiThatKhachThueCu)).Id;

                                            string capDoTheoDoi = Convert.ToString(r["Cấp độ theo dõi"]);
                                            int capDoTheoDoiId = (_repository.GetRepository<CapDoTheoDoi>().Read(o => o.Name == capDoTheoDoi)).Id;

                                            //Lưu bảng Khách
                                            khach.TenNguoiLienHeVaiTro = r["Tên người liên hệ - vai trò"] == DBNull.Value ? "" : (string)r["Tên người liên hệ - vai trò"];
                                            khach.SoDienThoai = r["Số điện thoại"] == DBNull.Value ? "" : (string)r["Số điện thoại"];
                                            khach.GhiChu = r["Ghi chú"] == DBNull.Value ? "" : (string)r["Ghi chú"];
                                            khach.NgayTao = DateTime.Now;
                                            khach.NguoiTaoId = AccountId;
                                            khach.TrangThai = 0;

                                            int result1 = 0;
                                            try
                                            {
                                                result1 = _repository.GetRepository<Khach>().Create(khach, AccountId);
                                            }
                                            catch (Exception ex) { }
                                            if (result1 > 0)
                                            {
                                                //Lưu bảng Nhu cầu thuê
                                                khachIdIddentify = khach.Id;

                                                nhuCauThue.KhachId = khachIdIddentify;
                                                nhuCauThue.MatBangId = matBangId;
                                                nhuCauThue.QuanId = quanId;
                                                nhuCauThue.QuanName = quan;
                                                nhuCauThue.DuongName = duong;
                                                nhuCauThue.DuongId = duongId;
                                                nhuCauThue.SoNha = r["Số nhà"] == DBNull.Value ? "" : r["Số nhà"].ToString();
                                                nhuCauThue.TenToaNha = r["Tên toàn nhà"] == DBNull.Value ? "" : (string)r["Tên toàn nhà"];
                                                nhuCauThue.MatTienTreoBien = r["Mặt tiền treo biển"] == DBNull.Value ? 0 : float.Parse(r["Mặt tiền treo biển"].ToString());
                                                nhuCauThue.BeNgangLotLong = r["Bề ngang lọt lòng"] == DBNull.Value ? 0 : float.Parse(r["Bề ngang lọt lòng"].ToString());
                                                nhuCauThue.DienTichDat = r["Diện tích đất"] == DBNull.Value ? 0 : float.Parse(r["Diện tích đất"].ToString());
                                                nhuCauThue.DienTichDatSuDungTang1 = r["Diện tích đất sử dụng tầng 1"] == DBNull.Value ? 0 : float.Parse(r["Diện tích đất sử dụng tầng 1"].ToString());
                                                nhuCauThue.SoTang = r["Số tầng"] == DBNull.Value ? 0 : int.Parse(r["Số tầng"].ToString());
                                                nhuCauThue.TongDienTichSuDung = r["Tổng diện tích sử dụng"] == DBNull.Value ? 0 : int.Parse(r["Tổng diện tích sử dụng"].ToString());
                                                nhuCauThue.DiChungChu = (string)r["Đi chung chủ"] == "Có" ? true : false;
                                                nhuCauThue.Ham = (string)r["Hầm"] == "Có" ? true : false;
                                                nhuCauThue.ThangMay = (string)r["Thang máy"] == "Có" ? true : false;
                                                nhuCauThue.NoiThatKhachThueCuId = noiThatKhachThueCuId;
                                                nhuCauThue.TongGiaThue = r["Tổng giá thuê"] == DBNull.Value ? 0 : decimal.Parse(r["Tổng giá thuê"].ToString());
                                                nhuCauThue.GiaThueBQ = r["Giá thuê BQ"] == DBNull.Value ? 0 : decimal.Parse(r["Giá thuê BQ"].ToString());
                                                nhuCauThue.NgayCNHenLienHeLai = r["Ngày CN hẹn liên hệ lại"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["Ngày CN hẹn liên hệ lại"]);
                                                nhuCauThue.CapDoTheoDoiId = capDoTheoDoiId;
                                                nhuCauThue.GhiChu = r["Ghi chú"] == DBNull.Value ? "" : (string)r["Ghi chú"];
                                                nhuCauThue.NgayTao = DateTime.Now;
                                                nhuCauThue.NguoiTaoId = AccountId;
                                                nhuCauThue.TrangThai = 0; //Chờ duyệt

                                                int result2 = 0;
                                                try
                                                {
                                                    result2 = _repository.GetRepository<NhuCauThue>().Create(nhuCauThue, AccountId);
                                                }
                                                catch { }
                                                if (result2 > 0)
                                                {
                                                    resultCount++;
                                                }
                                            }
                                        }

                                        if (resultCount == recordExcelCount)
                                        {
                                            tscope.Complete();

                                            TempData["Success"] = "Import dữ liệu thành công!";

                                            return RedirectToAction("Index");
                                        }
                                        else
                                        {
                                            Transaction.Current.Rollback();
                                            tscope.Dispose();

                                            TempData["Error"] = "Import dữ liệu không thành công, vui lòng thử lại!";
                                            return RedirectToAction("Index");
                                        }
                                    }
                                    //Kết thúc trans
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Import dữ liệu không thành công, vui lòng thử lại!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}