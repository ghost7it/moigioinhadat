using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    [Table("Feedback")]
    public class Feedback : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề!")]
        [Display(Name = "Tiêu đề")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung!")]
        [Display(Name = "Nội dung")]
        [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự!")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn!")]
        [Display(Name = "Họ và tên")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự!")]
        public string Name { get; set; }

        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Column(TypeName = "varchar")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        [Display(Name = "Địa chỉ email")]
        [StringLength(50, ErrorMessage = "Địa chỉ email không được vượt quá 50 ký tự!")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "Số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự!")]
        public string PhoneNumber { get; set; }

        public DateTime FeedbackDate { get; set; }

        //Phản hổi này đã trả lời hay chưa
        public bool IsResponded { get; set; }

        public virtual ICollection<RespondFeedback> RespondFeedbacks { get; set; }

        public string Describe()
        {
            return "{ Title : \"" + Title + "\", Name : \"" + Name + "\", { Email : \"" + Email + "\", { PhoneNumber : \"" + PhoneNumber + "\" }";
        }
    }
}
