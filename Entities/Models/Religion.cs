using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục tôn giáo
    /// </summary>
    [Table("Religion")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Religion : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tôn giáo!")]
        [Display(Name = "Tên tôn giáo")]
        [StringLength(100, ErrorMessage = "Tên tôn giáo không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public string Describe()
        {
            return "{ ReligionId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
