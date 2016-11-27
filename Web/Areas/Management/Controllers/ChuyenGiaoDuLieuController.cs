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
    public class ChuyenGiaoDuLieuController : BaseController
    {
        [Route("chuyen-giao-du-lieu", Name = "ChuyenGiaoDuLieuIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.ChuyenGiaoDuLieu)]
        public ActionResult Index()
        {

            InitData();

            return View();
        }

        [HttpPost]
        [Route("chuyen-giao-du-lieu", Name = "ChuyenGiaoDuLieu")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.ChuyenGiaoDuLieu)]
        public ActionResult ChuyenGiaoDuLieu()
        {
            long nguoiGiaoId = Request.Form["NguoiGiaoId"] == null ? 0 : Convert.ToInt64(Request.Form["NguoiGiaoId"]);
            long nguoiNhanId = Request.Form["nguoiNhanId"] == null ? 0 : Convert.ToInt64(Request.Form["nguoiNhanId"]);

            if (nguoiGiaoId == nguoiNhanId)
            {
                TempData["Error"] = "Người đang phụ trách bị trùng người tiếp nhận. Vui lòng chọn lại!";

                InitData();

                return View("Index");

            }

            //Bắt đầu trans
            using (TransactionScope tscope = new TransactionScope())
            {
                //Chuyển giao trong bảng Nhà
                Nha nha = _repository.GetRepository<Nha>().Read(o => o.NguoiTaoId == nguoiGiaoId);

                nha.NguoiPhuTrachId = nguoiNhanId;
                int result1 = _repository.GetRepository<Nha>().Update(nha, AccountId);

                if (result1 < 1)
                {
                    //Rollback
                    Transaction.Current.Rollback();
                    tscope.Dispose();

                    TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại!";
                    InitData();

                    return View("Index");
                }

                //Chuyển giao trong bảng Nhu cầu thuê
                NhuCauThue nhuCauThue = _repository.GetRepository<NhuCauThue>().Read(o => o.NguoiTaoId == nguoiGiaoId);

                nhuCauThue.NguoiPhuTrachId = nguoiNhanId;
                int result2 = _repository.GetRepository<NhuCauThue>().Update(nhuCauThue, AccountId);

                if (result2 < 1)
                {
                    //Rollback
                    Transaction.Current.Rollback();
                    tscope.Dispose();

                    TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại!";
                    InitData();

                    return View("Index");
                }

                //Chuyển giao trong bảng Khách
                Khach khach = _repository.GetRepository<Khach>().Read(o => o.NguoiTaoId == nguoiGiaoId);

                khach.NguoiPhuTrachId = nguoiNhanId;
                int result21 = _repository.GetRepository<Khach>().Update(khach, AccountId);

                if (result21 < 1)
                {
                    //Rollback
                    Transaction.Current.Rollback();
                    tscope.Dispose();

                    TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại!";
                    InitData();

                    return View("Index");
                }

                //Chuyển giao trong bảng Quản lý công việc
                QuanLyCongViec quanLyCongViec = _repository.GetRepository<QuanLyCongViec>().Read(o => o.NhanVienPhuTrachId == nguoiGiaoId);

                quanLyCongViec.NhanVienPhuTrachId = nguoiNhanId;
                int result3 = _repository.GetRepository<QuanLyCongViec>().Update(quanLyCongViec, AccountId);

                if (result3 < 1)
                {
                    //Rollback
                    Transaction.Current.Rollback();
                    tscope.Dispose();

                    TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại!";
                    InitData();

                    return View("Index");
                }

                tscope.Complete();

                TempData["Success"] = "Chuyển giao dữ liệu thành công!";
                InitData();

                return View("Index");
            }
        }

        public void InitData()
        {
            //Lấy danh sách nhân viên
            var account = _repository.GetRepository<AccountRole>().GetAll()
                 .Join(_repository.GetRepository<Account>().GetAll(), b => b.AccountId, c => c.Id, (b, c) => new { AccountRole = b, Account = c }).ToList();

            var obj = new List<object>();

            foreach (var item in account)
            {
                obj.Add(new { ID = item.Account.Id, Name = item.Account.Name });
            }

            var listAccount = new SelectList(obj, "ID", "Name", 1);

            ViewBag.Accounts = listAccount;
        }

    }
}