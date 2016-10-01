using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục chức vụ
    /// </summary>
    [Table("Position")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Position : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chức vụ!")]
        [Display(Name = "Tên chức vụ")]
        [StringLength(100, ErrorMessage = "Tên chức vụ không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        //Đã được duyệt hay chưa. Chỉ dùng trong trường hợp chức vụ là do người dùng (cựu sinh viên) nhập
        public bool IsApproved { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public string Describe()
        {
            return "{ PositionId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
