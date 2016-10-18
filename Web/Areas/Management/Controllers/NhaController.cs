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
                nha.SoTang = 0;
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


        [Route("danh-sach-nha-json", Name = "GetNhaJson")]
        public ActionResult GetNhaJson(byte status)
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
                                                                         o.SoDienThoai.Contains(key)) &&
                                                                         (o.TrangThai == status))
                                                                         .Join(_repository.GetRepository<MatBang>().GetAll(), b => b.MatBangId, c => c.Id, (b, c) => new { Nha = b, MatBang = c })
                                                                         .Join(_repository.GetRepository<Quan>().GetAll(), b => b.Nha.QuanId, c => c.Id, (b, c) => new { Nha = b, Quan = c })
                                                                         .Join(_repository.GetRepository<Duong>().GetAll(), b => b.Nha.Nha.DuongId, c => c.Id, (b, c) => new { Nha = b, Duong = c })
                                                                         .Join(_repository.GetRepository<Duong>().GetAll(), b => b.Nha.Nha.Nha.NoiThatKhachThueCuId, c => c.Id, (b, c) => new { Nha = b, NoiThatKhachThueCu = c }).ToList();

            return Json(new
            {
                draw = drawReturn,
                recordsTotal = paging.TotalRecord,
                recordsFiltered = paging.TotalRecord,
                data = articles.Select(o => new
                {
                    o.Article.Id,
                    o.Article.Title,
                    UpdateDate = o.Article.UpdateDate.HasValue ? o.Article.UpdateDate.Value.ToString("dd/MM/yyyy") : o.Article.CreateDate.ToString("dd/MM/yyyy"),
                    CreateBy = o.Account.Name,
                    Status = ((Entities.Enums.ArticleStatusEnum)o.Article.Status).GetDescription(),
                    CategoryName = string.Join(", ", _repository.GetRepository<ArticleCategory>().GetAll(x => x.ArticleId == o.Article.Id).Select(x => x.Category.Name).ToList())
                })
            }, JsonRequestBehavior.AllowGet);
        }

        //[Route("cap-nhat-bai-viet/{id?}", Name = "ArticleUpdate")]
        //[ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Article)]
        //public async Task<ActionResult> Update(long id)
        //{
        //    //ViewBag.Categories = await _repository.GetRepository<Category>().GetAllAsync(o => o.CategoryId == null);
        //    ViewBag.CategoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
        //    Article article = await _repository.GetRepository<Article>().ReadAsync(id);
        //    //Nếu bài viết có trạng thái KHÁC đang biên tập thì không cho sửa
        //    if (article.Status != 1)
        //    {
        //        TempData["Error"] = "Bài viết không cho phép sửa!";
        //        return RedirectToAction("Index");
        //    }
        //    ArticleUpdatingViewModel model = new ArticleUpdatingViewModel()
        //    {
        //        Id = article.Id,
        //        Title = article.Title,
        //        Description = article.Description,
        //        Content = article.Content,
        //        ImageDescription = article.ImageDescription,
        //        EventStartDate = article.EventStartDate.HasValue ? article.EventStartDate.Value.ToString("dd/MM/yyyy") : "",
        //        EventFinishDate = article.EventFinishDate.HasValue ? article.EventFinishDate.Value.ToString("dd/MM/yyyy") : ""
        //    };
        //    List<long> articleCategoryIds = new List<long>();
        //    var articleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
        //    if (articleCategory != null && articleCategory.Any())
        //    {
        //        articleCategoryIds = articleCategory.Select(o => o.CategoryId).ToList();
        //    }
        //    ViewBag.articleCategoryIds = articleCategoryIds;
        //    return View(model);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //[Route("cap-nhat-bai-viet")]
        //[ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Article)]
        //public async Task<ActionResult> Update(long id, ArticleUpdatingViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Article article = await _repository.GetRepository<Article>().ReadAsync(id);
        //        article.Title = StringHelper.KillChars(model.Title);
        //        article.Description = StringHelper.KillChars(model.Description);
        //        article.Content = StringHelper.KillChars(model.Content);
        //        article.ImageDescription = StringHelper.KillChars(model.ImageDescription);
        //        article.UpdateDate = DateTime.Now;
        //        article.UpdateBy = AccountId;
        //        if (!string.IsNullOrEmpty(model.EventStartDate))
        //        {
        //            try
        //            {
        //                DateTime date = DateTime.ParseExact(model.EventStartDate, "dd/MM/yyyy", null);
        //                article.EventStartDate = date;
        //            }
        //            catch
        //            {
        //                article.EventStartDate = null;
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(model.EventFinishDate))
        //        {
        //            try
        //            {
        //                DateTime date = DateTime.ParseExact(model.EventFinishDate, "dd/MM/yyyy", null);
        //                article.EventFinishDate = date;
        //            }
        //            catch
        //            {
        //                article.EventFinishDate = null;
        //            }
        //        }
        //        int result = await _repository.GetRepository<Article>().UpdateAsync(article, AccountId);
        //        if (result > 0)
        //        {
        //            //Cập nhật danh mục cho bài viết
        //            List<long> articleCategoryIds = new List<long>();
        //            //Danh sách danh mục hiện tại của bài viết
        //            var articleCategory1 = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
        //            //articleCategory1 = article.ArticleCategories;//Chưa thử lại cách này
        //            //Id Danh sách danh mục hiện tại của bài viết
        //            if (articleCategory1 != null && articleCategory1.Any())
        //            {
        //                articleCategoryIds = articleCategory1.Select(o => o.CategoryId).ToList();
        //            }

        //            //Nếu người dùng chọn danh mục nào đó (hoặc danh mục đã được chọn từ lúc biên tập)
        //            if (model.CategoryId != null && model.CategoryId.Any())
        //            {
        //                var articleCategory = model.CategoryId.Select(o => new ArticleCategory()
        //                {
        //                    ArticleId = article.Id,
        //                    CategoryId = Convert.ToInt64(o.ToString())
        //                });
        //                //Danh sách danh mục hiện tại không chứa danh mục được chọn thì xóa
        //                var articleCategoryDelete = articleCategory1.Where(o => !articleCategory.Any(x => x.CategoryId == o.CategoryId));
        //                try
        //                {
        //                    int result2 = await _repository.GetRepository<ArticleCategory>().DeleteAsync(articleCategoryDelete, AccountId);
        //                }
        //                catch { }
        //                //Danh sách danh mục được chọn không có trong danh mục hiện tại thì thêm
        //                var articleCategoryAdd = articleCategory.Where(o => !articleCategoryIds.Contains(o.CategoryId));
        //                try
        //                {
        //                    int result2 = await _repository.GetRepository<ArticleCategory>().CreateAsync(articleCategoryAdd, AccountId);
        //                }
        //                catch { }
        //            }
        //            else
        //            {//Nếu người dùng không chọn vào danh mục nào thì xóa tất cái hiện tại
        //                int result3 = await _repository.GetRepository<ArticleCategory>().DeleteAsync(articleCategory1, AccountId);
        //            }
        //            TempData["Success"] = "Cập nhật bài viết thành công!";
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            //ViewBag.Categories = await _repository.GetRepository<Category>().GetAllAsync(o => o.CategoryId == null);
        //            ViewBag.CategoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
        //            List<long> articleCategoryIds = new List<long>();
        //            var articleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
        //            if (articleCategory != null && articleCategory.Any())
        //            {
        //                articleCategoryIds = articleCategory.Select(o => o.CategoryId).ToList();
        //            }
        //            ViewBag.listArticleCategory = articleCategoryIds;
        //            ModelState.AddModelError(string.Empty, "Cập nhật bài viết không thành công! Vui lòng kiểm tra và thử lại!");
        //            return View(model);
        //        }
        //    }
        //    else
        //    {
        //        //ViewBag.Categories = await _repository.GetRepository<Category>().GetAllAsync(o => o.CategoryId == null);
        //        ViewBag.CategoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
        //        List<long> articleCategoryIds = new List<long>();
        //        var articleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
        //        if (articleCategory != null && articleCategory.Any())
        //        {
        //            articleCategoryIds = articleCategory.Select(o => o.CategoryId).ToList();
        //        }
        //        ViewBag.articleCategoryIds = articleCategoryIds;
        //        ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
        //        return View(model);
        //    }
        //}
        //[Route("xuat-ban-bai-viet", Name = "ArticlePublish")]
        //[ValidationPermission(Action = ActionEnum.Publish, Module = ModuleEnum.ArticlePublish)]
        //public ActionResult Publish()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("xet-trang-thai-bai-viet/{ids?}/{status?}", Name = "ArticleSetArticleStatus")]
        //public async Task<ActionResult> SetArticleStatus(string ids, byte status)
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
        //                bool result = await SetArticleStatus(articleId, status);
        //                if (result)
        //                    succeed++;
        //            }
        //        return Json(new { success = true, message = string.Format(@"Đã ghi nhận thành công trạng thái {0} bài viết.", succeed) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        ///// <summary>
        ///// Xét trạng thái của bài viết
        ///// </summary>
        ///// <param name="articleId"></param>
        ///// <param name="status"></param>
        ///// <returns></returns>
        //private async Task<bool> SetArticleStatus(long articleId, byte status)
        //{
        //    var article = await _repository.GetRepository<Article>().ReadAsync(articleId);
        //    if (article == null)
        //        return false;
        //    article.Status = status;
        //    if (status == 4)
        //    {
        //        article.PublishDate = DateTime.Now;
        //        article.PublishBy = AccountId;
        //    }
        //    int result = await _repository.GetRepository<Article>().UpdateAsync(article, AccountId);
        //    if (result > 0)
        //    {
        //        //TODO: Ghi nhật ký
        //        return true;
        //    }
        //    return false;
        //}
        //[HttpPost]
        //[Route("xoa-bai-viet/{ids?}", Name = "ArticleDeleteArticles")]
        //[ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Article)]
        //public async Task<ActionResult> DeleteArticles(string ids)
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
        //                bool result = await DeleteArticles(articleId);
        //                if (result)
        //                    succeed++;
        //            }
        //        return Json(new { success = true, message = string.Format(@"Đã xóa thành công {0} bài viết.", succeed) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Không xóa được bài viết. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //private async Task<bool> DeleteArticles(long articleId)
        //{
        //    var article = await _repository.GetRepository<Article>().ReadAsync(articleId);
        //    if (article == null)
        //        return false;
        //    //Nếu bài viết đang biên tập hoặc được trả lại biên tập thì mới cho xóa
        //    if (article.Status == 1 || article.Status == 3)
        //    {
        //        //Xóa ArticleCategory
        //        await _repository.GetRepository<ArticleCategory>().DeleteAsync(o => o.ArticleId == articleId, AccountId);
        //        int result = await _repository.GetRepository<Article>().DeleteAsync(article, AccountId);
        //        if (result > 0)
        //        {
        //            //TODO: Ghi nhật ký
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        ///// <summary>
        ///// Xem chi tiết bài viết theo modal dialog
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Route("xem-chi-tiet-bai-viet-modal/{id?}", Name = "ArticleDetailModal")]
        //public async Task<ActionResult> DetailModal(long id)
        //{
        //    var article = await _repository.GetRepository<Article>().ReadAsync(id);
        //    var account = await _repository.GetRepository<Account>().ReadAsync(article.CreateBy);
        //    ViewBag.CreateBy = account.Name;
        //    //Lấy danh sách chuyên mục
        //    ViewBag.ArticleCategory = await _repository.GetRepository<ArticleCategory>().GetAllAsync(o => o.ArticleId == id);
        //    return PartialView("_DetailModal", article);
        //}
    }
}