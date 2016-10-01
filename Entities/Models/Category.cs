using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Chuyên mục
    /// </summary>
    [Table("Category")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Category : Entity
    {
        [NotMapped]
        public bool IsNullOrEmpty { get; set; }
        public Category NullCategory()
        {
            var nullCategory = new Category();
            nullCategory.IsNullOrEmpty = true;
            return nullCategory;
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chuyên mục!")]
        [Display(Name = "Tên chuyên mục")]
        [StringLength(100, ErrorMessage = "Tên chuyên mục không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Chuyên mục cha")]
        public long? CategoryId { get; set; }

        [Display(Name = "Mô tả chuyên mục")]
        [StringLength(500, ErrorMessage = "Mô tả chuyên mục không được vượt quá 500 ký tự!")]
        public string Description { get; set; }

        [Display(Name = "Ảnh mô tả lớn")]
        [StringLength(200, ErrorMessage = "Đường dẫn ảnh mô tả chuyên mục không được vượt quá 200 ký tự!")]
        public string DescriptionIcon { get; set; }

        [Display(Name = "Ảnh mô tả nhỏ")]
        [StringLength(200, ErrorMessage = "Đường dẫn ảnh mô tả chuyên mục không được vượt quá 200 ký tự!")]
        public string SmallIcon { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Vui lòng chỉ nhập số nguyên dương!")]
        public int? OrdinalNumber { get; set; }

        [Display(Name = "Link chuyên mục")]
        [StringLength(200, ErrorMessage = "Link chuyên mục không được vượt quá 200 ký tự!")]
        public string Link { get; set; }

        [Display(Name = "Loại chuyên mục")]
        public long? CategoryTypeId { get; set; }

        //=1: Hiển thị mặc định (danh sách bài viết); =2: Hiển thị danh sách chuyên mục con

        [Display(Name = "Cách hiển thị chuyên mục")]
        [Column(TypeName = "tinyint")]
        public byte? DisplayType { get; set; }

        [ForeignKey("CategoryTypeId")]
        public virtual CategoryType CategoryType { get; set; }

        public virtual ICollection<ArticleCategory> ArticleCategories { get; set; }

        //Chuyên mục cha nếu có
        [ForeignKey("CategoryId")]
        public virtual Category ParentCategory { get; set; }
        //Danh sách chuyên mục con nếu có
        public virtual ICollection<Category> Categories { get; set; }

        public string Describe()
        {
            return "{ CategoryId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
