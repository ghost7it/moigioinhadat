using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục tỉnh/thành phố
    /// </summary>
    [Table("Province")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Province : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tỉnh/thành phố!")]
        [Display(Name = "Tên tỉnh/thành phố")]
        [StringLength(100, ErrorMessage = "Tên tỉnh/thành phố không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        public virtual ICollection<District> Districts { get; set; }

        public string Describe()
        {
            return "{ ProvinceId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
