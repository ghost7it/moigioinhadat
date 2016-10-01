using Common;
using Entities.Models;
using Entities.ViewModels;
using Interface;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("dang-nhap")]
    public class LoginController : Controller
    {
        public IRepository _repository = DependencyResolver.Current.GetService<IRepository>();
        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "Login")]
        [ActionName("Index")]
        public ActionResult Login()
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            ViewBag.Success = TempData["Success"];
            ViewBag.Forget = TempData["Forget"];
            Session.Abandon();
            return View();
        }
        /// <summary>
        /// Đăng nhập hệ thống (POST)
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string email = StringHelper.KillChars(model.Email);
                    string password = StringHelper.StringToMd5(StringHelper.KillChars(model.Password)).ToLower();
                    password = StringHelper.StringToMd5(password);
                    var account = await _repository.GetRepository<Account>().ReadAsync(o => (o.Email == email && o.Password == password && o.IsManageAccount == true));
                    if (account == null)
                    {
                        //ModelState.AddModelError(string.Empty, "Sai địa chỉ e-mail hoặc mật khẩu!");
                        ViewBag.Error = "Sai địa chỉ e-mail hoặc mật khẩu!";
                        return View(model);
                    }
                    else
                    {
                        if (account.Profile != null && account.Profile.Status != 2)
                        {//Nếu là tài khoản của cựu sinh viên mà chưa được duyệt hoặc chưa phân quyền thì không cho đăng nhập
                            return View(model);
                        }
                        Session["Email"] = account.Email;
                        Session["AccountId"] = account.Id;
                        Session["AccountName"] = account.Name;
                        Session["ProfilePicture"] = account.ProfilePicture;

                        var accountRoles = await _repository.GetRepository<AccountRole>().GetAllAsync(o => o.AccountId == account.Id);
                        var moduleRoles = await _repository.GetRepository<ModuleRole>().GetAllAsync();
                        CacheFactory _cacheFactory = new CacheFactory();
                        _cacheFactory.SaveCache("AccountRoles_" + account.Id, accountRoles.ToList());
                        _cacheFactory.SaveCache("ModuleRoles", moduleRoles.ToList());
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                    ViewBag.Error = "Đã có lỗi xảy ra: " + ex.Message;
                    return View(model);
                }
            }
            else
            {
                //ModelState.AddModelError(string.Empty, "Vui lòng nhập đúng thông tin để đăng nhập!");
                ViewBag.Error = "Vui lòng nhập đúng thông tin để đăng nhập!";
                return View(model);
            }
        }
        /// <summary>
        /// Khởi tạo dữ liệu khi lần đầu tiên chạy trang web
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("~/quan-ly/khoi-tao/{key?}")]
        public async Task<ActionResult> Init(string key)
        {
            if (key != "ghost7it")
            {
                return RedirectToAction("Index", "Login");
            }
            var account = new Account()
            {
                Name = "Administrator",
                Password = StringHelper.StringToMd5(StringHelper.StringToMd5("123456").ToLower()),
                Email = "quochuy7it@gmail.com",
                CreateDate = DateTime.Now,
                IsManageAccount = true,
                IsNormalAccount = false,
                PhoneNumber = "0987.6996.87"
            };
            await _repository.GetRepository<Account>().CreateAsync(account, 0);
            var oganization = new Organization()
            {
                Name = "eBtech Team",
                IsSystemOwner = true,
                IsApproved = true
            };
            await _repository.GetRepository<Organization>().CreateAsync(oganization, 0);
            var role = new Role()
            {
                Name = "Quản trị hệ thống"
            };
            await _repository.GetRepository<Role>().CreateAsync(role, 0);
            var accountRole = new AccountRole()
            {
                AccountId = 1,
                RoleId = 1
            };
            await _repository.GetRepository<AccountRole>().CreateAsync(accountRole, 0);
            var moduleRole = new ModuleRole()
            {
                RoleId = 1,
                ModuleCode = "Account",
                Create = 1,
                Read = 1,
                Update = 1,
                Delete = 1
            };
            await _repository.GetRepository<ModuleRole>().CreateAsync(moduleRole, 0);
            return RedirectToAction("Index", "Login");
        }
        /// <summary>
        /// Yêu cầu mật khẩu mới khi quên mật khẩu
        /// </summary>
        /// <returns></returns>
        [Route("~/quen-mat-khau/")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetPassword()
        {
            TempData["Forget"] = true;
            if (ModelState.IsValid)
            {
                string email = Request.Params["email_forget"];
                var account = await _repository.GetRepository<Account>().ReadAsync(o => o.Email == email);
                if (account == null)
                {
                    TempData["Error"] = "Địa chỉ email không tồn tại!";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    string activeCode = StringHelper.CreateRandomString(32);
                    activeCode = StringHelper.StringToMd5(activeCode).ToLower();
                    var temporaryPassword = StringHelper.CreateRandomString(8);

                    ForgetPassword forgetPassword = new Entities.Models.ForgetPassword();
                    forgetPassword.AccountId = account.Id;
                    forgetPassword.ActiveCode = activeCode;
                    forgetPassword.RequestTime = DateTime.Now;
                    forgetPassword.TemporaryPassword = Base64Hepler.EncodeTo64UTF8(StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(temporaryPassword)).ToLower()));
                    forgetPassword.Status = 0;
                    forgetPassword.RequestIp = CommonHelper.GetIPAddress(Request);
                    int result = await _repository.GetRepository<ForgetPassword>().CreateAsync(forgetPassword, account.Id);
                    var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();

                    if (result > 0 && systemInfo != null)
                    {
                        string domainName = Request.Url.Scheme + "://" + Request.Url.Authority;
                        StringBuilder body = new StringBuilder();
                        body.Append("Kính gửi " + StringHelper.KillChars(account.Name) + ",<br /><br />");
                        body.Append("Quí vị đã yêu cầu khôi phục mật khẩu trên website " + systemInfo.SiteName + "!<br />");
                        body.Append("Mật khẩu mới của quí vị là: " + temporaryPassword);
                        body.Append("<br />Quí vị vui lòng bấm ");
                        body.Append("<a href=\"" + domainName + "/xac-nhan-khoi-phuc-mat-khau/" + activeCode + "\" target=\"_blank\">vào đây</a>");
                        body.Append(" để xác thực việc quên mật khẩu. <br />");
                        body.Append(" Yêu cầu của quí vị chỉ có hiệu lực trong 24 giờ. <br />");
                        body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                        body.Append("<br />Kính thư, <br /><br />");
                        body.Append(systemInfo.SiteName + "<br />");
                        body.Append("Phát triển bởi eBtech Team<br />");
                        body.Append("Webmaster: quochuy7it@gmail.com");

                        bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, email, "Xác nhận khôi phục mật khẩu", body.ToString());
                        if (!result2)
                        {//Gửi email không thành công
                            forgetPassword.Status = 4;
                            int result1 = await _repository.GetRepository<ForgetPassword>().UpdateAsync(forgetPassword, account.Id);
                            TempData["Error"] = "Đã xảy ra lỗi khi thực hiện yêu cầu của bạn! (không gửi được email)";
                            return RedirectToAction("Index", "Login");
                        }
                        else
                        {
                            TempData["Message"] = "Yêu cầu khôi phục mật khẩu của bạn đã được chấp nhận. Vui lòng kiểm tra email và làm theo hướng dẫn.";
                            return RedirectToAction("Index", "Login");
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Lỗi không xác định! Vui lòng thử lại!";
                        return RedirectToAction("Index", "Login");
                    }
                }
            }
            else
            {
                TempData["Error"] = "Lỗi không xác định! Vui lòng thử lại!";
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// Xác nhận yêu cầu mật khẩu mới
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("~/xac-nhan-khoi-phuc-mat-khau/{code?}")]
        public async Task<ActionResult> ConfirmPassword(string code)
        {
            try
            {
                ViewBag.Message = "";
                var forgetPassword = await _repository.GetRepository<ForgetPassword>().ReadAsync(o => o.ActiveCode == code);
                if (forgetPassword == null)
                {
                    ViewBag.Message = "Yêu cầu của bạn không chính xác. Vui lòng kiểm tra lại!";
                    return View();
                }
                else
                {
                    if (forgetPassword.Status != 0)
                    {
                        ViewBag.Message = "Yêu cầu của bạn không chính xác. Vui lòng kiểm tra lại!";
                        return View();
                    }
                    DateTime requestTime = forgetPassword.RequestTime;
                    TimeSpan timespan = DateTime.Now - requestTime;
                    double hours = timespan.TotalHours;
                    if (hours > 24)
                    {
                        ViewBag.Message = "Yêu cầu khôi phục mật khẩu của bạn đã hết thời hạn!";
                        forgetPassword.Status = 3;
                        int result1 = await _repository.GetRepository<ForgetPassword>().UpdateAsync(forgetPassword, forgetPassword.AccountId);
                        return View();
                    }
                    var account = await _repository.GetRepository<Account>().ReadAsync(forgetPassword.AccountId);
                    account.Password = Base64Hepler.DecodeFrom64(forgetPassword.TemporaryPassword);
                    int result2 = await _repository.GetRepository<Account>().UpdateAsync(account, forgetPassword.AccountId);
                    if (result2 > 0)
                    {
                        forgetPassword.Status = 1;
                        forgetPassword.ActiveTime = DateTime.Now;
                        int result1 = await _repository.GetRepository<ForgetPassword>().UpdateAsync(forgetPassword, forgetPassword.AccountId);
                        ViewBag.Message = "Yêu cầu của bạn đã được xử lý thành công. Bạn đã có thể dùng mật khẩu mới để truy cập!";
                        //Có cần gửi email thông báo là đổi mật khẩu thành công hay không?
                        return View();
                    }
                    else
                    {
                        forgetPassword.Status = 2;
                        int result1 = await _repository.GetRepository<ForgetPassword>().UpdateAsync(forgetPassword, forgetPassword.AccountId);
                        ViewBag.Message = "Yêu cầu của bạn đã được xử lý không thành công. Vui lòng thử lại!";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi kích hoạt mật khẩu: " + ex.Message;
                return View();
            }
        }
    }
}