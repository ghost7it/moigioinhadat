using Common;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
using Web.Areas.Management.Helpers;
using Web.Helpers;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("chuyen-muc")]
    public class CategoryController : BaseController
    {
        [Route("quan-ly-chuyen-muc", Name = "CategoryIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Category)]
        public async Task<ActionResult> Index()
        {
            var categoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
            ViewBag.CategoryTypes = categoryTypes.ToList().ToSelectList();
            ViewBag.Categories = new SelectList(new SharedController().GetCategories(null), "Id", "Name", string.Empty);
            return View();
        }
        [Route("them-moi-cap-nhat-chuyen-muc", Name = "CategoryCreateOrUpdate")]
        public PartialViewResult CreateOrUpdate(long id)
        {
            Category category = new Category();
            List<CategoryToDropDownGroup> categories = new SharedController().GetCategories2();

            if (id != 0)
            {
                category = _repository.GetRepository<Category>().Read(id);
                categories = categories.Where(o => (!GetChildCategoryIds(id).Contains(o.Id))).ToList();
            }

            ViewBag.Categories = categories.Select(t => new GroupedSelectListItem
            {
                GroupKey = t.CategoryTypeId.ToString(),
                GroupName = t.CategoryTypeName,
                Text = t.Name,
                Value = t.Id.ToString(),
                Selected = category != null && category.CategoryId == t.Id
            });
            var categoryTypes = _repository.GetRepository<CategoryType>().GetAll();
            ViewBag.CategoryTypes = categoryTypes.ToList().ToSelectList();
            return PartialView("_CreateOrUpdate", category);
        }
        public List<long> GetChildCategoryIds(long parentCategoryId)
        {
            List<long> childIds = new List<long>();
            childIds.Add(parentCategoryId);
            var donVis = _repository.GetRepository<Category>().GetAll(o => o.CategoryId == parentCategoryId);
            if (donVis.Any())
            {
                foreach (var item in donVis)
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

        [Route("nhap-chuyen-muc", Name = "CategoryCreate")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Category)]
        public async Task<ActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!model.CategoryId.HasValue && !model.CategoryTypeId.HasValue)
                    {
                        return Json(new { success = false, message = "Vui lòng chọn chuyên mục cha hoặc loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
                    }
                    //Nhập chuyên mục
                    Category category = new Category();
                    category.Name = StringHelper.KillChars(model.Name);
                    category.Description = StringHelper.KillChars(model.Description);
                    category.OrdinalNumber = model.OrdinalNumber;
                    category.Link = StringHelper.KillChars(model.Link);
                    category.CategoryId = model.CategoryId;
                    if (!model.CategoryId.HasValue)
                    {
                        category.CategoryTypeId = model.CategoryTypeId;
                        category.DisplayType = model.DisplayType.HasValue ? model.DisplayType.Value : (byte)1;
                    }
                    category.DescriptionIcon = StringHelper.KillChars(model.DescriptionIcon);
                    category.SmallIcon = StringHelper.KillChars(model.SmallIcon); ;

                    int updateResult = await _repository.GetRepository<Category>().CreateAsync(category, AccountId);
                    if (updateResult > 0)
                        return Json(new { success = true, CategoryId = model.CategoryId, CategoryTypeId = model.CategoryTypeId, message = "Nhập chuyên mục thành công!" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { success = false, message = "Nhập chuyên mục không thành công!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng nhập chính xác các thông tin!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("cap-nhat-chuyen-muc", Name = "CategoryUpdate")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Category)]
        public async Task<ActionResult> Update(long id, Category model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!model.CategoryId.HasValue && !model.CategoryTypeId.HasValue)
                    {
                        return Json(new { success = false, message = "Vui lòng chọn chuyên mục cha hoặc loại chuyên mục!" }, JsonRequestBehavior.AllowGet);
                    }
                    //Cập nhật chuyên mục
                    Category category = await _repository.GetRepository<Category>().ReadAsync(id);
                    if (category == null)
                        return Json(new { success = false, message = "Không tìm thấy chuyên mục!" }, JsonRequestBehavior.AllowGet);
                    category.Name = StringHelper.KillChars(model.Name);
                    category.Description = StringHelper.KillChars(model.Description);
                    category.OrdinalNumber = model.OrdinalNumber;
                    category.Link = StringHelper.KillChars(model.Link);
                    category.CategoryId = model.CategoryId;
                    if (!model.CategoryId.HasValue)
                    {
                        category.CategoryTypeId = model.CategoryTypeId;
                        category.DisplayType = model.DisplayType.HasValue ? model.DisplayType.Value : (byte)1;
                    }
                    else
                    {
                        category.CategoryTypeId = null;
                        category.DisplayType = (byte)1;
                    }
                    category.DescriptionIcon = StringHelper.KillChars(model.DescriptionIcon);
                    category.SmallIcon = StringHelper.KillChars(model.SmallIcon);

                    int updateResult = await _repository.GetRepository<Category>().UpdateAsync(category, AccountId);
                    if (updateResult > 0)
                        return Json(new { success = true, CategoryId = model.CategoryId, CategoryTypeId = model.CategoryTypeId, NewName = category.Name, message = "Nhập chuyên mục thành công!" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { success = false, message = "Cập nhật chuyên mục không thành công!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng nhập chính xác các thông tin!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("xoa-chuyen-muc/{id?}", Name = "CategoryDelete")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Category)]
        public async Task<JsonResult> Delete(long id)
        {
            try
            {
                Category category = await _repository.GetRepository<Category>().ReadAsync(id);
                if (category != null)
                {
                    if (category.Categories != null && category.Categories.Any())
                    {
                        //Nếu loại chuyên mục đang có các chuyên mục con thì không cho xóa
                        return Json(new { success = false, message = "Bạn phải xóa các chuyên mục con trước!" });
                    }
                    if (category.ArticleCategories != null && category.ArticleCategories.Any())
                    {
                        //Nếu loại chuyên mục đang có các bài viết thì không cho xóa
                        //????Có nên cập nhật categoryId của các bài viết thành null rồi cho phép xóa hay không?
                        return Json(new { success = false, message = "Bạn phải xóa các bài viết trước!" });
                    }
                    int result = await _repository.GetRepository<Category>().DeleteAsync(category, AccountId);
                    if (result > 0)
                    {
                        return Json(new { success = true, message = "Xóa chuyên mục thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Lỗi: Không xóa được chuyên mục!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy chuyên mục!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("danh-sach-loai-chuyen-muc-json", Name = "CategoryGetCategoryTypeJson")]
        public async Task<ActionResult> GetCategoryTypeJson(string parent, string type)
        {
            var categoryTypes = await _repository.GetRepository<CategoryType>().GetAllAsync();
            var json = categoryTypes.OrderBy(o => o.Name).Select(o => new
            {
                id = "nodetype_" + o.Id,
                text = o.Name,
                icon = "fa fa-folder icon-lg icon-state-success",
                state = new
                {
                    opened = false,
                    disabled = true,
                    selected = false
                },
                children = o.Categories != null && o.Categories.Any() ? true : false,
            }).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [Route("danh-sach-chuyen-muc-json", Name = "CategoryGetCategoryJson")]
        public async Task<JsonResult> GetCategoryJson(string parent)
        {
            var tmp = parent.Split('_');
            long id = Convert.ToInt64(tmp[1]);
            if (tmp[0] == "nodetype")
            {
                var categories = await _repository.GetRepository<Category>().GetAllAsync(o => o.CategoryTypeId == id);
                var json = categories.OrderBy(o => o.OrdinalNumber).Select(o => new
                {
                    id = "node_" + o.Id,
                    parent = parent,
                    text = o.Name,
                    icon = "fa fa-folder icon-lg icon-state-success",
                    state = new
                    {
                        opened = false,
                        disabled = false,
                        selected = false
                    },
                    children = o.Categories != null && o.Categories.Any() ? true : false,
                }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var categories = await _repository.GetRepository<Category>().GetAllAsync(o => o.CategoryId == id);
                var json = categories.OrderBy(o => o.OrdinalNumber).Select(o => new
                {
                    id = "node_" + o.Id,
                    parent = parent,
                    text = o.Name,
                    icon = "fa fa-folder icon-lg icon-state-success",
                    state = new
                    {
                        opened = false,
                        disabled = false,
                        selected = false
                    },
                    children = o.Categories != null && o.Categories.Any() ? true : false,
                }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("danh-sach-bai-viet-tao-lien-ket/{p?}/{key?}", Name = "CategoryGetArticleToCreateLink")]
        public ActionResult GetArticleToCreateLink(int p = 1, string key = "")
        {
            int take = 13;
            int skip = (p - 1) * take;
            Paging paging = new Paging()
            {
                TotalRecord = 0,
                Skip = skip,
                Take = take,
                OrderDirection = "desc"
            };
            string orderKey = "PublishDate";
            byte status = 4;
            //var articles = await _repository.GetRepository<Article>().GetAllAsync(o => o.Status == 4);
            //articles = articles.OrderByDescending(o => o.PublishDate);
            var articles = _repository.GetRepository<Article>().GetAll(ref paging, orderKey, o => (key == null || key == "" || o.Title.Contains(key) || o.Description.Contains(key) || o.Content.Contains(key)) && (o.Status == status)).ToList();

            ViewBag.PagingInfo = new PagingInfo
            {
                CurrentPage = p,
                ItemsPerPage = take,
                TotalItems = paging.TotalRecord
            };
            ViewBag.Key = key;
            return PartialView("_GetArticleToCreateLink", articles);
        }
        [Route("danh-sach-chuyen-muc-tao-lien-ket/{categoryId?}", Name = "CategoryGetCategoryToCreateLink")]
        public ActionResult GetCategoryToCreateLink(long? categoryId)
        {
            ViewBag.CategoryTypes = _repository.GetRepository<CategoryType>().GetAll();
            ViewBag.CategoryId = categoryId;
            return PartialView("_GetCategoryToCreateLink");
        }
    }
}