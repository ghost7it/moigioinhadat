using Entities.Models;
using Entities.ViewModels;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("chia-se")]
    public class SharedController : Controller
    {
        IRepository _repository = DependencyResolver.Current.GetService<IRepository>();

        [Route("~/quan-ly/khong-co-quyen-truy-cap", Name = "SharedNoPermission")]
        [ActionName("NoPermission")]
        public ActionResult NoPermission()
        {
            return View();
        }
        [Route("~/quan-ly/loi-he-thong", Name = "ManagementGeneralError")]
        public ActionResult GeneralError()
        {
            ViewBag.Exception = TempData["Exception"];
            return View();
        }
        public List<Category> GetCategories(long? parentCategoryId, int level = 0, bool showPrefix = true)
        {
            List<Category> listCategory = new List<Category>();
            try
            {
                StringBuilder prefix = new StringBuilder();
                for (int i = 0; i < level; i++)
                {
                    prefix.Append("---");
                }
                var categories = _repository.GetRepository<Category>()
                    .GetAll(o => (parentCategoryId == null ? o.CategoryId == null : o.CategoryId == parentCategoryId) && (o.Link == null || o.Link == ""))
                    .OrderBy(o => o.OrdinalNumber).ToList<Category>();

                foreach (var item in categories)
                {
                    listCategory.Add(new Category
                    {
                        Id = item.Id,
                        Name = showPrefix == true ? (prefix.ToString() + " " + item.Name) : item.Name,
                        CategoryId = parentCategoryId
                    });
                    //Nếu có con thì gọi đệ quy
                    if (_repository.GetRepository<Category>().Any(o => o.CategoryId == item.Id))
                    {
                        listCategory.AddRange(GetCategories(item.Id, level + 1, showPrefix));
                    }
                }
                return listCategory;
            }
            catch
            {
                return listCategory;
            }
        }
        public List<CategoryToDropDownGroup> GetCategories2(int level = 0, bool showPrefix = true)
        {
            List<CategoryToDropDownGroup> listCategory = new List<CategoryToDropDownGroup>();
            try
            {
                StringBuilder prefix = new StringBuilder();
                for (int i = 0; i < level; i++)
                {
                    prefix.Append("---");
                }
                var categoryType = _repository.GetRepository<CategoryType>().GetAll().OrderBy(o => o.Name);
                if (categoryType != null && categoryType.Any())
                {
                    foreach (var item in categoryType)
                    {
                        var categories = _repository.GetRepository<Category>().GetAll(o => o.CategoryTypeId == item.Id).OrderBy(o => o.OrdinalNumber).ToList<Category>();
                        if (categories != null && categories.Any())
                        {
                            foreach (var item2 in categories)
                            {
                                listCategory.Add(new CategoryToDropDownGroup
                                {
                                    Id = item2.Id,
                                    Name = showPrefix == true ? (prefix.ToString() + " " + item2.Name) : item2.Name,
                                    CategoryId = null,
                                    CategoryTypeId = item.Id,
                                    CategoryTypeName = item.Name,
                                    OrdinalNumber = item2.OrdinalNumber
                                });
                                if (item2.Categories != null && item2.Categories.Any())
                                {
                                    listCategory.AddRange(GetCategories3(item2.Id, item.Id, item.Name, level + 1, showPrefix));
                                }
                            }
                        }
                    }
                }
                return listCategory;
            }
            catch
            {
                return listCategory;
            }
        }
        public List<CategoryToDropDownGroup> GetCategories3(long? parentCategoryId, long categpryTypeId, string categpryTypeName, int level = 0, bool showPrefix = true)
        {
            List<CategoryToDropDownGroup> listCategory = new List<CategoryToDropDownGroup>();
            try
            {
                StringBuilder prefix = new StringBuilder();
                for (int i = 0; i < level; i++)
                {
                    prefix.Append("---");
                }
                var categories = _repository.GetRepository<Category>()
                    .GetAll(o => (parentCategoryId == null ? o.CategoryId == null : o.CategoryId == parentCategoryId))
                    .OrderBy(o => o.OrdinalNumber).ToList<Category>();

                foreach (var item in categories)
                {
                    listCategory.Add(new CategoryToDropDownGroup
                    {
                        Id = item.Id,
                        Name = showPrefix == true ? (prefix.ToString() + " " + item.Name) : item.Name,
                        CategoryId = parentCategoryId,
                        CategoryTypeId = categpryTypeId,
                        CategoryTypeName = categpryTypeName,
                        OrdinalNumber = item.OrdinalNumber
                    });
                    //Nếu có con thì gọi đệ quy
                    if (item.Categories != null && item.Categories.Any())
                    {
                        listCategory.AddRange(GetCategories3(item.Id, categpryTypeId, categpryTypeName, level + 1, showPrefix));
                    }
                }
                return listCategory;
            }
            catch
            {
                return listCategory;
            }
        }
    }
}