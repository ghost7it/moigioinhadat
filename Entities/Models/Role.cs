using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Nhóm quyền
    /// </summary>
    [Table("Role")]
    public class Role : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhóm quyền!")]
        [Display(Name = "Tên nhóm quyền")]
        [StringLength(100, ErrorMessage = "Tên nhóm quyền không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Mô tả!")]
        [StringLength(200, ErrorMessage = "Mô tả nhóm quyền không được vượt quá 200 ký tự!")]
        public string Description { get; set; }

        public virtual ICollection<AccountRole> AccountRoles { get; set; }
        public virtual ICollection<ModuleRole> RoleModules { get; set; }

        public string Describe()
        {
            return "{ RoleId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
