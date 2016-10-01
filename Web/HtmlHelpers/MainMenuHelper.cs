using Common;
using Entities.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace System.Web.Mvc
{
    public static class MainMenuHelper
    {
        public static string RenderMainMenu(this HtmlHelper html, IEnumerable<Category> categories)
        {
            StringBuilder result = new StringBuilder();
            UrlHelper Url = new UrlHelper(html.ViewContext.RequestContext);
            if (categories != null && categories.Any())
            {
                result.Append("<ul>");
                foreach (Category category in categories.OrderBy(o => o.OrdinalNumber))
                {
                    result.Append("<li id=\"category_menu_" + category.Id + "\">");
                    if (string.IsNullOrEmpty(category.Link))
                    {
                        result.Append("<a href=\"" + Url.RouteUrl("FrontEndArticleIndex", new { categoryName = StringHelper.StringFilter(category.Name), categoryId = category.Id }) + "\">&nbsp; " + category.Name + "</a>");
                    }
                    else
                    {
                        result.Append("<a href=\"" + category.Link + "\">&nbsp; " + category.Name + "</a>");
                    }
                    if (category.Categories != null && category.Categories.Any())
                        result.Append(html.RenderMainMenu(category.Categories));
                    result.Append("</li>");
                }
                result.Append("</ul>");
            }
            return result.ToString();
        }
    }
}