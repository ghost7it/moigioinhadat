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
    /// Danh mục Khách
    /// </summary>
    [Table("Khach")]
    public class Khach : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Tên người liên hệ - vai trò")]
        public string TenNguoiLienHeVaiTro { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Lĩnh vực")]
        public string LinhVuc { get; set; }

        [Display(Name = "SP chính")]
        public string SPChinh { get; set; }

        [Display(Name = "Phân khúc")]
        public string PhanKhuc { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public long NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Id + "\" }";
        }
    }
}
