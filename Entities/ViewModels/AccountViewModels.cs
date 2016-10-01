using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class AccountCreatingViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên!")]
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }
        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Display(Name = "Địa chỉ E-mail!")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        public string Email { get; set; }
        [Display(Name = "Mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        public string Password { get; set; }
    }
    public class AccountUpdatingViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên!")]
        [Display(Name = "Họ và tên")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự!")]
        public string Name { get; set; }

        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Display(Name = "Địa chỉ E-mail!")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        [StringLength(50, ErrorMessage = "Địa chỉ E-mail không được vượt quá 50 ký tự!")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "Đường dẫn ảnh đại diện không được vượt quá 200 ký tự!")]
        [Display(Name = "Ảnh đại diện")]
        public string ProfilePicture { get; set; }

        [StringLength(20, ErrorMessage = "Điện thoại không được vượt quá 20 ký tự!")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Display(Name = "Mật khẩu cũ")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ!")]
        public string OldPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới!")]
        public string NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu mới")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới!")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp nhau!")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangeMemberPasswordViewModel
    {
        [Display(Name = "Mật khẩu mới")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới!")]
        public string NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu mới")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới!")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp nhau!")]
        public string ConfirmPassword { get; set; }
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ E-mail!")]
        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Display(Name = "Địa chỉ E-mail!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [Display(Name = "Mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        public string Password { get; set; }
    }
    public class AccountMappingRole
    {
        public long AccountId { get; set; }
        public long RoleId { get; set; }
    }
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ E-mail!")]
        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Display(Name = "Địa chỉ E-mail!")]
        public string Email { get; set; }
    }
}
