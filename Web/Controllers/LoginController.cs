using Common;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace Web.Controllers
{
    //[RoutePrefix("tai-khoan")]
    public class LoginController : BaseController
    {
        [Route("~/dang-nhap", Name = "FrontEndLoginIndex")]
        public ActionResult Index()
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            ViewBag.Success = TempData["Success"];
            ViewBag.Forget = TempData["Forget"];
            Session.Abandon();
            return View();
        }
        [Route("~/dang-nhap")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string email = StringHelper.KillChars(model.Email);
                    string password = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.Password)).ToLower());
                    //var account = await _repository.GetRepository<Account>().ReadAsync(o => (o.Email == email && o.Password == password && o.IsNormalAccount == true && o.Profile != null));
                    var account = await _repository.GetRepository<Account>().ReadAsync(o => (o.Email == email && o.Password == password));
                    if (account == null)
                    {
                        ModelState.AddModelError(string.Empty, "Sai địa chỉ e-mail hoặc mật khẩu!");
                        //ViewBag.Error = "Sai địa chỉ e-mail hoặc mật khẩu!";
                        return View(model);
                    }
                    else
                    {
                        if (account.IsManageAccount && account.Profile == null)
                        {
                            Session["Email"] = account.Email;
                            Session["AccountId"] = account.Id;
                            Session["AccountName"] = account.Name;
                            Session["ProfilePicture"] = account.ProfilePicture;

                            var accountRoles = await _repository.GetRepository<AccountRole>().GetAllAsync(o => o.AccountId == account.Id);
                            var moduleRoles = await _repository.GetRepository<ModuleRole>().GetAllAsync();
                            CacheFactory _cacheFactory = new CacheFactory();
                            _cacheFactory.SaveCache("AccountRoles_" + account.Id, accountRoles.ToList());
                            _cacheFactory.SaveCache("ModuleRoles", moduleRoles.ToList());

                            return RedirectToRoute("ManagementHome");
                        }
                        if (account.Profile != null && account.Profile.Status != 2)
                        {//Nếu là tài khoản của cựu sinh viên mà chưa được duyệt hoặc chưa phân quyền thì không cho đăng nhập
                            ModelState.AddModelError(string.Empty, "Sai địa chỉ e-mail hoặc mật khẩu!");
                            return View(model);
                        }
                        Session["Email"] = account.Email;
                        Session["AccountId"] = account.Id;
                        Session["AccountName"] = account.Name;
                        Session["ProfilePicture"] = account.ProfilePicture;
                        Session["ProfileId"] = account.Profile.Id;
                        return RedirectToRoute("FrontEndProfileDetail");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                    //ViewBag.Error = "Đã có lỗi xảy ra: " + ex.Message;
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đúng thông tin để đăng nhập!");
                //ViewBag.Error = "Vui lòng nhập đúng thông tin để đăng nhập!";
                return View(model);
            }
        }
        [Route("~/dang-xuat", Name = "FrontEndLoginLogout")]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToRoute("FrontEndHomeIndex");
        }
        [Route("~/quen-mat-khau-dang-nhap", Name = "FrontEndLoginForgetPassword")]
        public ActionResult ForgetPassword()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }
        [CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật không chính xác!")]
        [Route("~/quen-mat-khau-dang-nhap/")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = StringHelper.KillChars(model.Email);
                var account = await _repository.GetRepository<Account>().ReadAsync(o => o.Email == email);
                if (account == null)
                {
                    ModelState.AddModelError(string.Empty, "Địa chỉ email không tồn tại!");
                    return View(model);
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
                            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi thực hiện yêu cầu của bạn! (không gửi được email)");
                            return View(model);
                        }
                        else
                        {
                            TempData["Success"] = true;
                            return RedirectToRoute("FrontEndLoginForgetPassword");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ các thông tin!");
                        return View(model);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ các thông tin!");
                return View(model);
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