using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục học vị 
    /// </summary>
    [Table("Degree")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Degree : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Mã học vị")]
        [StringLength(10, ErrorMessage = "Mã học vị không được vượt quá 10 ký tự!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên học vị!")]
        [Display(Name = "Tên học vị")]
        [StringLength(100, ErrorMessage = "Tên học vị không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public string Describe()
        {
            return "{ DegreeId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
