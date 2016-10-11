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
    /// Lịch sửa update nhà
    /// </summary>
    [Table("NhaHistory")]
    public class NhaHistory : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Ngày thay đổi")]
        public long NhaId { get; set; }

        [Display(Name = "Ngày thay đổi")]
        public DateTime NgayThayDoi { get; set; }

        [Display(Name = "Người thay đổi")]
        public long NguoiThayDoi { get; set; }

        [Display(Name = "Nội dung thay đổi")]
        public string NoiDungThayDoi { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Id + "\" }";
        }
    }
}
