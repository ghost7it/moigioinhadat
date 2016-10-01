using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục dân tộc
    /// </summary>
    [Table("Nation")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Nation : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dân tộc!")]
        [Display(Name = "Tên dân tộc")]
        [StringLength(100, ErrorMessage = "Tên dân tộc không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public string Describe()
        {
            return "{ NationId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
