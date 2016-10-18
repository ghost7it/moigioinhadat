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
    /// Danh mục Cấp độ theo dõi
    /// </summary>
    [Table("CapDoTheoDoi")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class CapDoTheoDoi : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Cấp độ theo dõi")]
        public string Name { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
