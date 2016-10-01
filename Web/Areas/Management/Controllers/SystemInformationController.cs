using Common;
using Entities.Enums;
using Entities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
using Web.Areas.Management.Helpers;
namespace Web.Areas.Management.Controllers
{
    /// <summary>
    /// Cấu hình các tham số của hệ thống
    /// </summary>
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("tham-so-he-thong")]
    public class SystemInformationController : BaseController
    {
        [Route("", Name = "SystemInformationIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.SystemInformation)]
        public async Task<ActionResult> Index()
        {
            SystemInformation systemInformation = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
            if (systemInformation == null)
            {
                systemInformation = new SystemInformation()
                {
                    Id = 0,
                    SMTPPassword = ""
                };
            }
            //ViewBag.Categories = new SelectList(new SharedController().GetCategories(null), "Id", "Name", string.Empty);
            var categories = new SharedController().GetCategories2();
            ViewBag.Categories = categories.Select(t => new GroupedSelectListItem
            {
                GroupKey = t.CategoryTypeId.ToString(),
                GroupName = t.CategoryTypeName,
                Text = t.Name,
                Value = t.Id.ToString()
            });
            ViewBag.CategoryType = new SelectList(_repository.GetRepository<CategoryType>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            ViewBag.Album = new SelectList(_repository.GetRepository<Album>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            systemInformation.SMTPPassword = Base64Hepler.DecodeFrom64(systemInformation.SMTPPassword);
            return View(systemInformation);
        }
        [Route("")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.SystemInformation)]
        public async Task<ActionResult> Index(SystemInformation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var systemInformation = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                    if (systemInformation == null)
                    {
                        systemInformation = new SystemInformation();
                        systemInformation.SiteName = StringHelper.KillChars(model.SiteName);
                        systemInformation.Slogan = StringHelper.KillChars(model.Slogan);
                        systemInformation.Logo = StringHelper.KillChars(model.Logo);
                        systemInformation.Copyright = StringHelper.KillChars(model.Copyright);
                        systemInformation.Email = StringHelper.KillChars(model.Email);
                        systemInformation.SMTPEmail = StringHelper.KillChars(model.SMTPEmail);
                        systemInformation.SMTPPassword = StringHelper.KillChars(model.SMTPPassword);
                        systemInformation.SMTPName = StringHelper.KillChars(model.SMTPName);
                        int result = await _repository.GetRepository<SystemInformation>().CreateAsync(systemInformation, AccountId);
                    }
                    else
                    {
                        systemInformation.SiteName = StringHelper.KillChars(model.SiteName);
                        systemInformation.Slogan = StringHelper.KillChars(model.Slogan);
                        systemInformation.Logo = StringHelper.KillChars(model.Logo);
                        systemInformation.Copyright = StringHelper.KillChars(model.Copyright);
                        systemInformation.Email = StringHelper.KillChars(model.Email);
                        systemInformation.SMTPEmail = StringHelper.KillChars(model.SMTPEmail);
                        systemInformation.SMTPPassword = StringHelper.KillChars(model.SMTPPassword);
                        systemInformation.SMTPName = StringHelper.KillChars(model.SMTPName);
                        int result = await _repository.GetRepository<SystemInformation>().UpdateAsync(systemInformation, AccountId);
                    }
                    //Session["SystemInformation"] = systemInformation;
                    _cacheFactory.SaveCache("SystemInformation", systemInformation);
                    ViewBag.Success = "Đã ghi nhận thành công!";
                    //ViewBag.Categories = new SelectList(new SharedController().GetCategories(null), "Id", "Name", string.Empty);
                    var categories = new SharedController().GetCategories2();
                    ViewBag.Categories = categories.Select(t => new GroupedSelectListItem
                    {
                        GroupKey = t.CategoryTypeId.ToString(),
                        GroupName = t.CategoryTypeName,
                        Text = t.Name,
                        Value = t.Id.ToString()
                    });
                    ViewBag.CategoryType = new SelectList(_repository.GetRepository<CategoryType>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
                    ViewBag.Album = new SelectList(_repository.GetRepository<Album>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
                    return View(model);
                }
                catch (Exception ex)
                {
                    //ViewBag.Categories = new SelectList(new SharedController().GetCategories(null), "Id", "Name", string.Empty);
                    var categories = new SharedController().GetCategories2();
                    ViewBag.Categories = categories.Select(t => new GroupedSelectListItem
                    {
                        GroupKey = t.CategoryTypeId.ToString(),
                        GroupName = t.CategoryTypeName,
                        Text = t.Name,
                        Value = t.Id.ToString()
                    });
                    ViewBag.CategoryType = new SelectList(_repository.GetRepository<CategoryType>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
                    ViewBag.Album = new SelectList(_repository.GetRepository<Album>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
                    ViewBag.Error = "Đã xảy ra lỗi: " + ex.Message;
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác thông tin!");
                ViewBag.Error = "Vui lòng nhập chính xác thông tin!";
                return View(model);
            }
        }
        [Route("site-map", Name = "SystemInformationSiteMap")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.SystemInformation)]
        public async Task<ActionResult> SiteMap(SystemInformation model)
        {
            try
            {
                var systemInformation = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                if (systemInformation == null)
                {
                    systemInformation = new SystemInformation();
                    systemInformation.MainMenu = model.MainMenu;
                    systemInformation.RightMenu = model.RightMenu;
                    systemInformation.HomeAlbum = model.HomeAlbum;
                    systemInformation.RightAlbum = model.RightAlbum;
                    systemInformation.LeftArticle1 = model.LeftArticle1;
                    systemInformation.LeftArticle2 = model.LeftArticle2;
                    systemInformation.LeftArticle3 = model.LeftArticle3;
                    systemInformation.RightArticle1 = model.RightArticle1;
                    systemInformation.RightArticle2 = model.RightArticle2;
                    systemInformation.RightArticle3 = model.RightArticle3;
                    systemInformation.BottomAlbum = model.BottomAlbum;
                    systemInformation.BottomMenu = model.BottomMenu;
                    int result = await _repository.GetRepository<SystemInformation>().CreateAsync(systemInformation, AccountId);
                }
                else
                {
                    systemInformation.MainMenu = model.MainMenu;
                    systemInformation.RightMenu = model.RightMenu;
                    systemInformation.HomeAlbum = model.HomeAlbum;
                    systemInformation.RightAlbum = model.RightAlbum;
                    systemInformation.LeftArticle1 = model.LeftArticle1;
                    systemInformation.LeftArticle2 = model.LeftArticle2;
                    systemInformation.LeftArticle3 = model.LeftArticle3;
                    systemInformation.RightArticle1 = model.RightArticle1;
                    systemInformation.RightArticle2 = model.RightArticle2;
                    systemInformation.RightArticle3 = model.RightArticle3;
                    systemInformation.BottomAlbum = model.BottomAlbum;
                    systemInformation.BottomMenu = model.BottomMenu;
                    int result = await _repository.GetRepository<SystemInformation>().UpdateAsync(systemInformation, AccountId);
                }
                _cacheFactory.SaveCache("SystemInformation", systemInformation);
                //Session["SystemInformation"] = systemInformation;
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string message = "Đã xảy ra lỗi: " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("smtp", Name = "SystemInformationSMTP")]
        [HttpPost]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.SystemInformation)]
        public async Task<ActionResult> SMTP(SystemInformation model)
        {
            try
            {
                var systemInformation = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                if (systemInformation == null)
                {
                    systemInformation = new SystemInformation();
                    systemInformation.SMTPEmail = model.SMTPEmail;
                    systemInformation.SMTPName = model.SMTPName;
                    systemInformation.SMTPPassword = Base64Hepler.EncodeTo64UTF8(model.SMTPPassword);
                    int result = await _repository.GetRepository<SystemInformation>().CreateAsync(systemInformation, AccountId);
                }
                else
                {
                    systemInformation.SMTPEmail = model.SMTPEmail;
                    systemInformation.SMTPName = model.SMTPName;
                    systemInformation.SMTPPassword = Base64Hepler.EncodeTo64UTF8(model.SMTPPassword);
                    int result = await _repository.GetRepository<SystemInformation>().UpdateAsync(systemInformation, AccountId);
                }
                _cacheFactory.SaveCache("SystemInformation", systemInformation);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string message = "Đã xảy ra lỗi: " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}