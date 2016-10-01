using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Loại chuyên mục: Chuyên mục chính, chuyên mục nhỏ bên phải, chuyên mục cuối trang
    /// </summary>
    [Table("CategoryType")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class CategoryType : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại chuyên mục!")]
        [Display(Name = "Tên loại chuyên mục")]
        [StringLength(100, ErrorMessage = "Tên loại chuyên mục không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả loại chuyên mục không được vượt quá 500 ký tự!")]
        public string Description { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public string Describe()
        {
            return "{ CategoryTypeId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
