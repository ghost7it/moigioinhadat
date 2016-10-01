using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    [Table("AlbumDetail")]
    public class AlbumDetail : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn album!")]
        [Display(Name = "Chọn album")]
        public long AlbumId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề chính!")]
        [Display(Name = "Tiêu đề chính")]
        [StringLength(100, ErrorMessage = "Tiêu đề chính không được vượt quá 100 ký tự!")]
        public string Title { get; set; }

        [Display(Name = "Tiêu đề phụ")]
        [StringLength(100, ErrorMessage = "Tiêu đề phụ không được vượt quá 100 ký tự!")]
        public string SmallTitle { get; set; }

        [Display(Name = "Mô tả ảnh")]
        [StringLength(200, ErrorMessage = "Mô tả ảnh không được vượt quá 200 ký tự!")]
        public string Description { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Vui lòng chỉ nhập số nguyên dương!")]
        public int? OrdinalNumber { get; set; }

        [Display(Name = "Liên kết ảnh")]
        [StringLength(200, ErrorMessage = "Liên kết ảnh không được vượt quá 200 ký tự!")]
        public string Link { get; set; }

        [Display(Name = "Ảnh")]
        [Required(ErrorMessage = "Vui lòng chọn ảnh!")]
        [StringLength(200, ErrorMessage = "Đường dẫn ảnh không được vượt quá 200 ký tự!")]
        public string PhotoLocation { get; set; }

        //Ngày upload ảnh
        public DateTime UploadDate { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Album Album { get; set; }

        public string Describe()
        {
            return "{ AlbumDetailId : \"" + Id + "\", AlbumId : \"" + AlbumId + "\" }";
        }
    }
}
