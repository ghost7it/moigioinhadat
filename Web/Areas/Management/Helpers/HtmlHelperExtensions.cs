using Entities.Models;
using System.Collections.Generic;
using System.Linq;
namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string CategoryTree(this HtmlHelper html, IEnumerable<Category> categories, List<long> articleCategoryIds)
        {
            string htmlOutput = string.Empty;
            //Vì đây là lấy chuyên mục để gán cho bài viết, nên các chuyên mục dạng link không được phép hiển thị
            //Update: Không lọc nữa vì nếu lọc thì danh mục con cũng biến mất
            //categories = categories.Where(o => o.Link == null || o.Link == "");            
            if (categories != null && categories.Any())
            {
                htmlOutput += "<ul class=\"list-unstyled\">";
                foreach (Category category in categories.OrderBy(o => o.OrdinalNumber))
                {
                    htmlOutput += "<li>";
                    if (articleCategoryIds != null && articleCategoryIds.Any() && articleCategoryIds.Contains(category.Id))
                    {
                        htmlOutput += "<label><input type=\"checkbox\" name=\"CategoryId\" value=\"" + category.Id + "\" checked=\"checked\" />&nbsp; " + category.Name + "</label>";
                    }
                    else
                    {
                        htmlOutput += "<label><input type=\"checkbox\" name=\"CategoryId\" value=\"" + category.Id + "\" />&nbsp; " + category.Name + "</label>";
                    }
                    if (category.Categories != null && category.Categories.Any())
                        htmlOutput += html.CategoryTree(category.Categories, articleCategoryIds);
                    htmlOutput += "</li>";
                }
                htmlOutput += "</ul>";
            }
            return htmlOutput;
        }
        public static string CategoryTypeTree(this HtmlHelper html, IEnumerable<CategoryType> categoryTypes, List<long> articleCategoryIds)
        {
            string htmlOutput = string.Empty;
            if (categoryTypes != null && categoryTypes.Any())
            {

                foreach (CategoryType categoryType in categoryTypes)
                {
                    htmlOutput += "<div class=\"note note-success\" style=\"padding: 0 0 0 10px;\">";
                    htmlOutput += "<h4 class=\"block\">" + categoryType.Name + "</h4></div>";
                    if (categoryType.Categories != null && categoryType.Categories.Any())
                        htmlOutput += html.CategoryTree(categoryType.Categories, articleCategoryIds);

                }

            }
            return htmlOutput;
        }
        public static string CategoryTreeForCreateLink(this HtmlHelper html, IEnumerable<Category> categories, long? categoryId)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string htmlOutput = string.Empty;
            categories = categories.Where(o => o.Id != categoryId);
            if (categories != null && categories.Any())
            {
                htmlOutput += "<ul class=\"list-unstyled\">";
                foreach (Category category in categories.OrderBy(o => o.OrdinalNumber))
                {
                    string link = url.RouteUrl("FrontEndArticleIndex", new { categoryName = Common.StringHelper.StringFilter(category.Name), categoryId = category.Id }).ToString();
                    htmlOutput += "<li style=\"margin:3px 0; width:100%;display: inline-block;\">";
                    htmlOutput += category.Name + "<a class=\"btn green btn-sm create-link pull-right\" href=\"#\" data-link=\"" + link + "\"><i class=\"fa fa-link\"></i> Chọn</a>";


                    if (category.Categories != null && category.Categories.Any())
                        htmlOutput += html.CategoryTreeForCreateLink(category.Categories, categoryId);
                    htmlOutput += "</li>";
                }
                htmlOutput += "</ul>";
            }
            return htmlOutput;
        }
        public static string CategoryTypeTreeForCreateLink(this HtmlHelper html, IEnumerable<CategoryType> categoryTypes, long? categoryId)
        {
            string htmlOutput = string.Empty;
            if (categoryTypes != null && categoryTypes.Any())
            {

                foreach (CategoryType categoryType in categoryTypes)
                {
                    htmlOutput += "<div class=\"note note-success\" style=\"padding: 0 0 0 10px;\">";
                    htmlOutput += "<h4 class=\"block\">" + categoryType.Name + "</h4></div>";
                    if (categoryType.Categories != null && categoryType.Categories.Any())
                        htmlOutput += html.CategoryTreeForCreateLink(categoryType.Categories, categoryId);

                }

            }
            return htmlOutput;
        }
    }
}