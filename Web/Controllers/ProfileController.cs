using Common;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Web.Controllers
{
    //[RoutePrefix("ho-so")]
    public class ProfileController : BaseController
    {
        private string _profilePicturePath
        {
            get { return Server.MapPath(ConfigurationManager.AppSettings["ProfilePicturePath"].ToString()); }
        }
        [Route("~/dang-ky-ho-so", Name = "FrontEndProfileRegister")]
        public ActionResult Register()
        {
            ViewBag.Success = TempData["Success"];
            ViewBag.Organizations = new SelectList(new SharedController().GetOrganizations(null, true).ToList(), "Id", "Name", string.Empty);
            return View();
        }
        [CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật không chính xác!")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/dang-ky-ho-so")]
        public async Task<ActionResult> Register(RegisterProfileViewModel model)
        {
            ViewBag.Organizations = new SelectList(new SharedController().GetOrganizations(null, true).ToList(), "Id", "Name", string.Empty);
            if (ModelState.IsValid)
            {
                try
                {
                    string email = StringHelper.KillChars(Request.Form["Email"]);
                    string name = StringHelper.KillChars(Request.Form["Name"]);
                    //Kiểm tra chưa nhập họ tên và email
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                    {
                        return View(model);
                    }
                    //Kiểm tra trùng email
                    bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == email);
                    bool exists2 = await _repository.GetRepository<Profile>().AnyAsync(o => o.Email == email);
                    if (exists || exists2)
                    {
                        ModelState.AddModelError(string.Empty, "Địa chỉ email đã tồn tại!");
                        ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng cho tài khoản khác!");
                        return View(model);
                    }
                    //Kiểm tra chưa chọn trường
                    string[] organizations = null;
                    if (Request.Form["OrganizationId"] != null)
                        organizations = Request.Form["OrganizationId"].Split(',');
                    if (organizations == null || organizations.Length == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Vui lòng chọn trường!");
                        return View(model);
                    }

                    string dob = StringHelper.KillChars(Request.Form["DateOfBirth"]);
                    string sex = StringHelper.KillChars(Request.Form["Sex"]);
                    string cellphone = StringHelper.KillChars(Request.Form["CellPhone"]);
                    string nativeland = StringHelper.KillChars(Request.Form["NativeLand"]);
                    string currentresidence = StringHelper.KillChars(Request.Form["CurrentResidence"]);
                    string extrainfomation = StringHelper.KillChars(Request.Form["ExtraInfomation"]);


                    //string[] amounts = Request.Form.GetValues("Amount");
                    //string[] amounts = Request.Form["Amount"].Split(new char[] { ',' });

                    Profile profile = new Profile();
                    profile.Name = name;
                    profile.Email = email;
                    profile.Sex = string.IsNullOrEmpty(sex) ? false : Convert.ToBoolean(sex);
                    profile.CellPhone = cellphone;
                    profile.NativeLand = nativeland;
                    profile.CurrentResidence = currentresidence;
                    profile.ExtraInfomation = extrainfomation;

                    profile.Status = 1;
                    profile.CreateDate = DateTime.Now;
                    profile.CreateBy = 0;

                    if (!string.IsNullOrEmpty(dob))
                    {
                        try
                        {
                            DateTime date = DateTime.ParseExact(dob, "dd/MM/yyyy", null);
                            profile.DateOfBirth = date;
                        }
                        catch
                        {
                            profile.DateOfBirth = null;
                        }
                    }
                    int result = await _repository.GetRepository<Profile>().CreateAsync(profile, 0);
                    if (result > 0)
                    {
                        string[] startyears = Request.Form.GetValues("StartYear");
                        string[] graduateyears = Request.Form.GetValues("GraduateYear");
                        string[] majorsids = Request.Form.GetValues("MajorsId");
                        string[] courseids = Request.Form.GetValues("CourseId");

                        var profileOrganizations = new List<ProfileOrganization>();
                        for (int i = 0; i < organizations.Length; i++)
                        //Response.Write(string.Format("Value of Item{0} is {1}", i, organizations[i]));
                        {
                            if (!string.IsNullOrEmpty(organizations[i]))
                            {
                                long x1 = profile.Id;
                                long OrganizationId = Convert.ToInt64(organizations[i]);
                                int? StartYear = string.IsNullOrEmpty(startyears[i]) ? (int?)null : Convert.ToInt16(startyears[i]);
                                int? GraduateYear = string.IsNullOrEmpty(graduateyears[i]) ? (int?)null : Convert.ToInt16(graduateyears[i]);
                                long? MajorsId = string.IsNullOrEmpty(majorsids[i]) ? (long?)null : Convert.ToInt64(majorsids[i]);
                                long? CourseId = string.IsNullOrEmpty(courseids[i]) ? (long?)null : Convert.ToInt64(courseids[i]);
                                profileOrganizations.Add(new ProfileOrganization()
                                {
                                    ProfileId = profile.Id,
                                    OrganizationId = Convert.ToInt64(organizations[i]),
                                    StartYear = string.IsNullOrEmpty(startyears[i]) ? (int?)null : Convert.ToInt16(startyears[i]),
                                    GraduateYear = string.IsNullOrEmpty(graduateyears[i]) ? (int?)null : Convert.ToInt16(graduateyears[i]),
                                    MajorsId = string.IsNullOrEmpty(majorsids[i]) ? (long?)null : Convert.ToInt64(majorsids[i]),
                                    CourseId = string.IsNullOrEmpty(courseids[i]) ? (long?)null : Convert.ToInt64(courseids[i])
                                });
                            }
                        }
                        if (profileOrganizations != null && profileOrganizations.Any())
                        {
                            int result2 = await _repository.GetRepository<ProfileOrganization>().CreateAsync(profileOrganizations, 0);
                        }
                        TempData["Success"] = true;
                        return RedirectToRoute("FrontEndProfileRegister");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin hồ sơ!");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin hồ sơ!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin hồ sơ!");
                return View(model);
            }
        }
        [Route("~/thong-tin-ho-so", Name = "FrontEndProfileDetail")]
        public ActionResult Detail()
        {
            if (Session["ProfileId"] == null)
            {
                Session.Abandon();
                return RedirectToRoute("FrontEndHomeIndex");
            }
            long profileId = Convert.ToInt64(Session["ProfileId"].ToString());
            var profile = _repository.GetRepository<Profile>().Read(profileId);
            return View(profile);
        }
        [Route("~/cap-nhat-ho-so", Name = "FrontEndProfileUpdate")]
        public async Task<ActionResult> Update()
        {
            if (Session["ProfileId"] == null)
            {
                Session.Abandon();
                return RedirectToRoute("FrontEndHomeIndex");
            }
            long profileId = Convert.ToInt64(Session["ProfileId"].ToString());
            var profile = await _repository.GetRepository<Profile>().ReadAsync(profileId);

            ViewBag.Nations = new SelectList(_repository.GetRepository<Nation>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", profile.NationId);
            ViewBag.Religions = new SelectList(_repository.GetRepository<Religion>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", profile.ReligionId);
            ViewBag.CurrentOrganizations = new SelectList(new Web.Controllers.SharedController().GetOrganizations(null).ToList(), "Id", "Name", profile.CurrentOrganizationId);
            ViewBag.Positions = new SelectList(_repository.GetRepository<Position>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", profile.CurrentPositionId);
            ViewBag.Degrees = new SelectList(_repository.GetRepository<Degree>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", profile.CurrentDegreeId);
            ViewBag.Ranks = new SelectList(_repository.GetRepository<Rank>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", profile.CurrentRankId);
            ViewBag.Organizations = new SelectList(new Web.Controllers.SharedController().GetOrganizations(null, true).ToList(), "Id", "Name", string.Empty);
            ViewBag.Countries = new SelectList(_repository.GetRepository<Country>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", profile.CountryId);

            var provinces = _repository.GetRepository<Province>().GetAll().OrderBy(o => o.Name).ToList();
            ViewBag.NativeLandProvinces = new SelectList(provinces, "Id", "Name", profile.NativeLandProvinceId);
            ViewBag.NativeLandDistricts = new SelectList(_repository.GetRepository<District>().GetAll(o => o.ProvinceId == profile.NativeLandProvinceId).OrderBy(o => o.Name).ToList(), "Id", "Name", profile.NativeLandDistrictId);
            ViewBag.CurrentResidenceProvinces = new SelectList(provinces, "Id", "Name", profile.CurrentResidenceProvinceId);
            ViewBag.CurrentResidenceDistricts = new SelectList(_repository.GetRepository<District>().GetAll(o => o.ProvinceId == profile.CurrentResidenceProvinceId).OrderBy(o => o.Name).ToList(), "Id", "Name", profile.CurrentResidenceDistrictId);

            return View(profile);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Route("~/cap-nhat-ho-so")]
        public async Task<ActionResult> Update(Profile model)
        {
            if (Session["ProfileId"] == null)
            {
                Session.Abandon();
                return RedirectToRoute("FrontEndHomeIndex");
            }
            long profileId = Convert.ToInt64(Session["ProfileId"].ToString());

            ViewBag.Nations = new SelectList(_repository.GetRepository<Nation>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", model.NationId);
            ViewBag.Religions = new SelectList(_repository.GetRepository<Religion>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", model.ReligionId);
            ViewBag.CurrentOrganizations = new SelectList(new Web.Controllers.SharedController().GetOrganizations(null).ToList(), "Id", "Name", model.CurrentOrganizationId);
            ViewBag.Positions = new SelectList(_repository.GetRepository<Position>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", model.CurrentPositionId);
            ViewBag.Degrees = new SelectList(_repository.GetRepository<Degree>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", model.CurrentDegreeId);
            ViewBag.Ranks = new SelectList(_repository.GetRepository<Rank>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", model.CurrentRankId);
            ViewBag.Organizations = new SelectList(new Web.Controllers.SharedController().GetOrganizations(null, true).ToList(), "Id", "Name", string.Empty);
            ViewBag.Countries = new SelectList(_repository.GetRepository<Country>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", model.CountryId);

            var provinces = _repository.GetRepository<Province>().GetAll().OrderBy(o => o.Name).ToList();
            ViewBag.NativeLandProvinces = new SelectList(provinces, "Id", "Name", model.NativeLandProvinceId);
            ViewBag.NativeLandDistricts = new SelectList(_repository.GetRepository<District>().GetAll(o => o.ProvinceId == model.NativeLandProvinceId).OrderBy(o => o.Name).ToList(), "Id", "Name", model.NativeLandDistrictId);
            ViewBag.CurrentResidenceProvinces = new SelectList(provinces, "Id", "Name", model.CurrentResidenceProvinceId);
            ViewBag.CurrentResidenceDistricts = new SelectList(_repository.GetRepository<District>().GetAll(o => o.ProvinceId == model.CurrentResidenceProvinceId).OrderBy(o => o.Name).ToList(), "Id", "Name", model.CurrentResidenceDistrictId);
            try
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(profileId);
                string email = StringHelper.KillChars(Request.Form["Email"]);
                string name = StringHelper.KillChars(Request.Form["Name"]);
                //Kiểm tra chưa nhập họ tên và email
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                {
                    return View(model);
                }
                //Kiểm tra trùng email
                bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == email && o.Profile.Id != profileId);
                bool exists2 = await _repository.GetRepository<Profile>().AnyAsync(o => o.Email == email && o.Id != profileId);
                if (exists || exists2)
                {
                    ModelState.AddModelError(string.Empty, "Địa chỉ email đã tồn tại!");
                    ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng cho tài khoản khác!");
                    return View(model);
                }
                //Kiểm tra chưa chọn trường
                string[] organizations = null;
                if (Request.Form["OrganizationId"] != null)
                    organizations = Request.Form["OrganizationId"].Split(',');
                if (organizations == null || organizations.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng chọn trường!");
                    return View(model);
                }

                string dob = StringHelper.KillChars(Request.Form["DateOfBirth"]);
                string sex = StringHelper.KillChars(Request.Form["Sex"]);
                string cellphone = StringHelper.KillChars(Request.Form["CellPhone"]);
                string profilepicture = StringHelper.KillChars(Request.Form["ProfilePicture"]);
                string homephone = StringHelper.KillChars(Request.Form["HomePhone"]);
                string officephone = StringHelper.KillChars(Request.Form["OfficePhone"]);
                string nativeland = StringHelper.KillChars(Request.Form["NativeLand"]);
                string currentresidence = StringHelper.KillChars(Request.Form["CurrentResidence"]);
                string extrainfomation = StringHelper.KillChars(Request.Form["ExtraInfomation"]);

                long? nationId = model.NationId;
                long? religionId = model.ReligionId;
                long? currentOrganizationId = model.CurrentOrganizationId;
                long? currentPositionId = model.CurrentPositionId;
                long? currentDegreeId = model.CurrentDegreeId;
                long? currentRankId = model.CurrentRankId;

                long? countryId = model.CountryId;
                long? nativeLandProvinceId = model.NativeLandProvinceId;
                long? nativeLandDistrictId = model.NativeLandDistrictId;
                long? currentResidenceProvinceId = model.CurrentResidenceProvinceId;
                long? currentResidenceDistrictId = model.CurrentResidenceDistrictId;

                profile.Name = name;
                profile.Email = email;
                profile.Sex = string.IsNullOrEmpty(sex) ? false : Convert.ToBoolean(sex);
                profile.CellPhone = cellphone;
                profile.ProfilePicture = profilepicture;
                profile.NativeLand = nativeland;
                profile.CurrentResidence = currentresidence;
                profile.ExtraInfomation = extrainfomation;
                profile.HomePhone = homephone;
                profile.OfficePhone = officephone;

                profile.NationId = nationId;
                profile.ReligionId = religionId;
                profile.CurrentOrganizationId = currentOrganizationId;
                profile.CurrentPositionId = currentPositionId;
                profile.CurrentDegreeId = currentDegreeId;
                profile.CurrentRankId = currentRankId;

                profile.CountryId = countryId;
                profile.NativeLandProvinceId = nativeLandProvinceId;
                profile.NativeLandDistrictId = nativeLandDistrictId;
                profile.CurrentResidenceProvinceId = currentResidenceProvinceId;
                profile.CurrentResidenceDistrictId = currentResidenceDistrictId;

                if (!string.IsNullOrEmpty(dob))
                {
                    try
                    {
                        DateTime date = DateTime.ParseExact(dob, "dd/MM/yyyy", null);
                        profile.DateOfBirth = date;
                    }
                    catch
                    {
                        profile.DateOfBirth = null;
                    }
                }
                int result = await _repository.GetRepository<Profile>().UpdateAsync(profile, profileId);
                if (result > 0)
                {
                    var profileOrganizationsToAdd = new List<ProfileOrganization>();
                    List<long> poIds = new List<long>();
                    List<long> poIdsNotToDelete = new List<long>();
                    var po = profile.ProfileOrganizations.ToList();
                    if (po != null && po.Any())
                    {
                        poIds = po.Select(o => o.Id).ToList();
                    }

                    string[] startyears = Request.Form.GetValues("StartYear");
                    string[] graduateyears = Request.Form.GetValues("GraduateYear");
                    string[] majorsids = Request.Form.GetValues("MajorsId");
                    string[] courseids = Request.Form.GetValues("CourseId");

                    string[] profileOrganizations = Request.Form.GetValues("ProfileOrganization");
                    if (profileOrganizations != null && profileOrganizations.Any())
                    {
                        for (int i = 0; i < profileOrganizations.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(profileOrganizations[i]))
                            {
                                //Nếu chọn lần đầu
                                if (profileOrganizations[i] == "0")
                                {
                                    if (!string.IsNullOrEmpty(organizations[i]))
                                    {
                                        int? StartYear = string.IsNullOrEmpty(startyears[i]) ? (int?)null : Convert.ToInt16(startyears[i]);
                                        int? GraduateYear = string.IsNullOrEmpty(graduateyears[i]) ? (int?)null : Convert.ToInt16(graduateyears[i]);
                                        long? MajorsId = string.IsNullOrEmpty(majorsids[i]) ? (long?)null : Convert.ToInt64(majorsids[i]);
                                        long? CourseId = string.IsNullOrEmpty(courseids[i]) ? (long?)null : Convert.ToInt64(courseids[i]);
                                        profileOrganizationsToAdd.Add(new ProfileOrganization()
                                        {
                                            ProfileId = profile.Id,
                                            OrganizationId = Convert.ToInt64(organizations[i]),
                                            StartYear = string.IsNullOrEmpty(startyears[i]) ? (int?)null : Convert.ToInt16(startyears[i]),
                                            GraduateYear = string.IsNullOrEmpty(graduateyears[i]) ? (int?)null : Convert.ToInt16(graduateyears[i]),
                                            MajorsId = string.IsNullOrEmpty(majorsids[i]) ? (long?)null : Convert.ToInt64(majorsids[i]),
                                            CourseId = string.IsNullOrEmpty(courseids[i]) ? (long?)null : Convert.ToInt64(courseids[i])
                                        });
                                    }
                                }
                                else
                                {//Nếu đã chọn từ trước

                                    long poId = Convert.ToInt64(profileOrganizations[i]);
                                    //Nếu trường này đã chọn từ trước
                                    if (poIds.Contains(poId))
                                    {
                                        //Cập nhật ngành, khóa, năm
                                        ProfileOrganization profileOrganization = await _repository.GetRepository<ProfileOrganization>().ReadAsync(poId);
                                        profileOrganization.OrganizationId = Convert.ToInt64(organizations[i]);
                                        profileOrganization.StartYear = string.IsNullOrEmpty(startyears[i]) ? (int?)null : Convert.ToInt16(startyears[i]);
                                        profileOrganization.GraduateYear = string.IsNullOrEmpty(graduateyears[i]) ? (int?)null : Convert.ToInt16(graduateyears[i]);
                                        profileOrganization.MajorsId = string.IsNullOrEmpty(majorsids[i]) ? (long?)null : Convert.ToInt64(majorsids[i]);
                                        profileOrganization.CourseId = string.IsNullOrEmpty(courseids[i]) ? (long?)null : Convert.ToInt64(courseids[i]);
                                        await _repository.GetRepository<ProfileOrganization>().UpdateAsync(profileOrganization, profileId);
                                        //Đánh dấu là không bị xóa
                                        poIdsNotToDelete.Add(poId);
                                    }

                                }
                            }
                        }
                        if (poIdsNotToDelete != null && poIdsNotToDelete.Any())
                        {
                            int result2 = await _repository.GetRepository<ProfileOrganization>().DeleteAsync(o => o.ProfileId == profileId && (!poIdsNotToDelete.Contains(o.Id)), profileId);
                        }
                        if (profileOrganizationsToAdd != null && profileOrganizationsToAdd.Any())
                        {
                            int result3 = await _repository.GetRepository<ProfileOrganization>().CreateAsync(profileOrganizationsToAdd, profileId);
                        }

                    }

                    TempData["Success"] = "Đã cập nhật hồ sơ cựu sinh viên thành công!";
                    return RedirectToRoute("FrontEndProfileDetail");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin hồ sơ!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin hồ sơ!");
                return View(model);
            }
        }
        [Route("~/doi-mat-khau", Name = "FrontEndProfileChangePassword")]
        public ActionResult ChangePassword()
        {
            ViewBag.Success = TempData["Success"];
            return View();
        }
        [Route("doi-mat-khau")]
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
                    {
                        ModelState.AddModelError(string.Empty, "Không tìm thấy tài khoản!");
                        return View(model);
                    }
                    string password = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.OldPassword)).ToLower());
                    string newPassword = StringHelper.StringToMd5(StringHelper.StringToMd5(StringHelper.KillChars(model.NewPassword)).ToLower());
                    //Kiểm tra mật khẩu cũ
                    if (password == account.Password)
                    {
                        account.Password = newPassword;
                        int result = await _repository.GetRepository<Account>().UpdateAsync(account, accountId);
                        if (result > 0)
                        {
                            TempData["Success"] = true;
                            return RedirectToRoute("FrontEndProfileChangePassword");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Đổi mật khẩu không thành công!");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không chính xác, vui lòng kiểm tra lại!");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập chính xác các thông tin!");
                return View(model);
            }
        }
    }
}