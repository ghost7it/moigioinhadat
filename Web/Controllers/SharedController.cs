using Entities.Models;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
namespace Web.Controllers
{
    //[RoutePrefix("chia-se")]
    public class SharedController : BaseController
    {
        //Menu con bên phải (bên cạnh menu chính)
        public ActionResult RightMenu()
        {
            var categories = new List<Category>();
            if (SystemInformation.RightMenu.HasValue)
                categories = _repository.GetRepository<Category>().GetAll(o => o.CategoryTypeId == SystemInformation.RightMenu.Value).ToList();
            return PartialView("_RightMenu", categories);
        }
        //Menu chính
        public ActionResult MainMenu()
        {
            var categories = new List<Category>();
            if (SystemInformation.MainMenu.HasValue)
                categories = _repository.GetRepository<Category>().GetAll(o => o.CategoryTypeId == SystemInformation.MainMenu.Value).ToList();
            return PartialView("_MainMenu", categories);
        }
        //Album để hiển thị slide show ở trang chủ
        public ActionResult HomeAlbum()
        {
            var albumDetails = new List<AlbumDetail>();
            if (SystemInformation.HomeAlbum.HasValue)
                albumDetails = _repository.GetRepository<AlbumDetail>().GetAll(o => o.AlbumId == SystemInformation.HomeAlbum.Value).ToList();
            return PartialView("_HomeAlbum", albumDetails);
        }
        //Album hiển thị 4 box con ở trang chủ bên cạnh slide
        public ActionResult RightAlbum()
        {
            var albumDetails = new List<AlbumDetail>();
            if (SystemInformation.RightAlbum.HasValue)
                albumDetails = _repository.GetRepository<AlbumDetail>().GetAll(o => o.AlbumId == SystemInformation.RightAlbum.Value).ToList();
            return PartialView("_RightAlbum", albumDetails);
        }
        //Album ảnh các đối tác
        public ActionResult BottomAlbum()
        {
            var albumDetails = new List<AlbumDetail>();
            if (SystemInformation.BottomAlbum.HasValue)
            {
                Album album = _repository.GetRepository<Album>().Read(SystemInformation.BottomAlbum.Value);
                if (album != null)
                    ViewBag.AlbumName = album.Name;
                albumDetails = _repository.GetRepository<AlbumDetail>().GetAll(o => o.AlbumId == SystemInformation.BottomAlbum.Value).ToList();
            }
            return PartialView("_BottomAlbum", albumDetails);
        }
        //Danh sách liên kết cuối trang
        public ActionResult BottomMenu()
        {
            var categories = new List<Category>();
            if (SystemInformation.BottomMenu.HasValue)
                categories = _repository.GetRepository<Category>().GetAll(o => o.CategoryTypeId == SystemInformation.BottomMenu.Value).ToList();
            return PartialView("_BottomMenu", categories);
        }
        //
        public ActionResult FooterLink()
        {
            var categories = new List<Category>();
            if (SystemInformation.MainMenu.HasValue)
                categories = _repository.GetRepository<Category>().GetAll(o => o.CategoryTypeId == SystemInformation.MainMenu.Value).ToList();
            ViewBag.Copyright = SystemInformation.Copyright;
            return PartialView("_FooterLink", categories);
        }
        //Khối tin chính ở bên trái, trang chủ
        public ActionResult LeftArticle1()
        {
            var articles = new List<Article>();
            if (SystemInformation.LeftArticle1.HasValue)
            {
                ViewBag.CategoryId = SystemInformation.LeftArticle1.Value;
                var category = _repository.GetRepository<Category>().Read(SystemInformation.LeftArticle1.Value);
                if (category != null)
                    ViewBag.CategoryName = category.Name;
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                    .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == SystemInformation.LeftArticle1.Value), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
            }
            return PartialView("_LeftArticle1", articles);
        }
        //Khối tin chính thứ 2 ở bên trái, trang chủ
        public ActionResult LeftArticle2()
        {
            var articles = new List<Article>();
            if (SystemInformation.LeftArticle2.HasValue)
            {
                ViewBag.CategoryId = SystemInformation.LeftArticle2.Value;
                var category = _repository.GetRepository<Category>().Read(SystemInformation.LeftArticle2.Value);
                if (category != null)
                    ViewBag.CategoryName = category.Name;
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                    .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == SystemInformation.LeftArticle2.Value), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
            }
            return PartialView("_LeftArticle2", articles);
        }
        //Khối tin bên phải thứ nhất (có event)
        public ActionResult RightArticle1()
        {
            var articles = new List<Article>();
            if (SystemInformation.RightArticle1.HasValue)
            {
                ViewBag.CategoryId = SystemInformation.RightArticle1.Value;
                var category = _repository.GetRepository<Category>().Read(SystemInformation.RightArticle1.Value);
                if (category != null)
                    ViewBag.CategoryName = category.Name;
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                   .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == SystemInformation.RightArticle1.Value), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
            }
            return PartialView("_RightArticle1", articles);
        }
        //Khối tin bên phải thứ hai
        public ActionResult RightArticle2()
        {
            var articles = new List<Article>();
            if (SystemInformation.RightArticle2.HasValue)
            {
                ViewBag.CategoryId = SystemInformation.RightArticle2.Value;
                var category = _repository.GetRepository<Category>().Read(SystemInformation.RightArticle2.Value);
                if (category != null)
                {
                    ViewBag.CategoryName = category.Name;
                    ViewBag.CategoryDescription = category.Description;
                }
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                   .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == SystemInformation.RightArticle2.Value), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).OrderByDescending(y => y.Id).Take(3).ToList<Article>();
            }
            return PartialView("_RightArticle2", articles);
        }
        //Lấy chuyên mục cấp cha cao nhất
        public Category GetRootCategory(Category category)
        {
            if (category.ParentCategory == null) return category;
            else
            {
                return GetRootCategory(category.ParentCategory);
            }
        }
        //Lấy tất cả các id của cây chuyên mục thuộc tất cả các cấp
        public List<long> GetChildCategoryIds(long parentCategoryId)
        {
            List<long> childIds = new List<long>();
            childIds.Add(parentCategoryId);
            var categories = _repository.GetRepository<Category>().GetAll(o => o.CategoryId == parentCategoryId);
            if (categories.Any())
            {
                foreach (var item in categories)
                {
                    childIds.Add(item.Id);
                    if (item.Categories.Any())
                    {
                        childIds.AddRange(GetChildCategoryIds(item.Id));
                    }
                }
            }
            return childIds;
        }
        //Danh sách bài viết được xem nhiều nhất theo chuyên mục
        public ActionResult MostViewed(long categoryId, string categoryName)
        {
            var articles = new List<Article>();
            var categoryIds = GetChildCategoryIds(categoryId);
            articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
               .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => categoryIds.Contains(o.CategoryId)), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).OrderByDescending(y => y.Views).Take(10).ToList<Article>();
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = categoryName;
            return PartialView("_MostViewed", articles);
        }
        //Danh sách bài viết khác đã xuất bản cùng chuyên mục
        public ActionResult Featured(long categoryId, long articleId, string categoryName)
        {
            var articles = new List<Article>();
            var categoryIds = GetChildCategoryIds(categoryId);
            articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4 && o.Id != articleId)
               .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => categoryIds.Contains(o.CategoryId)), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).OrderByDescending(y => y.Views).Take(10).ToList<Article>();
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = categoryName;
            return PartialView("_Featured", articles);
        }
        //Danh sách bài viết được xem nhiều nhất
        public ActionResult MostViewed2()
        {
            var articles = new List<Article>();
            articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4).OrderByDescending(o => o.Views).Take(20).ToList<Article>();            
            return PartialView("_MostViewed2", articles);
        }
        //Danh sách bài viết khác đã xuất bản cùng chuyên mục
        public ActionResult Featured2(long categoryId, long articleId, string categoryName)
        {
            var articles = new List<Article>();
            var categoryIds = GetChildCategoryIds(categoryId);
            articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4 && o.Id != articleId)
               .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => categoryIds.Contains(o.CategoryId)), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).OrderByDescending(y => y.Views).Take(10).ToList<Article>();
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = categoryName;
            return PartialView("_Featured2", articles);
        }
        public List<Organization> GetOrganizations(long? parentOrganizationId, bool isSystemOwner = false, int level = 0, bool showPrefix = true)
        {
            List<Organization> listOrganization = new List<Organization>();
            try
            {
                StringBuilder prefix = new StringBuilder();
                for (int i = 0; i < level; i++)
                {
                    prefix.Append("---");
                }
                var organizations = _repository.GetRepository<Organization>()
                    .GetAll(o => (parentOrganizationId == null ? o.OrganizationId == null : o.OrganizationId == parentOrganizationId))
                    .OrderBy(o => o.Name).ToList<Organization>();
                if (level == 0 && isSystemOwner)
                {
                    organizations = organizations.Where(o => o.IsSystemOwner == true).ToList();
                }
                foreach (var item in organizations)
                {
                    item.Name = showPrefix == true ? (prefix.ToString() + " " + item.Name) : item.Name;
                    listOrganization.Add(item);
                    //listOrganization.Add(new Organization
                    //{
                    //    Id = item.Id,
                    //    Name = showPrefix == true ? (prefix.ToString() + " " + item.Name) : item.Name,
                    //    OrganizationId = parentOrganizationId
                    //});
                    //Nếu có con thì gọi đệ quy
                    if (item.Organizations != null && item.Organizations.Any())
                    {
                        listOrganization.AddRange(GetOrganizations(item.Id, isSystemOwner, level + 1, showPrefix));
                    }
                }
                return listOrganization;
            }
            catch
            {
                return listOrganization;
            }
        }
        [Route("~/danh-sach-nganh-theo-don-vi-shared/{organizationId?}", Name = "SharedGetMajorsByOrganization")]
        public JsonResult GetMajorsByOrganization(long? organizationId)
        {
            try
            {
                if (organizationId.HasValue)
                {
                    var majors = _repository.GetRepository<Majors>().GetAll(o => o.OrganizationId == organizationId).OrderBy(o => o.Name).ToList();
                    //majors = majors.ToSelectList();
                    var result = majors.Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    });
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new List<Majors>().ToSelectList(), JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(new List<Majors>().ToSelectList(), JsonRequestBehavior.AllowGet);
            }
        }
        [Route("~/danh-sach-khoa-theo-nganh-shared/{majorsId?}", Name = "SharedGetCourseByMajors")]
        public JsonResult GetCourseByMajors(long? majorsId)
        {
            try
            {
                if (majorsId.HasValue)
                {
                    var majors = _repository.GetRepository<Course>().GetAll(o => o.MajorsId == majorsId).OrderBy(o => o.Name).ToList();
                    var result = majors.Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    });
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new List<Course>().ToSelectList(), JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(new List<Course>().ToSelectList(), JsonRequestBehavior.AllowGet);
            }
        }
        [Route("~/danh-sach-quan-huyen-theo-tinh-tp-shared/{majorsId?}", Name = "SharedGetDistrictsByProvince")]
        public JsonResult GetDistrictsByProvince(long? provinceId)
        {
            try
            {
                if (provinceId.HasValue)
                {
                    var districts = _repository.GetRepository<District>().GetAll(o => o.ProvinceId == provinceId).OrderBy(o => o.Name).ToList();
                    var result = districts.Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    });
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new List<Course>().ToSelectList(), JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(new List<Course>().ToSelectList(), JsonRequestBehavior.AllowGet);
            }
        }

        [Route("~/danh-sach-duong-theo-quan-shared/{majorsId?}", Name = "SharedGetDuongByQuan")]
        public JsonResult GetDuongsByQuan(long? quanId)
        {
            try
            {
                if (quanId.HasValue)
                {
                    var duong = _repository.GetRepository<Duong>().GetAll(o => o.QuanId == quanId).OrderBy(o => o.Name).ToList();
                    var result = duong.Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    });
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new List<Course>().ToSelectList(), JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(new List<Course>().ToSelectList(), JsonRequestBehavior.AllowGet);
            }
        }

        //Khối tin chính ở bên trái, trang chủ (Dùng cho chuyên mục hiển thị danh sách chuyên mục con)
        public ActionResult LeftArticle1Category()
        {
            var articles = new List<Article>();
            if (SystemInformation.LeftArticle1.HasValue)
            {
                ViewBag.CategoryId = SystemInformation.LeftArticle1.Value;
                var category = _repository.GetRepository<Category>().Read(SystemInformation.LeftArticle1.Value);
                if (category != null)
                    ViewBag.CategoryName = category.Name;
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                    .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == SystemInformation.LeftArticle1.Value), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
            }
            return PartialView("_LeftArticle1Category", articles);
        }
        //Khối tin chính thứ 2 ở bên trái, trang chủ (Dùng cho chuyên mục hiển thị danh sách chuyên mục con)
        public ActionResult LeftArticle2Category()
        {
            var articles = new List<Article>();
            if (SystemInformation.LeftArticle2.HasValue)
            {
                ViewBag.CategoryId = SystemInformation.LeftArticle2.Value;
                var category = _repository.GetRepository<Category>().Read(SystemInformation.LeftArticle2.Value);
                if (category != null)
                    ViewBag.CategoryName = category.Name;
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                    .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == SystemInformation.LeftArticle2.Value), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
            }
            return PartialView("_LeftArticle2Category", articles);
        }
    }
}