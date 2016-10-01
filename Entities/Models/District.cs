using Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục quận/huyện
    /// </summary>
    [Table("District")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class District : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên quận/huyện!")]
        [Display(Name = "Tên quận/huyện")]
        [StringLength(100, ErrorMessage = "Tên quận/huyện không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố!")]
        [Display(Name = "Chọn tỉnh/thành phố")]
        public long ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }

        public string Describe()
        {
            return "{ DistrictId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
