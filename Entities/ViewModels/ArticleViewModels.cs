using System.ComponentModel.DataAnnotations;
namespace Entities.ViewModels
{
    public class ArticleCreatingViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài viết!")]
        [Display(Name = "Tiêu đề bài viết")]
        [StringLength(200, ErrorMessage = "Tiêu đề bài viết không được vượt quá 200 ký tự!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung trích dẫn!")]
        [Display(Name = "Nội dung trích dẫn")]
        [StringLength(1000, ErrorMessage = "Nội dung trích dẫn không được vượt quá 1000 ký tự!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung chi tiết!")]
        [Display(Name = "Nội dung chi tiết")]
        public string Content { get; set; }

        [Display(Name = "Ảnh mô tả")]
        [StringLength(200, ErrorMessage = "Ảnh mô tả không được vượt quá 200 ký tự!")]
        public string ImageDescription { get; set; }

        [Display(Name = "Ngày bắt đầu sự kiện")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string EventStartDate { get; set; }

        [Display(Name = "Ngày kết thúc sự kiện")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string EventFinishDate { get; set; }

        public string[] CategoryId { get; set; }
    }
    public class ArticleUpdatingViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài viết!")]
        [Display(Name = "Tiêu đề bài viết")]
        [StringLength(200, ErrorMessage = "Tiêu đề bài viết không được vượt quá 200 ký tự!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung trích dẫn!")]
        [Display(Name = "Nội dung trích dẫn")]
        [StringLength(1000, ErrorMessage = "Nội dung trích dẫn không được vượt quá 1000 ký tự!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung chi tiết!")]
        [Display(Name = "Nội dung chi tiết")]
        public string Content { get; set; }

        [Display(Name = "Ảnh mô tả")]
        [StringLength(200, ErrorMessage = "Ảnh mô tả không được vượt quá 200 ký tự!")]
        public string ImageDescription { get; set; }

        [Display(Name = "Ngày bắt đầu sự kiện")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string EventStartDate { get; set; }

        [Display(Name = "Ngày kết thúc sự kiện")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string EventFinishDate { get; set; }

        public string[] CategoryId { get; set; }
    }
}
