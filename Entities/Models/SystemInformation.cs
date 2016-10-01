using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Thông tin cấu hình hệ thống
    /// </summary>
    [Table("SystemInformation")]
    public class SystemInformation : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên trang web")]
        [Display(Name = "Tên trang web")]
        [StringLength(200, ErrorMessage = "Tên trang web không được vượt quá 200 ký tự!")]
        public string SiteName { get; set; }

        [Display(Name = "Khẩu hiệu")]
        [StringLength(250, ErrorMessage = "Khẩu hiệu không được vượt quá 250 ký tự!")]
        public string Slogan { get; set; }

        [Display(Name = "Ảnh logo")]
        [StringLength(200, ErrorMessage = "Đường dẫn ảnh logo không được vượt quá 200 ký tự!")]
        public string Logo { get; set; }

        [Display(Name = "Thông tin bản quyền (cuối trang)")]
        [StringLength(500, ErrorMessage = "Thông tin bản quyền không được vượt quá 500 ký tự!")]
        public string Copyright { get; set; }

        [Display(Name = "Địa chỉ Email")]
        [StringLength(50, ErrorMessage = "Địa chỉ email không được vượt quá 50 ký tự!")]
        [Column(TypeName = "varchar")]
        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        public string Email { get; set; }

        //SMTP
        [Display(Name = "Địa chỉ email SMTP")]
        [Column(TypeName = "varchar")]
        [StringLength(50, ErrorMessage = "Địa chỉ email SMTP không được vượt quá 50 ký tự!")]
        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        public string SMTPEmail { get; set; }

        [Display(Name = "Mật khẩu email SMTP")]
        [StringLength(32, ErrorMessage = "Mật khẩu email SMTP không được vượt quá 32 ký tự!")]
        [Column(TypeName = "varchar")]
        public string SMTPPassword { get; set; }

        [Display(Name = "Tên email SMTP")]
        [StringLength(100, ErrorMessage = "Tên email SMTP không được vượt quá 100 ký tự!")]
        public string SMTPName { get; set; }


        //=================Site map

        //Menu chính
        public long? MainMenu { get; set; }

        //Menu con bên phải (bên cạnh menu chính)
        public long? RightMenu { get; set; }

        //Album để hiển thị slide show ở trang chủ
        public long? HomeAlbum { get; set; }

        //Album hiển thị 4 box con ở trang chủ bên cạnh slide
        public long? RightAlbum { get; set; }

        //Khối tin chính ở bên trái, trang chủ
        public long? LeftArticle1 { get; set; }

        //Khối tin chính thứ 2 ở bên trái, trang chủ
        public long? LeftArticle2 { get; set; }

        //Khối tin chính thứ 3 ở bên trái, trang chủ
        public long? LeftArticle3 { get; set; }

        //Khối tin bên phải thứ nhất
        public long? RightArticle1 { get; set; }

        //Khối tin bên phải thứ hai
        public long? RightArticle2 { get; set; }

        //Khối tin bên phải thứ ba
        public long? RightArticle3 { get; set; }

        //Album ảnh các đối tác
        public long? BottomAlbum { get; set; }

        //Danh sách liên kết cuối trang
        public long? BottomMenu { get; set; }

        public string Describe()
        {
            return "SystemInformation";
        }
    }
}
