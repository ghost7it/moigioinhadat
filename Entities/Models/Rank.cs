using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục học hàm
    /// </summary>
    [Table("Rank")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Rank : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Mã học hàm")]
        [StringLength(10, ErrorMessage = "Mã học hàm không được vượt quá 10 ký tự!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên học hàm!")]
        [Display(Name = "Tên học hàm")]
        [StringLength(100, ErrorMessage = "Tên học hàm không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public string Describe()
        {
            return "{ RankId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
