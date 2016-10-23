using Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục đường
    /// </summary>
    [Table("Duong")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Duong : Entity
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đường")]
        [Display(Name = "Tên đường")]
        [StringLength(100, ErrorMessage = "Tên đường không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [Display(Name = "Chọn quận/huyện")]
        public long QuanId { get; set; }
        [ForeignKey("QuanId")]
        public virtual Quan Quan { get; set; }

        public string Describe()
        {
            return "{ DuongId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
