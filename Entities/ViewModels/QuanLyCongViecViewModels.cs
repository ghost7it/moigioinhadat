using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels
{
    public class QuanLyCongViecViewModel
    {
        public long Id { get; set; }

        [Display(Name = "Nhân viên phụ trách")]
        public long NhanVienPhuTrachId { get; set; }

        [Display(Name = "Khách")]
        public long KhachId { get; set; }

        [Display(Name = "Nhà")]
        public long NhaId { get; set; }

        [Display(Name = "Nhu cầu thuê")]
        public long NhuCauThueId { get; set; }

        [Display(Name = "Nội dung công việc")]
        public string NoiDungCongViec { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Người tạo")]
        public long NguoiTaoId { get; set; }

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        [Display(Name = "Ẩn thông tin nhà")]
        public string NhaHiddenField { get; set; }

        [Display(Name = "Ẩn thông tin nhu cầu")]
        public string NhuCauHiddenField { get; set; }

        [Display(Name = "Ẩn thông tin khách")]
        public string KhachHiddenField { get; set; }

        [Display(Name = "Các trường dữ liệu nhà cần ẩn")]
        public string NhaField { get; set; }

        [Display(Name = "Các trường dữ liệu khách cần ẩn")]
        public string KhachField { get; set; }

        public List<FieldHidden> FieldHidden { get; set; }
    }

    public class FieldHidden{
        public string FieldKey { get; set; }
        public string FieldName{ get; set; }
        public bool IsSelected { get; set; }

        public bool IsNha { get; set; }
        public bool IsNhuCau { get; set; }

    }
}
