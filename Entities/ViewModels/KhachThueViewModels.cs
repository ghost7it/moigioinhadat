using System.ComponentModel.DataAnnotations;
namespace Entities.ViewModels
{
    public class KhachThueCreatingViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên khách")]
        [Display(Name = "Tên khách")]
        public string TenKhach { get; set; }

        [Display(Name = "Tên người liên hệ - vai trò")]
        public string TenNguoiLienHeVaiTro { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
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
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public string NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }
    }
    public class KhachThueUpdatingViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách")]
        [Display(Name = "Tên khách")]
        public string TenKhach { get; set; }

        [Display(Name = "Tên người liên hệ - vai trò")]
        public string TenNguoiLienHeVaiTro { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
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
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Ngày không hợp lệ!")]
        public string NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public string NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }
    }
}
