using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    /// <summary>
    /// Danh mục Nội thất khách thuê cũ
    /// </summary>
    [Table("NoiThatKhachThueCu")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class NoiThatKhachThueCu : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Nội thất khách thuê cũ")]
        public string Name { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
