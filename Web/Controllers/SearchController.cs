using Common;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    //[RoutePrefix("tim-kiem")]
    public class SearchController : BaseController
    {
        [Route("~/tim-kiem/{keywords?}/{p?}", Name = "FrontEndSearchIndex")]
        public ActionResult Index(string keywords, int p = 1)
        {
            int take = 12;
            int skip = (p - 1) * take;
            keywords = string.IsNullOrEmpty(keywords) ? "" : keywords.Trim();
            var articles = new List<Article>();
            if (!string.IsNullOrEmpty(keywords))
            {
                articles = _repository.GetRepository<Article>().GetAll(keywords).ToList();
            }

            ViewBag.PagingInfo = new PagingInfo
            {
                CurrentPage = p,
                ItemsPerPage = take,
                TotalItems = articles.AsQueryable().Count()
            };
            articles = articles.Skip(skip).Take(take).ToList();
            ViewBag.Keywords = keywords;
            return View(articles);
        }

    }
}