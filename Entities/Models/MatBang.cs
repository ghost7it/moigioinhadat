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
    /// Danh mục loại mặt bằng
    /// </summary>
    [Table("MatBang")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class MatBang : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Loại mặt bằng")]
        public string Name { get; set; }

        public string Describe()
        {
            return "{ MatBangId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
