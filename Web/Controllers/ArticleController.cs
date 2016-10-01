using Common;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace Web.Controllers
{
    //[RoutePrefix("bai-viet")]
    public class ArticleController : BaseController
    {
        /// <summary>
        /// Chi tiết bài viết
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("~/{categoryName}/{title}/{categoryId}/{id}", Name = "FrontEndArticleDetail")]
        public async Task<ActionResult> Detail(string categoryName, string title, long categoryId, long id)
        {
            Category category = await _repository.GetRepository<Category>().ReadAsync(categoryId);
            Category rootCategory = new SharedController().GetRootCategory(category);
            ViewBag.RootCategory = rootCategory;
            ViewBag.CategoryId = categoryId;
            ViewBag.Category = category;
            var article = _repository.GetRepository<Article>().Read(id);
            if(article.Status!=4)
            {
                return RedirectToRoute("FrontEndHomeIndex");
            }
            article.Views = article.Views + 1;
            try
            {
                await _repository.GetRepository<Article>().UpdateAsync(article, 0);
            }
            catch { }
            return View(article);
        }
        [Route("~/bai-viet/{title}/{id}", Name = "FrontEndArticleDetail2")]
        public async Task<ActionResult> Detail(string title, long id)
        {
            var articleCategory = await _repository.GetRepository<ArticleCategory>().ReadAsync(o => o.ArticleId == id);
            if (articleCategory != null)
            {
                Category category = await _repository.GetRepository<Category>().ReadAsync(articleCategory.CategoryId);
                Category rootCategory = new SharedController().GetRootCategory(category);
                ViewBag.RootCategory = rootCategory;
                ViewBag.CategoryId = articleCategory.CategoryId;
                ViewBag.Category = category;
            }
            var article = _repository.GetRepository<Article>().Read(id);
            if (article.Status != 4)
            {
                return RedirectToRoute("FrontEndHomeIndex");
            }
            article.Views = article.Views + 1;
            try
            {
                await _repository.GetRepository<Article>().UpdateAsync(article, 0);
            }
            catch { }
            return View(article);
        }
        ///// <summary>
        ///// Danh sách bản tin theo chuyên mục
        ///// </summary>
        ///// <param name="title"></param>
        ///// <param name="categoryId"></param>
        ///// <returns></returns>
        //[Route("~/chuyen-muc/{categoryName}/{categoryId}/{p?}", Name = "FrontEndArticleIndex")]
        //public async Task<ActionResult> Index(string categoryName, long categoryId, int p = 1)
        //{
        //    Category category = await _repository.GetRepository<Category>().ReadAsync(categoryId);
        //    ViewBag.Category = category;
        //    if (category.CategoryId == null && category.DisplayType.HasValue && category.DisplayType.Value == 2)
        //    {
        //        List<Category> categories = new List<Category>();
        //        if (category.Categories != null && category.Categories.Any())
        //        {
        //            foreach (var item in category.Categories.OrderBy(o => o.OrdinalNumber))
        //            {
        //                if (item.Categories != null && item.Categories.Any())
        //                {
        //                    categories.AddRange(item.Categories.OrderBy(o => o.OrdinalNumber));
        //                }
        //            }
        //        }
        //        return View("Category", categories);
        //    }
        //    Category rootCategory = new SharedController().GetRootCategory(category);
        //    ViewBag.CategoryName = category.Name;
        //    ViewBag.RootCategory = rootCategory;
        //    ViewBag.CategoryId = categoryId;

        //    int take = 20;
        //    int skip = (p - 1) * take;
        //    var articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
        //          .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == categoryId), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
        //    if (articles == null || articles.AsQueryable().Count() == 0)
        //    {
        //        var categoryIds = GetChildCategoryIds(rootCategory);
        //        articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
        //          .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => categoryIds.Contains(o.CategoryId)), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article).ToList<Article>();
        //    }
        //    ViewBag.PagingInfo = new PagingInfo
        //    {
        //        CurrentPage = p,
        //        ItemsPerPage = take,
        //        TotalItems = articles.AsQueryable().Count()
        //    };
        //    articles = articles.Skip(skip).Take(take).ToList();



        //    return View(articles);
        //}
        /// <summary>
        /// Danh sách bản tin theo chuyên mục
        /// </summary>
        /// <param name="title"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [Route("~/chuyen-muc/{categoryName}/{categoryId}/{p?}", Name = "FrontEndArticleIndex")]
        public async Task<ActionResult> Index(string categoryName, long categoryId, int p = 1)
        {
            Category category = await _repository.GetRepository<Category>().ReadAsync(categoryId);
            Category rootCategory = new SharedController().GetRootCategory(category);
            ViewBag.CategoryName = category.Name;
            ViewBag.RootCategory = rootCategory;
            ViewBag.CategoryId = categoryId;
            ViewBag.Category = category;

            if (category.CategoryId == null && category.DisplayType.HasValue &&
                (category.DisplayType.Value == 1 || category.DisplayType.Value == 3 || category.DisplayType.Value == 5))
            {
                if (category.DisplayType.Value == 5)
                {
                    List<Category> categories = new List<Category>();
                    if (category.Categories != null && category.Categories.Any())
                    {
                        foreach (var item in category.Categories.OrderBy(o => o.OrdinalNumber))
                        {
                            if (item.Categories != null && item.Categories.Any())
                            {
                                categories.AddRange(item.Categories.OrderBy(o => o.OrdinalNumber));
                            }
                        }
                    }
                    return View("Category5", categories);
                }
                if (category.DisplayType.Value == 1)
                    return View("Category1");//Không cần sanh sách bài viết
                if (category.DisplayType.Value == 3)
                    return View("Category3");//Không cần sanh sách bài viết
            }

            int take = 12;
            int skip = (p - 1) * take;
            var articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                  .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == categoryId), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article)
                  .OrderByDescending(o => o.UpdateDate).ToList<Article>();
            if (articles == null || articles.AsQueryable().Count() == 0)
            {
                //var categoryIds = GetChildCategoryIds(rootCategory);
                var categoryIds = GetChildCategoryIds(category);
                articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                  .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => categoryIds.Contains(o.CategoryId)), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article)
                  .OrderByDescending(o => o.UpdateDate).ToList<Article>();
            }
            ViewBag.PagingInfo = new PagingInfo
            {
                CurrentPage = p,
                ItemsPerPage = take,
                TotalItems = articles.AsQueryable().Count()
            };
            articles = articles.OrderByDescending(o => o.UpdateDate).Skip(skip).Take(take).ToList();

            if (category.CategoryId == null && category.DisplayType.HasValue && category.DisplayType.Value == 2)
                return View("Category2", articles);
            if (category.CategoryId == null && category.DisplayType.HasValue && category.DisplayType.Value == 4)
                return View("Category4", articles);
            //return View(articles);//Bỏ cái này
            return View("Category2", articles);

        }
        public ActionResult ArticlesByCategory1(long categoryId)
        {
            Category category = _repository.GetRepository<Category>().Read(categoryId);
            ViewBag.Category = category;
            var categoryIds = GetChildCategoryIds(category);
            var model = new List<Article>();
            var articles = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                  .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId == categoryId), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article)
                  .OrderByDescending(o => o.UpdateDate).Take(10).ToList<Article>();
            if (articles == null || articles.AsQueryable().Count() < 10)
            {
                int take = 0;
                if (articles != null)
                {
                    model.AddRange(articles);
                    int count = articles.AsQueryable().Count();
                    take = 10 - count;
                }
                var articles2 = _repository.GetRepository<Article>().GetAll(o => o.Status == 4)
                   .Join(_repository.GetRepository<ArticleCategory>().GetAll(o => o.CategoryId != categoryId && categoryIds.Contains(o.CategoryId)), b => b.Id, c => c.ArticleId, (b, c) => new { Article = b }).Select(x => x.Article)
                   .OrderByDescending(o => o.UpdateDate).Take(take).ToList<Article>();
                model.AddRange(articles2);
            }
            //articles = articles.Take(10).ToList();
            return PartialView("_ArticlesByCategory1", model);
        }
        private List<long> GetChildCategoryIds(Category parentCategory)
        {
            List<long> childIds = new List<long>();
            childIds.Add(parentCategory.Id);
            if (parentCategory.Categories != null && parentCategory.Categories.Any())
            {
                foreach (var item in parentCategory.Categories)
                {
                    childIds.AddRange(GetChildCategoryIds(item));
                }
            }
            return childIds;
        }
        //private List<long> GetChildCategoryIds(long parentCategoryId)
        //{
        //    List<long> childIds = new List<long>();
        //    childIds.Add(parentCategoryId);
        //    var donVis = _repository.GetRepository<Category>().GetAll(o => o.CategoryId == parentCategoryId);
        //    if (donVis.Any())
        //    {
        //        foreach (var item in donVis)
        //        {
        //            childIds.Add(item.Id);
        //            if (item.Categories.Any())
        //            {
        //                childIds.AddRange(GetChildCategoryIds(item.Id));
        //            }
        //        }
        //    }
        //    return childIds;
        //}
    }
}