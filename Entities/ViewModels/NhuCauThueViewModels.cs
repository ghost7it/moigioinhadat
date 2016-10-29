using System.ComponentModel.DataAnnotations;
namespace Entities.ViewModels
{
    public class NhuCauThueCreatingViewModel
    {
        public string KhachId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại mặt bằng")]
        [Display(Name = "Loại mặt bằng")]
        public string MatBangId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận")]
        [Display(Name = "Quận")]
        public string QuanId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đường")]
        [Display(Name = "Đường")]
        public string Duong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số nhà")]
        [Display(Name = "Số nhà")]
        public string SoNha { get; set; }

        [Display(Name = "Tên tòa nhà")]
        public string TenToaNha { get; set; }

        [Display(Name = "Mặt tiền treo biển")]
        public string MatTienTreoBien { get; set; }

        [Display(Name = "Bề ngang lọt lòng (m)")]
        public string BeNgangLotLong { get; set; }

        [Display(Name = "Diện tích đất (m)")]
        public string DienTichDat { get; set; }

        [Display(Name = "Diện tích đất sử dụng tầng 1 (m)")]
        public string DienTichDatSuDungTang1 { get; set; }

        [Display(Name = "Số tầng")]
        public string SoTang { get; set; }

        [Display(Name = "Tổng diện tích sử dụng")]
        public string TongDienTichSuDung { get; set; }

        [Display(Name = "Đi chung chủ")]
        public string DiChungChu { get; set; }

        [Display(Name = "Hầm")]
        public string Ham { get; set; }

        [Display(Name = "Thang máy")]
        public string ThangMay { get; set; }

        [Display(Name = "Nột thất/khách thuê cũ")]
        public string NoiThatKhachThueCuId { get; set; }

        [Display(Name = "Tổng giá thuê")]
        public string TongGiaThue { get; set; }

        [Display(Name = "Giá thuê BQ/m2")]
        public string GiaThueBQ { get; set; }

        [Display(Name = "Ngày CN hẹn liên hệ lại")]
        public string NgayCNHenLienHeLai { get; set; }

        [Display(Name = "Cấp độ theo dõi")]
        public string CapDoTheoDoiId { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public string NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }
    }
    public class NhuCauThueUpdatingViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại mặt bằng")]
        [Display(Name = "Loại mặt bằng")]
        public string MatBangId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận")]
        [Display(Name = "Quận")]
        public string QuanId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đường")]
        [Display(Name = "Đường")]
        public string Duong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số nhà")]
        [Display(Name = "Số nhà")]
        public string SoNha { get; set; }

        [Display(Name = "Tên tòa nhà")]
        public string TenToaNha { get; set; }

        [Display(Name = "Mặt tiền treo biển")]
        public string MatTienTreoBien { get; set; }

        [Display(Name = "Bề ngang lọt lòng (m)")]
        public string BeNgangLotLong { get; set; }

        [Display(Name = "Diện tích đất (m)")]
        public string DienTichDat { get; set; }

        [Display(Name = "Diện tích đất sử dụng tầng 1 (m)")]
        public string DienTichDatSuDungTang1 { get; set; }

        [Display(Name = "Số tầng")]
        public string SoTang { get; set; }

        [Display(Name = "Tổng diện tích sử dụng")]
        public string TongDienTichSuDung { get; set; }

        [Display(Name = "Đi chung chủ")]
        public string DiChungChu { get; set; }

        [Display(Name = "Hầm")]
        public string Ham { get; set; }

        [Display(Name = "Thang máy")]
        public string ThangMay { get; set; }

        [Display(Name = "Nột thất/khách thuê cũ")]
        public string NoiThatKhachThueCuId { get; set; }

        [Display(Name = "Tổng giá thuê")]
        public string TongGiaThue { get; set; }

        [Display(Name = "Giá thuê BQ/m2")]
        public string GiaThueBQ { get; set; }

        [Display(Name = "Ngày CN hẹn liên hệ lại")]
        public string NgayCNHenLienHeLai { get; set; }

        [Display(Name = "Cấp độ theo dõi")]
        public string CapDoTheoDoiId { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public string NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }
    }
}
