using Common;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("tai-khoan")]
    public class AccountController : BaseController
    {
        /// <summary>
        /// Danh sách tài khoản
        /// Dữ liệu được lấy bằng ajax theo hàm: GetAccountsJson 
        /// </summary>
        /// <returns></returns>
        [Route("danh-sach-tai-khoan", Name = "AccountIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Account)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Nhập tài khoản hệ thống mới
        /// </summary>
        /// <returns></returns>
        [Route("nhap-tai-khoan-moi", Name = "AccountCreate")]
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Nhập tài khoản hệ thống mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("nhap-tai-khoan-moi")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Account)]
        public async Task<ActionResult> Create(Account model)
        {
            if (ModelState.IsValid)
            {
                string email = StringHelper.KillChars(model.Email);
                //Kiểm tra trùng email
                bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == email);
                bool exists2 = await _repository.GetRepository<Account>().AnyAsync(o => o.Profile != null && o.Profile.Email == email);
                if (exists || exists2)
                {
                    ModelState.AddModelError(string.Empty, "Địa chỉ email đã tồn tại!");
                    ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng cho tài khoản khác!");
                    return View(model);
                }
                else
                {
                    string password = StringHelper.StringToMd5(StringHelper.KillChars(model.Password)).ToLower();
                    Account account = new Account();
                    account.IsManageAccount = true;
                    account.IsNormalAccount = false;
                    account.Email = email;
                    account.Name = StringHelper.KillChars(model.Name);
                    account.PhoneNumber = StringHelper.KillChars(model.PhoneNumber);
                    account.ProfilePicture = StringHelper.KillChars(model.ProfilePicture); ;
                    account.CreateDate = DateTime.Now;
                    account.Password = StringHelper.StringToMd5(password);
                    int result = await _repository.GetRepository<Account>().CreateAsync(account, AccountId);
                    if (result > 0)
                    {
                        TempData["Success"] = "Nhập tài khoản mới thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Nhập tài khoản mới không thành công! Vui lòng kiểm tra và thử lại!");
                        return View(model);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
                return View(model);
            }
        }
        /// <summary>
        /// Chi tiết thông tin tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Account)]
        [Route("cap-nhat-tai-khoan/{id?}", Name = "AccountRead")]
        public async Task<ActionResult> Read(long id)
        {
            ViewBag.Roles = await _repository.GetRepository<Role>().GetAllAsync();
            ViewBag.AccountRoles = await _repository.GetRepository<AccountRole>().GetAllAsync(o => o.AccountId == id);
            ViewBag.Organizations = await _repository.GetRepository<Organization>().GetAllAsync(o => o.ParentOrganization.IsSystemOwner == true);
            ViewBag.AccountOrganizations = await _repository.GetRepository<AccountOrganization>().GetAllAsync(o => o.AccountId == id);
            Account account = await _repository.GetRepository<Account>().ReadAsync(id);
            return View(account);
        }
        /// <summary>
        /// Cập nhật thông tin tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("cap-nhat-tai-khoan/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Account)]
        public async Task<ActionResult> Read(long id, AccountUpdatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string email = StringHelper.KillChars(model.Email);
                    //Kiểm tra trùng email
                    bool result = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == email && o.Id != id);
                    if (result)
                        return Json(new { success = false, message = "Địa chi email đã được sử dụng. Vui lòng nhập địa chỉ email khác!" }, JsonRequestBehavior.AllowGet);
                    Account account = await _repository.GetRepository<Account>().ReadAsync(id);
                    if (account == null)
                        return Json(new { success = false, message = "Không tìm thấy tài khoản người dùng!" }, JsonRequestBehavior.AllowGet);
                    //Cập nhật thông tin
                    account.Name = StringHelper.KillChars(model.Name);
                    account.Email = email;
                    account.PhoneNumber = StringHelper.KillChars(model.PhoneNumber);
                    account.ProfilePicture = StringHelper.KillChars(model.ProfilePicture); ;
                    int updateResult = await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                    if (updateResult > 0)
                    {
                        if (id == AccountId)
                        {
                            Session["Email"] = account.Email;
                            Session["AccountId"] = account.Id;
                            Session["AccountName"] = account.Name;
                            Session["ProfilePicture"] = account.ProfilePicture;
                        }
                        return Json(new { success = true, message = "Cập nhật thông tin tài khoản thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { success = false, message = "Cập nhật thông tin tài khoản không thành công!" }, JsonRequestBehavior.AllowGet);
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
        [Route("doi-mat-khau", Name = "AccountChangePassword")]
        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }
        //Đổi mật khẩu của người đang đăng nhập
        [Route("doi-mat-khau", Name = "AccountChangePasswordPost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    long accountId = Convert.ToInt64(Session["AccountId"].ToString());
                    Account account = await _repository.GetRepository<Account>().ReadAsync(accountId);
                    if (account == null)
                        return Json(new { success = false, message = "Không tìm thấy tài khoản người dùng!" }, JsonRequestBehavior.AllowGet);
                    string password = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.OldPassword)).ToLower());
                    string newPassword = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.NewPassword)).ToLower());
                    //Kiểm tra mật khẩu cũ
                    if (password == account.Password)
                    {
                        account.Password = newPassword;
                        int result = await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                        if (result > 0)
                            return Json(new { success = true, message = "Đổi mật khẩu thành công!" }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { success = false, message = "Đổi mật khẩu không thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Mật khẩu hiện tại không chính xác, vui lòng kiểm tra lại!" }, JsonRequestBehavior.AllowGet);
                    }
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
        /// <summary>
        /// Đổi mật khẩu cho thành viên
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("doi-mat-khau-tai-khoan/{id?}", Name = "AccountChangeMemberPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Account)]
        public async Task<ActionResult> ChangeMemberPassword(long id, ChangeMemberPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Account account = await _repository.GetRepository<Account>().ReadAsync(id);
                    if (account == null)
                        return Json(new { success = false, message = "Không tìm thấy tài khoản người dùng!" }, JsonRequestBehavior.AllowGet);
                    string password = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.ConfirmPassword)).ToLower());
                    string newPassword = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.NewPassword)).ToLower());
                    //Kiểm tra mật khẩu có đồng nhất hay không
                    if (password == newPassword)
                    {
                        account.Password = newPassword;
                        int result = await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                        if (result > 0)
                            return Json(new { success = true, message = "Đổi mật khẩu thành công!" }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { success = false, message = "Đổi mật khẩu không thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Mật khẩu không khớp, vui lòng kiểm tra lại!" }, JsonRequestBehavior.AllowGet);
                    }
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
        /// <summary>
        /// Xóa tài khoản người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Account)]
        [Route("xoa-tai-khoan/{id?}", Name = "AccountDelete")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                if (id == AccountId || id == 1)
                {
                    TempData["Error"] = "Xóa tài khoản không thành công!";
                    return RedirectToAction("Index");
                }
                Account account = await _repository.GetRepository<Account>().ReadAsync(id);
                if (account != null)
                {
                    int result1 = await _repository.GetRepository<AccountRole>().DeleteAsync(o => o.AccountId == id, AccountId);
                    int result2 = await _repository.GetRepository<AccountOrganization>().DeleteAsync(o => o.AccountId == id, AccountId);
                    int result3 = await _repository.GetRepository<RespondFeedback>().DeleteAsync(o => o.AccountId == id, AccountId);
                    if (account.Profile != null)
                    {
                        var profile = await _repository.GetRepository<Profile>().ReadAsync(o => o.Id == account.Profile.Id);
                        profile.Account = null;
                        await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                    }
                    int result = await _repository.GetRepository<Account>().DeleteAsync(account, AccountId);

                    if (result > 0)
                        TempData["Success"] = "Xóa tài khoản thành công!";
                    else
                        TempData["Error"] = "Xóa tài khoản không thành công!";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Xóa tài khoản không thành công! Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// Xóa tài khoản người dùng
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Account)]
        [HttpPost]
        [Route("xoa-nhieu-tai-khoan/{ids?}", Name = "AccountDeleteAccounts")]
        public async Task<ActionResult> DeleteAccounts(string ids)
        {
            try
            {
                byte succeed = 0;
                string[] accountIds = Regex.Split(ids, ",");
                if (accountIds != null && accountIds.Any())
                    foreach (var item in accountIds)
                    {
                        long accountId = 0;
                        long.TryParse(item, out accountId);
                        bool result = await DeleteAccount(accountId);
                        if (result)
                            succeed++;
                    }
                return Json(new { success = true, message = string.Format(@"Đã xóa thành công {0} tài khoản.", succeed) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không xóa được tài khoản. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Xóa tài khoản người dùng
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private async Task<bool> DeleteAccount(long accountId)
        {
            if (accountId == AccountId || accountId == 1)
                return false;
            var account = await _repository.GetRepository<Account>().ReadAsync(accountId);
            if (account == null)
                return false;
            int result1 = await _repository.GetRepository<AccountRole>().DeleteAsync(o => o.AccountId == accountId, AccountId);
            int result2 = await _repository.GetRepository<AccountOrganization>().DeleteAsync(o => o.AccountId == accountId, AccountId);
            int result3 = await _repository.GetRepository<RespondFeedback>().DeleteAsync(o => o.AccountId == accountId, AccountId);
            if (account.Profile != null)
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(o => o.Id == account.Profile.Id);
                profile.Account = null;
                await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
            }
            int result = await _repository.GetRepository<Account>().DeleteAsync(account, AccountId);
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Phân quyền sử dụng chức năng
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Account)]
        [Route("phan-quyen-cho-tai-khoan", Name = "AccountMappingRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AccountMappingRole(long accountId, string roles)
        {
            try
            {
                string[] rolesChecked = Regex.Split(roles, ",");
                var accountRoles = rolesChecked.Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => new AccountRole()
                    {
                        RoleId = Convert.ToInt64(o),
                        AccountId = accountId
                    }).ToList();
                try
                {
                    if (accountRoles.Any())
                    {
                        var accountRolesMapped = await _repository.GetRepository<AccountRole>().GetAllAsync(o => o.AccountId == accountId);
                        var toMap = accountRoles.Where(p => !accountRolesMapped.Any(p2 => p2.RoleId == p.RoleId));
                        var toUnMap = accountRolesMapped.Where(p => !accountRoles.Any(p2 => p2.RoleId == p.RoleId));
                        int result2 = await _repository.GetRepository<AccountRole>().DeleteAsync(toUnMap, AccountId);
                        int result = await _repository.GetRepository<AccountRole>().CreateAsync(toMap, AccountId);
                    }
                    else
                    {
                        int result = await _repository.GetRepository<AccountRole>().DeleteAsync(o => o.AccountId == accountId, AccountId);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Phân quyền không thành công. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
                var accountRoles2 = _repository.GetRepository<AccountRole>().GetAll(o => o.AccountId == AccountId).ToList();
                var moduleRoles = _repository.GetRepository<ModuleRole>().GetAll().ToList();
                CacheFactory _cacheFactory = new CacheFactory();
                _cacheFactory.SaveCache("AccountRoles_" + AccountId, accountRoles2);
                _cacheFactory.SaveCache("ModuleRoles", moduleRoles);

                return Json(new { success = true, message = "Đã phân quyền thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Phân quyền không thành công. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Phân quyền duyệt hồ sơ theo đơn vị
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="organizations"></param>
        /// <returns></returns>
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Account)]
        [Route("phan-quyen-duyet-ho-so-cho-tai-khoan/{accountId?}/{organizations?}", Name = "AccountOrganization")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AccountOrganization(long accountId, string organizations)
        {
            try
            {
                string[] organizationsChecked = Regex.Split(organizations, ",");
                var accountOrganizations = organizationsChecked.Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => new AccountOrganization()
                {
                    OrganizationId = Convert.ToInt64(o),
                    AccountId = accountId
                }).ToList();
                try
                {
                    if (accountOrganizations.Any())
                    {
                        var accountOrganizationsMapped = await _repository.GetRepository<AccountOrganization>().GetAllAsync(o => o.AccountId == accountId);
                        var accountOrganizations1 = accountOrganizations.Where(p => !accountOrganizationsMapped.Any(p2 => p2.OrganizationId == p.OrganizationId));
                        int result = await _repository.GetRepository<AccountOrganization>().CreateAsync(accountOrganizations1, AccountId);
                    }
                    else
                    {
                        int result = await _repository.GetRepository<AccountOrganization>().DeleteAsync(o => o.AccountId == accountId, AccountId);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Phân quyền không thành công. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, message = "Đã phân quyền thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Phân quyền không thành công. Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("danh-sach-tai-khoan-json/{status?}", Name = "AccountGetAccountsJson")]
        public ActionResult GetAccountsJson(byte status)
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
            orderKey = string.IsNullOrEmpty(orderKey) ? "CreateDate" : orderKey;

            if (!string.IsNullOrEmpty(start))
                skip = Convert.ToInt16(start);
            if (!string.IsNullOrEmpty(length))
                take = Convert.ToInt16(length);
            if (!string.IsNullOrEmpty(draw))
                drawReturn = draw;
            string objectStatus = Request.Params["objectStatus"];//Lọc trạng thái 
            if (!string.IsNullOrEmpty(objectStatus))
                byte.TryParse(objectStatus.ToString(), out status);
            Paging paging = new Paging()
            {
                TotalRecord = 0,
                Skip = skip,
                Take = take,
                OrderDirection = orderDir
            };
            var accounts = _repository.GetRepository<Account>().GetAll(ref paging, orderKey, o => (
               key == null ||
               key == "" ||
               o.Email.Contains(key) ||
               o.Name.Contains(key) ||
               o.PhoneNumber.Contains(key))
               ).ToList();
            if (status == 1)
            {
                accounts = accounts.Where(o => o.Profile == null).ToList();
            }
            if (status == 2)
            {
                accounts = accounts.Where(o => o.Profile != null).ToList();
            }

            return Json(new
            {
                draw = drawReturn,
                recordsTotal = paging.TotalRecord,
                recordsFiltered = paging.TotalRecord,
                data = accounts.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Email,
                    o.PhoneNumber,
                    CreateDate = o.CreateDate.ToString("dd/MM/yyyy")
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}