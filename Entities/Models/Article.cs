using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Bài viết 
    /// </summary>
    [Table("Article")]
    public class Article : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
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

        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdateBy { get; set; }
        public DateTime? PublishDate { get; set; }
        public long? PublishBy { get; set; }

        //[Display(Name = "Trạng thái bài viết")] //(Duyệt/Chờ xuất bản/Xuất bản/Hủy xuất bản): Dùng enum ArticleStatusEnum
        [Column(TypeName = "tinyint")]
        public byte Status { get; set; }

        [Display(Name = "Ngày bắt đầu sự kiện")]
        public DateTime? EventStartDate { get; set; }
        [Display(Name = "Ngày kết thúc sự kiện")]
        public DateTime? EventFinishDate { get; set; }

        //Lượt xem
        public long Views { get; set; }

        public virtual ICollection<ArticleCategory> ArticleCategories { get; set; }

        public string Describe()
        {
            return "{ ArticleId : \"" + Id + "\", Title : \"" + Title + "\" }";
        }
    }
}
