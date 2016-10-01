using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục quốc gia
    /// </summary>
    [Table("Country")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Country : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "Mã quốc gia")]
        [StringLength(5, ErrorMessage = "Mã quốc gia không được vượt quá 5 ký tự!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên quốc gia!")]
        [Display(Name = "Tên quốc gia")]
        [StringLength(100, ErrorMessage = "Tên quốc gia không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        public string Describe()
        {
            return "{ CountryId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
