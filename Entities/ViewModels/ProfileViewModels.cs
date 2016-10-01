using System.ComponentModel.DataAnnotations;
namespace Entities.ViewModels
{
    public class RegisterProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên của bạn!")]
        [Display(Name = "Họ và tên")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Ngày sinh")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public bool? Sex { get; set; }

        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        [Display(Name = "Địa chỉ email")]
        [StringLength(50, ErrorMessage = "Địa chỉ email không được vượt quá 50 ký tự!")]
        public string Email { get; set; }

        [Display(Name = "Điện thoại di động")]
        [StringLength(20, ErrorMessage = "Điện thoại di động không được vượt quá 20 ký tự!")]
        public string CellPhone { get; set; }

        [Display(Name = "Điện thoại nhà riêng")]
        [StringLength(20, ErrorMessage = "Điện thoại nhà riêng không được vượt quá 20 ký tự!")]
        public string HomePhone { get; set; }

        [Display(Name = "Điện thoại cơ quan")]
        [StringLength(20, ErrorMessage = "Điện thoại cơ quan không được vượt quá 20 ký tự!")]
        public string OfficePhone { get; set; }

        [Display(Name = "Quê quán")]
        [StringLength(250, ErrorMessage = "Quê quán không được vượt quá 250 ký tự!")]
        public string NativeLand { get; set; }

        [Display(Name = "Nơi ở hiện tại")]
        [StringLength(250, ErrorMessage = "Nơi ở hiện tại không được vượt quá 250 ký tự!")]
        public string CurrentResidence { get; set; }

        [Display(Name = "Thông tin mở rộng")]
        [StringLength(1000, ErrorMessage = "Thông tin mở rộng không được vượt quá 1000 ký tự!")]
        public string ExtraInfomation { get; set; }

        [Display(Name = "Dân tộc")]
        public long? NationId { get; set; }

        [Display(Name = "Tôn giáo")]
        public long? ReligionId { get; set; }

        [Display(Name = "Cơ quan làm việc hiện tại")]
        public long? CurrentOrganizationId { get; set; }

        [Display(Name = "Chức vụ hiện tại")]
        public long? CurrentPositionId { get; set; }

        [Display(Name = "Học vị hiện tại")]
        public long? CurrentDegreeId { get; set; }

        [Display(Name = "Học hàm hiện tại")]
        public long? CurrentRankId { get; set; }

        [Display(Name = "Ảnh đại diện")]
        [StringLength(200, ErrorMessage = "Ảnh đại diện không được vượt quá 200 ký tự!")]
        public string ProfilePicture { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trường!")]
        [Display(Name = "Trường")]
        public long? OrganizationId { get; set; }

        //Năm bắt đầu học
        [Display(Name = "Năm nhập học")]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Vui lòng chỉ nhập số nguyên dương!")]
        public int? StartYear { get; set; }

        //Năm tốt nghiệp
        [Display(Name = "Năm tốt nghiệp")]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Vui lòng chỉ nhập số nguyên dương!")]
        public int? GraduateYear { get; set; }

        [Display(Name = "Ngành học")]
        public long? MajorsId { get; set; }

        [Display(Name = "Khóa học")]
        public long? CourseId { get; set; }

        [Display(Name = "Quốc tịch")]
        public long? CountryId { get; set; }

        //NativeLand
        [Display(Name = "Tỉnh/TP (quê quán)")]
        public long? NativeLandProvinceId { get; set; }

        [Display(Name = "Quận/Huyện (quê quán)")]
        public long? NativeLandDistrictId { get; set; }

        //CurrentResidence
        [Display(Name = "Tỉnh/TP (nơi ở hiện tại)")]
        public long? CurrentResidenceProvinceId { get; set; }

        [Display(Name = "Quận/Huyện (nơi ở hiện tại)")]
        public long? CurrentResidenceDistrictId { get; set; }
    }
}
