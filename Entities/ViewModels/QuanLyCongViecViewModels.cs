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

    public class ListFieldHidden
    {
        public List<FieldHidden> lst { get; set; }
        public ListFieldHidden()
        {
            lst = new List<FieldHidden>();
            lst.Add(new FieldHidden { FieldKey = "SoNha", FieldName = "Số nhà", IsSelected = false, IsNha = true, IsNhuCau = false });
            lst.Add(new FieldHidden { FieldKey = "TenToaNha", FieldName = "Tên tòa nhà", IsSelected = false, IsNha = true, IsNhuCau = false });
            lst.Add(new FieldHidden { FieldKey = "MatTienTreoBien", FieldName = "Mặt tiền treo biển", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "BeNgangLotLong", FieldName = "Bề ngang lọt lòng", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "DienTichDat", FieldName = "Diện tích đất", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "DienTichDatSuDungTang1", FieldName = "Diện tích đất sử dụng tầng 1", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "TongDienTichSuDung", FieldName = "Tổng diện tích sử dụng", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "SoTang", FieldName = "Số tầng", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "DiChungChu", FieldName = "Đi chung chủ", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "Ham", FieldName = "Hầm", IsSelected = false, IsNha = true, IsNhuCau = true });
            lst.Add(new FieldHidden { FieldKey = "ThangMay", FieldName = "Thang máy", IsSelected = false, IsNha = true, IsNhuCau = true });
        }
    }

    public static class ListOfFieldHidden
    {
        public static void Add(this List<FieldHidden> list,
            string fieldId, string fieldName, bool isNha, bool isSelected, bool isNhuCau )
        {
            if (null == list)
                throw new NullReferenceException();

            var emailData = new FieldHidden
            {
                FieldKey = fieldId,
                FieldName = fieldName,
                IsNha = isNha,
                IsNhuCau = isNhuCau,
                IsSelected = isSelected
            };
            list.Add(emailData);
        }
    }
}
