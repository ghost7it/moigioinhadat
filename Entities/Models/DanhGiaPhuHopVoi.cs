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
    /// Danh mục Đánh giá phù hợp với
    /// </summary>
    [Table("DanhGiaPhuHopVoi")]
    [DropDown(ValueField = "Id", TextField = "DanhGiaPhuHopVoi")]
    public class DanhGiaPhuHopVoi : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Đánh giá phù hợp với")]
        public string Name { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
