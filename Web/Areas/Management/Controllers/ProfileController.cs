using Common;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("ho-so-cuu-sinh-vien")]
    public class ProfileController : BaseController
    {
        [Route("danh-sach-ho-so", Name = "ProfileIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Profile)]
        public ActionResult Index()
        {
            return View();
        }
        [Route("nhap-ho-so", Name = "ProfileCreate")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Profile)]
        public ActionResult Create()
        {
            ViewBag.Nations = new SelectList(_repository.GetRepository<Nation>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            ViewBag.Religions = new SelectList(_repository.GetRepository<Religion>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            ViewBag.CurrentOrganizations = new SelectList(new Web.Controllers.SharedController().GetOrganizations(null).ToList(), "Id", "Name", string.Empty);
            ViewBag.Positions = new SelectList(_repository.GetRepository<Position>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            ViewBag.Degrees = new SelectList(_repository.GetRepository<Degree>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            ViewBag.Ranks = new SelectList(_repository.GetRepository<Rank>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);
            ViewBag.Organizations = new SelectList(new Web.Controllers.SharedController().GetOrganizations(null, true).ToList(), "Id", "Name", string.Empty);
            ViewBag.Countries = new SelectList(_repository.GetRepository<Country>().GetAll().OrderBy(o => o.Name).ToList(), "Id", "Name", string.Empty);

            var provinces = _repository.GetRepository<Province>().GetAll().OrderBy(o => o.Name).ToList();
            ViewBag.NativeLandProvinces = new SelectList(provinces, "Id", "Name", string.Empty);
            ViewBag.NativeLandDistricts = new SelectList(new List<District>(), "Id", "Name", string.Empty);
            ViewBag.CurrentResidenceProvinces = new SelectList(provinces, "Id", "Name", string.Empty);
            ViewBag.CurrentResidenceDistricts = new SelectList(new List<District>(), "Id", "Name", string.Empty);

            return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Route("nhap-ho-so")]
        [ValidationPermission(Action = ActionEnum.Create, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Create(RegisterProfileViewModel model)
        {
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

                Profile profile = new Profile();

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

                profile.Status = 1;
                profile.CreateDate = DateTime.Now;
                profile.CreateBy = AccountId;

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
                int result = await _repository.GetRepository<Profile>().CreateAsync(profile, AccountId);
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
                            //long x1 = profile.Id;
                            //long OrganizationId = Convert.ToInt64(organizations[i]);
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
                        int result2 = await _repository.GetRepository<ProfileOrganization>().CreateAsync(profileOrganizations, AccountId);
                    }
                    TempData["Success"] = "Đã nhập hồ sơ cựu sinh viên thành công!";
                    return RedirectToRoute("ProfileIndex");
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
        [Route("danh-sach-ho-so-json/{status?}", Name = "ProfileGetProfilesJson")]
        public ActionResult GetProfilesJson(byte status)
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

            var aos = _repository.GetRepository<AccountOrganization>().GetAll(o => o.AccountId == AccountId);
            List<long> organizationIds = aos.Select(o => o.OrganizationId).ToList();

            var profiles = _repository.GetRepository<Profile>().GetAll(ref paging, orderKey, o => (
               key == null ||
               key == "" ||
               o.Email.Contains(key) ||
               o.Name.Contains(key) ||
               o.CellPhone.Contains(key)) && o.Status == status
               )
                .Join(_repository.GetRepository<ProfileOrganization>().GetAll(
               po => (organizationIds.Contains(po.OrganizationId))
               ), b => b.Id, c => c.ProfileId, (b, c) => new { Profile = b }).Select(x => x.Profile).Distinct()
               .ToList<Profile>();

            return Json(new
            {
                draw = drawReturn,
                recordsTotal = paging.TotalRecord,
                recordsFiltered = paging.TotalRecord,
                data = profiles.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Email,
                    o.CellPhone,
                    DateOfBirth = o.DateOfBirth.HasValue ? o.DateOfBirth.Value.ToString("dd/MM/yyyy") : "",
                    CreateDate = o.CreateDate.ToString("dd/MM/yyyy")
                })
            }, JsonRequestBehavior.AllowGet);
        }
        [Route("xem-chi-tiet-ho-so/{id?}", Name = "ProfileDetail")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Detail(long id)
        {
            var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
            return View(profile);
        }
        [Route("xoa-ho-so/{id?}", Name = "ProfileDelete")]
        [ValidationPermission(Action = ActionEnum.Delete, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
                var account = await _repository.GetRepository<Account>().ReadAsync(o => o.Profile != null && o.Profile.Id == id);
                if (account != null)
                {
                    account.Profile = null;
                    await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                }
                int result = await _repository.GetRepository<ProfileOrganization>().DeleteAsync(profile.ProfileOrganizations, AccountId);
                int result2 = await _repository.GetRepository<Profile>().DeleteAsync(profile, AccountId);
                if (result2 > 0)
                {
                    TempData["Success"] = "Xóa hồ sơ thành công!";
                    return RedirectToRoute("ProfileIndex");
                }
                else
                {
                    TempData["Error"] = "Không xóa được hồ sơ!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToRoute("ProfileDetail", new { id = id });
            }
        }
        [Route("duyet-ho-so/{id?}", Name = "ProfileApprove")]
        [ValidationPermission(Action = ActionEnum.Verify, Module = ModuleEnum.ProfileApprove)]
        public async Task<ActionResult> Approve(long id)
        {
            try
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
                if (profile.Status == 2)
                    return RedirectToRoute("ProfileDetail", new { id = id });
                //Kiểm tra quyền
                var accountOrganizations = await _repository.GetRepository<AccountOrganization>().GetAllAsync(o => o.AccountId == AccountId);
                if (!profile.ProfileOrganizations.Any(o => accountOrganizations.Any(x => x.OrganizationId == o.OrganizationId)))
                {
                    TempData["Error"] = "Bạn không được phân quyền duyệt hồ sơ đối với đơn vị này!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
                //End kiểm tra quyền
                var currentStatus = profile.Status;
                profile.Status = 2;
                profile.ApproveBy = AccountId;
                profile.ApproveDate = DateTime.Now;
                var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                int result = await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                if (result > 0)
                {
                    //Tạo tài khoản
                    bool acc = false;
                    string password = StringHelper.CreateRandomString(8);
                    password = StringHelper.StringToMd5(StringHelper.StringToMd5(password).ToLower());
                    if (profile.Account == null)
                    {
                        bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == profile.Email && o.Profile.Id == id);
                        if (!exists)
                        {
                            var account = new Account();
                            account.IsManageAccount = false;
                            account.IsNormalAccount = true;
                            account.Password = password;
                            account.Name = profile.Name;
                            account.Email = profile.Email;
                            account.CreateDate = DateTime.Now;
                            account.PhoneNumber = profile.CellPhone;
                            account.ProfilePicture = profile.ProfilePicture;
                            account.Profile = profile;
                            int result3 = await _repository.GetRepository<Account>().CreateAsync(account, AccountId);
                            if (result3 > 0)
                            {
                                acc = true;
                                profile.Account = account;
                                await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                            }
                        }
                    }
                    //End tạo tài khoản
                    //Gửi thư
                    string title = "Hồ sơ của quí vị đã được duyệt";
                    string domainName = Request.Url.Scheme + "://" + Request.Url.Authority;
                    StringBuilder body = new StringBuilder();
                    body.Append("Kính gửi " + StringHelper.KillChars(profile.Name) + ",<br /><br />");
                    body.Append("Hồ sơ cựu sinh viên của quí vị tại <a href=\"" + domainName + " target=\"_blank\">" + systemInfo.SiteName + "</a> đã được phê duyệt. <br />");

                    if (acc)
                    {
                        body.Append("Đồng thời tài khoản của quí vị cũng đã được tạo.<br />");
                        body.Append("Tên truy cập là: " + profile.Email + "<br />");
                        body.Append("Mật khẩu là: " + password + "<br />");
                        body.Append("Quí vị vui lòng đổi mật khẩu ngay khi đăng nhập.<br />");
                    }

                    body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                    body.Append("<br />Kính thư, <br /><br />");
                    body.Append(systemInfo.SiteName + "<br />");
                    body.Append("Phát triển bởi eBtech Team<br />");
                    body.Append("Webmaster: quochuy7it@gmail.com");
                    bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, profile.Email, title, body.ToString());
                    if (result2)
                    {
                        TempData["Success"] = "Duyệt hồ sơ thành công!";
                        return RedirectToRoute("ProfileDetail", new { id = id });
                    }
                    else
                    {
                        try
                        {
                            profile.Status = currentStatus;
                            profile.ApproveDate = null;
                            profile.ApproveBy = null;
                            await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                        }
                        catch { }
                        TempData["Error"] = "Lỗi: Không duyệt được hồ sơ! (Không gửi được email)";
                        return RedirectToRoute("ProfileDetail", new { id = id });
                    }
                }
                else
                {
                    TempData["Error"] = "Lỗi: Không duyệt được hồ sơ!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToRoute("ProfileDetail", new { id = id });
            }
        }
        [HttpPost, ValidateInput(false)]
        [Route("huy-duyet-ho-so", Name = "ProfileUnApprovedJson")]
        [ValidationPermission(Action = ActionEnum.Verify, Module = ModuleEnum.ProfileApprove)]
        public async Task<ActionResult> UnApproved()
        {
            try
            {
                string UnApprovedMessage = StringHelper.KillChars(Request.Form["UnApprovedMessage"]);
                long profileId = Convert.ToInt64(Request.Form["profileId"]);
                //type=1: Không được duyệt; type=2: Đã duyệt nhưng bị hủy
                byte type = Convert.ToByte(Request.Form["type"]);
                var profile = await _repository.GetRepository<Profile>().ReadAsync(profileId);
                if (profile == null)
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy hồ sơ!" }, JsonRequestBehavior.AllowGet);
                }
                //Kiểm tra quyền
                var accountOrganizations = await _repository.GetRepository<AccountOrganization>().GetAllAsync(o => o.AccountId == AccountId);
                if (!profile.ProfileOrganizations.Any(o => accountOrganizations.Any(x => x.OrganizationId == o.OrganizationId)))
                {
                    return Json(new { success = false, message = "Lỗi: Bạn không được phân quyền duyệt hồ sơ đối với đơn vị này!" }, JsonRequestBehavior.AllowGet);
                }
                //End kiểm tra quyền
                var account = await _repository.GetRepository<Account>().ReadAsync(o => o.Profile != null && o.Profile.Id == profileId);
                var currentStatus = profile.Status;
                profile.Status = 3;
                profile.UnApprovedMessage = UnApprovedMessage;
                profile.UnApproveDate = DateTime.Now;
                profile.UnApproveBy = AccountId;
                profile.Account = null;
                var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                int result = await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                if (result > 0)
                {
                    //Gửi thư
                    string title = type == 1 ? "Hồ sơ không được duyệt" : "Hồ sơ đã bị hủy";
                    string tmp = type == 1 ? "Hồ sơ của quí vị không được duyệt vì: <br />" : "Hồ sở của quí vị đã bị hủy vì: <br />";
                    StringBuilder body = new StringBuilder();
                    body.Append("Kính gửi " + StringHelper.KillChars(profile.Name) + ",<br /><br />");
                    body.Append(tmp);
                    body.Append(UnApprovedMessage);
                    body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                    body.Append("<br />Kính thư, <br /><br />");
                    body.Append(systemInfo.SiteName + "<br />");
                    body.Append("Phát triển bởi eBtech Team<br />");
                    body.Append("Webmaster: quochuy7it@gmail.com");
                    bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, profile.Email, title, body.ToString());
                    if (result2)
                    {
                        if (account != null)
                        {
                            account.Profile = null;
                            await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                        }
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        try
                        {
                            profile.Status = currentStatus;
                            profile.UnApprovedMessage = "";
                            profile.UnApproveDate = null;
                            profile.UnApproveBy = null;
                            profile.Account = account;
                            await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                        }
                        catch { }
                        return Json(new { success = false, message = "Không ghi nhận được dữ liệu. (Không gửi được email)" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không ghi nhận được dữ liệu!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("nhap-ly-do-khong-duyet-modal/{id?}/{type?}", Name = "ProfileUnApprovedMessageModal")]
        public async Task<ActionResult> UnApprovedMessageModal(long id, byte type)
        {
            var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
            ViewBag.Type = type;
            return PartialView("_UnApprovedMessageModal", profile);
        }
        [Route("cap-nhat-ho-so/{id?}", Name = "ProfileUpdate")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Update(long id)
        {
            var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
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
        [Route("cap-nhat-ho-so/{id?}")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Update(long id, Profile model)
        {
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
                var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
                string email = StringHelper.KillChars(Request.Form["Email"]);
                string name = StringHelper.KillChars(Request.Form["Name"]);
                //Kiểm tra chưa nhập họ tên và email
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                {
                    return View(model);
                }
                //Kiểm tra trùng email
                bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == email && o.Profile.Id != id);
                bool exists2 = await _repository.GetRepository<Profile>().AnyAsync(o => o.Email == email && o.Id != id);
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
                int result = await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
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
                                        await _repository.GetRepository<ProfileOrganization>().UpdateAsync(profileOrganization, AccountId);
                                        //Đánh dấu là không bị xóa
                                        poIdsNotToDelete.Add(poId);
                                    }

                                }
                            }
                        }
                        if (poIdsNotToDelete != null && poIdsNotToDelete.Any())
                        {
                            int result2 = await _repository.GetRepository<ProfileOrganization>().DeleteAsync(o => o.ProfileId == id && (!poIdsNotToDelete.Contains(o.Id)), AccountId);
                        }
                        if (profileOrganizationsToAdd != null && profileOrganizationsToAdd.Any())
                        {
                            int result3 = await _repository.GetRepository<ProfileOrganization>().CreateAsync(profileOrganizationsToAdd, AccountId);
                        }

                    }

                    TempData["Success"] = "Đã cập nhật hồ sơ cựu sinh viên thành công!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
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
        [Route("tao-tai-khoan/{id?}", Name = "ProfileCreateAccount")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> CreateAccount(long id)
        {
            var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
            if (profile.Account != null)
            {
                TempData["Error"] = "Hồ sơ này đã được tạo tài khoản!";
                return RedirectToRoute("ProfileDetail", new { id = id });
            }
            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("tao-tai-khoan/{id?}")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> CreateAccount(long id, FormCollection model)
        {
            try
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
                if (profile == null)
                    return RedirectToRoute("ProfileIndex");

                bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == profile.Email && o.Profile.Id == id);
                if (exists)
                {
                    TempData["Error"] = "Địa chỉ email của hồ sơ này đã có tài khoản khác sử dụng!";
                    return RedirectToRoute("ProfileCreateAccount", new { id = id });
                }
                bool exists2 = await _repository.GetRepository<Account>().AnyAsync(o => o.Profile.Id == id);
                if (exists2)
                {
                    TempData["Error"] = "Hồ sơ này đã được tạo tài khoản!";
                    return RedirectToRoute("ProfileCreateAccount", new { id = id });
                }
                string password = StringHelper.KillChars(model["Password"]).Trim();
                string confirmPassword = StringHelper.KillChars(model["ConfirmPassword"]).Trim();
                if (string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Vui lòng nhập mật khẩu!";
                    return RedirectToRoute("ProfileCreateAccount", new { id = id });
                }
                if (password != confirmPassword)
                {
                    TempData["Error"] = "Mật khẩu không khớp nhau!";
                    return RedirectToRoute("ProfileCreateAccount", new { id = id });
                }
                password = StringHelper.StringToMd5(StringHelper.StringToMd5(password).ToLower());
                var account = new Account();
                account.IsManageAccount = false;
                account.IsNormalAccount = true;
                account.Password = password;
                account.Name = profile.Name;
                account.Email = profile.Email;
                account.CreateDate = DateTime.Now;
                account.PhoneNumber = profile.CellPhone;
                account.ProfilePicture = profile.ProfilePicture;
                account.Profile = profile;
                int result = await _repository.GetRepository<Account>().CreateAsync(account, AccountId);
                if (result > 0)
                {
                    profile.Account = account;
                    await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                    try
                    {
                        var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                        if (systemInfo != null)
                        {
                            //Gửi thư
                            string title = "Tài khoản của quí vị đã được tạo";
                            string domainName = Request.Url.Scheme + "://" + Request.Url.Authority;
                            StringBuilder body = new StringBuilder();
                            body.Append("Kính gửi " + StringHelper.KillChars(profile.Name) + ",<br /><br />");
                            body.Append("Tài khoản cựu sinh viên của quí vị tại <a href=\"" + domainName + " target=\"_blank\">" + systemInfo.SiteName + "</a> đã được tạo. <br />");

                            body.Append("Tên truy cập là: " + profile.Email + "<br />");
                            body.Append("Mật khẩu là: " + password + "<br />");
                            body.Append("Quí vị vui lòng đổi mật khẩu ngay khi đăng nhập.<br />");

                            body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                            body.Append("<br />Kính thư, <br /><br />");
                            body.Append(systemInfo.SiteName + "<br />");
                            body.Append("Phát triển bởi eBtech Team<br />");
                            body.Append("Webmaster: quochuy7it@gmail.com");
                            bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, profile.Email, title, body.ToString());
                        }
                    }
                    catch { }
                    TempData["Success"] = "Tạo tài khoản cho hồ sơ thành công!";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
                else
                {
                    TempData["Error"] = "Tạo tài khoản cho hồ sơ không thành công!";
                    return RedirectToRoute("ProfileCreateAccount", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToRoute("ProfileCreateAccount", new { id = id });
            }
        }
        [Route("phan-quyen-ho-so/{id?}", Name = "ProfileCommittee")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Committee(long id)
        {
            var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
            if (profile.Account == null)
            {
                TempData["Error"] = "Hồ sơ này chưa được tạo tài khoản! Bạn phải tạo tài khoản cho hồ sơ trước.";
                return RedirectToRoute("ProfileDetail", new { id = id });
            }
            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("phan-quyen-ho-so/{id?}")]
        [ValidationPermission(Action = ActionEnum.Update, Module = ModuleEnum.Profile)]
        public async Task<ActionResult> Committee(long id, FormCollection model)
        {
            try
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
                if (profile == null)
                    return RedirectToRoute("ProfileIndex");
                if (profile.Account == null)
                {
                    TempData["Error"] = "Hồ sơ này chưa được tạo tài khoản! Bạn phải tạo tài khoản cho hồ sơ trước.";
                    return RedirectToRoute("ProfileDetail", new { id = id });
                }
                //"on".Equals(Request.Params[item.Value + "_Read"]) ? 1 : 0
                if (profile.ProfileOrganizations != null && profile.ProfileOrganizations.Any())
                {
                    foreach (var item in profile.ProfileOrganizations)
                    {
                        bool isLiaisonCommittee = "on".Equals(Request.Params["IsLiaisonCommittee_" + item.Id.ToString()]) ? true : false;
                        var po = await _repository.GetRepository<ProfileOrganization>().ReadAsync(item.Id);
                        po.IsLiaisonCommittee = isLiaisonCommittee;
                        await _repository.GetRepository<ProfileOrganization>().UpdateAsync(po, AccountId);
                    }
                }
                var exists = await _repository.GetRepository<ProfileOrganization>().AnyAsync(o => o.ProfileId == id && o.IsLiaisonCommittee == true);
                if (exists)
                {
                    var account = profile.Account;
                    account.IsManageAccount = true;
                    await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                }

                TempData["Success"] = "Đã ghi nhận thành công!";
                return RedirectToRoute("ProfileCommittee", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToRoute("ProfileCommittee", new { id = id });
            }
        }

        //Phần dành cho thành viên ban liên lạc phê duyệt hồ sơ của thành viên khác

        [Route("danh-sach-ho-so-cuu-sinh-vien", Name = "ProfileList")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.ProfileApproveCommittee)]
        public ActionResult List()
        {
            var account = _repository.GetRepository<Account>().Read(AccountId);
            if (account.Profile == null ||
                account.Profile.ProfileOrganizations == null ||
                !account.Profile.ProfileOrganizations.Any(o => o.IsLiaisonCommittee == true)
                )
            {
                TempData["Exception"] = "Bạn không có quyền truy cập chức năng này!";
                return RedirectToRoute("ManagementGeneralError");
            }
            return View();
        }
        [Route("xem-chi-tiet-ho-so-cuu-sinh-vien/{id?}", Name = "ProfileDetailProfile")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.ProfileApproveCommittee)]
        public async Task<ActionResult> DetailProfile(long id)
        {
            var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
            return View(profile);
        }
        [Route("duyet-ho-so-cuu-sinh-vien/{id?}", Name = "ProfileApproveProfile")]
        [ValidationPermission(Action = ActionEnum.Verify, Module = ModuleEnum.ProfileApproveCommittee)]
        public async Task<ActionResult> ApproveProfile(long id)
        {
            try
            {
                var profile = await _repository.GetRepository<Profile>().ReadAsync(id);
                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ!";
                    return RedirectToRoute("ProfileDetailProfile", new { id = id });
                }
                if (profile.Status == 2)
                    return RedirectToRoute("ProfileDetailProfile", new { id = id });
                //Kiểm tra quyền
                var account = await _repository.GetRepository<Account>().ReadAsync(AccountId);
                if (account.Profile == null ||
                 account.Profile.ProfileOrganizations == null ||
                 !account.Profile.ProfileOrganizations.Any(o => o.IsLiaisonCommittee == true)
                 )
                {
                    TempData["Error"] = "Bạn không có quyền duyệt hồ sơ này!";
                    return RedirectToRoute("ProfileList");
                }
                else
                {
                    var pos = account.Profile.ProfileOrganizations.Where(o => o.IsLiaisonCommittee == true);
                    List<long> organizationIds = pos.Select(o => o.OrganizationId).ToList();
                    List<long> majorsIds = pos.Where(o => o.MajorsId.HasValue).Select(o => o.MajorsId.Value).ToList();
                    List<long> courseIds = pos.Where(o => o.CourseId.HasValue).Select(o => o.CourseId.Value).ToList();
                    if (!profile.ProfileOrganizations.Any(po =>
                          (organizationIds.Contains(po.OrganizationId)
                          && (!po.MajorsId.HasValue || majorsIds.Contains(po.MajorsId.Value))
                          && (!po.CourseId.HasValue || courseIds.Contains(po.CourseId.Value)))))
                    {
                        TempData["Error"] = "Bạn không có quyền duyệt hồ sơ này!";
                        return RedirectToRoute("ProfileList");
                    }
                }
                //End kiểm tra quyền

                var currentStatus = profile.Status;
                profile.Status = 2;
                profile.ApproveBy = AccountId;
                profile.ApproveDate = DateTime.Now;
                var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                int result = await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                if (result > 0)
                {
                    //Tạo tài khoản
                    bool acc = false;
                    string password = StringHelper.CreateRandomString(8);
                    password = StringHelper.StringToMd5(StringHelper.StringToMd5(password).ToLower());
                    if (profile.Account == null)
                    {
                        bool exists = await _repository.GetRepository<Account>().AnyAsync(o => o.Email == profile.Email && o.Profile.Id == id);
                        if (!exists)
                        {
                            var account2 = new Account();
                            account2.IsManageAccount = false;
                            account2.IsNormalAccount = true;
                            account2.Password = password;
                            account2.Name = profile.Name;
                            account2.Email = profile.Email;
                            account2.CreateDate = DateTime.Now;
                            account2.PhoneNumber = profile.CellPhone;
                            account2.ProfilePicture = profile.ProfilePicture;
                            account2.Profile = profile;
                            int result3 = await _repository.GetRepository<Account>().CreateAsync(account2, AccountId);
                            if (result3 > 0)
                            {
                                acc = true;
                                profile.Account = account2;
                                await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                            }
                        }
                    }
                    //End tạo tài khoản
                    //Gửi thư
                    string title = "Hồ sơ của quí vị đã được duyệt";
                    string domainName = Request.Url.Scheme + "://" + Request.Url.Authority;
                    StringBuilder body = new StringBuilder();
                    body.Append("Kính gửi " + StringHelper.KillChars(profile.Name) + ",<br /><br />");
                    body.Append("Hồ sơ cựu sinh viên của quí vị tại <a href=\"" + domainName + " target=\"_blank\">" + systemInfo.SiteName + "</a> đã được phê duyệt. <br />");
                    if (acc)
                    {
                        body.Append("Đồng thời tài khoản của quí vị cũng đã được tạo.<br />");
                        body.Append("Tên truy cập là: " + profile.Email + "<br />");
                        body.Append("Mật khẩu là: " + password + "<br />");
                        body.Append("Quí vị vui lòng đổi mật khẩu ngay khi đăng nhập.<br />");
                    }
                    body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                    body.Append("<br />Kính thư, <br /><br />");
                    body.Append(systemInfo.SiteName + "<br />");
                    body.Append("Phát triển bởi eBtech Team<br />");
                    body.Append("Webmaster: quochuy7it@gmail.com");
                    bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, profile.Email, title, body.ToString());
                    if (result2)
                    {
                        TempData["Success"] = "Duyệt hồ sơ thành công!";
                        return RedirectToRoute("ProfileDetailProfile", new { id = id });
                    }
                    else
                    {
                        try
                        {
                            profile.Status = currentStatus;
                            profile.ApproveDate = null;
                            profile.ApproveBy = null;
                            await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                        }
                        catch { }
                        TempData["Error"] = "Lỗi: Không duyệt được hồ sơ! (Không gửi được email)";
                        return RedirectToRoute("ProfileDetailProfile", new { id = id });
                    }
                }
                else
                {
                    TempData["Error"] = "Lỗi: Không duyệt được hồ sơ!";
                    return RedirectToRoute("ProfileDetailProfile", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToRoute("ProfileDetail", new { id = id });
            }
        }
        [HttpPost, ValidateInput(false)]
        [Route("huy-duyet-ho-so-cuu-sinh-vien", Name = "ProfileUnApprovedProfileJson")]
        [ValidationPermission(Action = ActionEnum.Verify, Module = ModuleEnum.ProfileApproveCommittee)]
        public async Task<ActionResult> UnApprovedProfile()
        {
            try
            {
                string UnApprovedMessage = StringHelper.KillChars(Request.Form["UnApprovedMessage"]);
                long profileId = Convert.ToInt64(Request.Form["profileId"]);
                //type=1: Không được duyệt; type=2: Đã duyệt nhưng bị hủy
                byte type = Convert.ToByte(Request.Form["type"]);
                var profile = await _repository.GetRepository<Profile>().ReadAsync(profileId);
                if (profile == null)
                {
                    return Json(new { success = false, message = "Lỗi: Không tìm thấy hồ sơ!" }, JsonRequestBehavior.AllowGet);
                }
                var account = await _repository.GetRepository<Account>().ReadAsync(o => o.Profile != null && o.Profile.Id == profileId);
                var currentStatus = profile.Status;
                profile.Status = 3;
                profile.UnApprovedMessage = UnApprovedMessage;
                profile.UnApproveDate = DateTime.Now;
                profile.UnApproveBy = AccountId;
                profile.Account = null;
                var systemInfo = (await _repository.GetRepository<SystemInformation>().GetAllAsync()).FirstOrDefault();
                int result = await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                if (result > 0)
                {
                    //Gửi thư
                    string title = type == 1 ? "Hồ sơ không được duyệt" : "Hồ sơ đã bị hủy";
                    string tmp = type == 1 ? "Hồ sơ của quí vị không được duyệt vì: <br />" : "Hồ sở của quí vị đã bị hủy vì: <br />";
                    StringBuilder body = new StringBuilder();
                    body.Append("Kính gửi " + StringHelper.KillChars(profile.Name) + ",<br /><br />");
                    body.Append(tmp);
                    body.Append(UnApprovedMessage);
                    body.Append("<br /><br />Vô cùng xin lỗi nếu email này làm phiền quí vị!<br /><br />");
                    body.Append("<br />Kính thư, <br /><br />");
                    body.Append(systemInfo.SiteName + "<br />");
                    body.Append("Phát triển bởi eBtech Team<br />");
                    body.Append("Webmaster: quochuy7it@gmail.com");
                    bool result2 = await SendEmail.SendAsync(systemInfo.SMTPName, systemInfo.SMTPEmail, systemInfo.SMTPPassword, profile.Email, title, body.ToString());
                    if (result2)
                    {
                        if (account != null)
                        {
                            account.Profile = null;
                            await _repository.GetRepository<Account>().UpdateAsync(account, AccountId);
                        }
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        try
                        {
                            profile.Status = currentStatus;
                            profile.UnApprovedMessage = "";
                            profile.UnApproveDate = null;
                            profile.UnApproveBy = null;
                            profile.Account = account;
                            await _repository.GetRepository<Profile>().UpdateAsync(profile, AccountId);
                        }
                        catch { }
                        return Json(new { success = false, message = "Không ghi nhận được dữ liệu. (Không gửi được email)" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi: Không ghi nhận được dữ liệu!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("danh-sach-ho-so-cuu-sinh-vien-json/{status?}", Name = "ProfileGetProfilesListJson")]
        public ActionResult GetProfilesListJson(byte status)
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
            var account = _repository.GetRepository<Account>().Read(AccountId);
            if (account.Profile == null ||
                account.Profile.ProfileOrganizations == null ||
                !account.Profile.ProfileOrganizations.Any(o => o.IsLiaisonCommittee == true)
                )
            {
                return Json(new
                {
                    draw = drawReturn,
                    recordsTotal = paging.TotalRecord,
                    recordsFiltered = paging.TotalRecord,
                    data = ""
                }, JsonRequestBehavior.AllowGet);
            }
            var pos = account.Profile.ProfileOrganizations.Where(o => o.IsLiaisonCommittee == true);
            List<long> organizationIds = pos.Select(o => o.OrganizationId).ToList();
            List<long> majorsIds = pos.Where(o => o.MajorsId.HasValue).Select(o => o.MajorsId.Value).ToList();
            List<long> courseIds = pos.Where(o => o.CourseId.HasValue).Select(o => o.CourseId.Value).ToList();


            var profiles = _repository.GetRepository<Profile>().GetAll(ref paging, orderKey, o => (
               key == null ||
               key == "" ||
               o.Email.Contains(key) ||
               o.Name.Contains(key) ||
               o.CellPhone.Contains(key)) && o.Status == status)
               .Join(_repository.GetRepository<ProfileOrganization>().GetAll(
               po => (organizationIds.Contains(po.OrganizationId) && (!po.MajorsId.HasValue || majorsIds.Contains(po.MajorsId.Value)) && (!po.CourseId.HasValue || courseIds.Contains(po.CourseId.Value)))
               ), b => b.Id, c => c.ProfileId, (b, c) => new { Profile = b }).Select(x => x.Profile).Distinct().ToList<Profile>();

            return Json(new
            {
                draw = drawReturn,
                recordsTotal = paging.TotalRecord,
                recordsFiltered = paging.TotalRecord,
                data = profiles.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Email,
                    o.CellPhone,
                    DateOfBirth = o.DateOfBirth.HasValue ? o.DateOfBirth.Value.ToString("dd/MM/yyyy") : "",
                    CreateDate = o.CreateDate.ToString("dd/MM/yyyy")
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}