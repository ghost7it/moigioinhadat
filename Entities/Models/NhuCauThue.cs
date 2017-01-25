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
    /// Danh mục Nhu cầu thuê của khách
    /// </summary>
    [Table("NhuCauThue")]
    public class NhuCauThue : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long KhachId { get; set; }

        [Display(Name = "Loại mặt bằng")]
        public string MatBangId { get; set; }

        //[Display(Name = "Loại mặt bằng array")]
        //public string MatBangIdArr { get; set; }

        [Display(Name = "Quận")]
        public long QuanId { get; set; }

        [Display(Name = "Tên Quận")]
        public string QuanName { get; set; }

        [Display(Name = "Đường")]
        public long DuongId { get; set; }

        [Display(Name = "Tên Đường")]
        public string DuongName { get; set; }

        [Display(Name = "Số nhà")]
        public string SoNha { get; set; }

        [Display(Name = "Tên tòa nhà")]
        public string TenToaNha { get; set; }

        [Display(Name = "Mặt tiền treo biển")]
        public float MatTienTreoBien { get; set; }

        [Display(Name = "Bề ngang lọt lòng (m)")]
        public float BeNgangLotLong { get; set; }

        [Display(Name = "Diện tích đất (m)")]
        public float DienTichDat { get; set; }

        [Display(Name = "Diện tích đất sử dụng tầng 1 (m)")]
        public float DienTichDatSuDungTang1 { get; set; }

        [Display(Name = "Số tầng")]
        public int SoTang { get; set; }

        [Display(Name = "Tổng diện tích sử dụng")]
        public float TongDienTichSuDung { get; set; }

        [Display(Name = "Đi chung chủ")]
        public bool DiChungChu { get; set; }

        [Display(Name = "Hầm")]
        public bool Ham { get; set; }

        [Display(Name = "Thang máy")]
        public bool ThangMay { get; set; }

        [Display(Name = "Nột thất/khách thuê cũ")]
        public int NoiThatKhachThueCuId { get; set; }

        [Display(Name = "Tổng giá thuê")]
        public decimal TongGiaThue { get; set; }

        [Display(Name = "Giá thuê BQ/m2")]
        public decimal GiaThueBQ { get; set; }

        [Display(Name = "Ngày CN hẹn liên hệ lại")]
        public DateTime? NgayCNHenLienHeLai { get; set; }

        [Display(Name = "Cấp độ theo dõi")]
        public int CapDoTheoDoiId { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public long NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } //0: Chờ duyệt | 1: Đã duyệt

        public long NguoiPhuTrachId { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Id + "\" }";
        }
    }
}
