using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Tài khoản người dùng 
    /// </summary>
    [Table("Account")]
    public class Account : Entity
    {
        [Key]
        //[Column(Order = 0), Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên!")]
        [Display(Name = "Họ và tên")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự!")]
        public string Name { get; set; }

        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Display(Name = "Địa chỉ E-mail!")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        [Column(TypeName = "varchar")]
        [StringLength(50, ErrorMessage = "Địa chỉ E-mail không được vượt quá 50 ký tự!")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên và ít hơn 32 ký tự!")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [Column(TypeName = "varchar")]
        public string Password { get; set; }

        //Là tài khoản quản trị nội dung
        public bool IsManageAccount { get; set; }
        //Là tài khoản trang ngoài
        public bool IsNormalAccount { get; set; }

        //Ngày tạo
        public DateTime CreateDate { get; set; }

        [StringLength(200, ErrorMessage = "Đường dẫn ảnh đại diện không được vượt quá 200 ký tự!")]
        [Display(Name = "Ảnh đại diện")]
        public string ProfilePicture { get; set; }

        [StringLength(20, ErrorMessage = "Điện thoại không được vượt quá 20 ký tự!")]
        [Column(TypeName = "varchar")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        //

        //[Column(Order = 1), Key, ForeignKey("Profile")]
        //public long? ProfileId { get; set; }
        //[ForeignKey("ProfileId")]
        //[ForeignKey("Id")]
        //[Column(Order = 1), Key, ForeignKey("Id")]
        public virtual Profile Profile { get; set; }

        //Important - always use ICollection, not IEnumerable for Navigation properties and make them virtual - thanks to this ef will be able to track changes.
        public virtual ICollection<AccountRole> AccountRoles { get; set; }
        public virtual ICollection<AccountOrganization> AccountOrganizations { get; set; }
        public virtual ICollection<RespondFeedback> RespondFeedbacks { get; set; }

        public string Describe()
        {
            return "{ AccountId : \"" + Id + "\", Name : \"" + Name + "\", { Email : \"" + Email + "\" }";
        }
    }
}
