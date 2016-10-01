using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    [Table("Profile")]
    public class Profile : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn!")]
        [Display(Name = "Họ và tên")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Ảnh đại diện")]
        [StringLength(200, ErrorMessage = "Ảnh đại diện không được vượt quá 200 ký tự!")]
        public string ProfilePicture { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public bool? Sex { get; set; }

        [Display(Name = "Quê quán")]
        [StringLength(250, ErrorMessage = "Quê quán không được vượt quá 250 ký tự!")]
        public string NativeLand { get; set; }

        [Display(Name = "Nơi ở hiện tại")]
        [StringLength(250, ErrorMessage = "Nơi ở hiện tại không được vượt quá 250 ký tự!")]
        public string CurrentResidence { get; set; }

        [RegularExpression(@"^([\w\!\#$\%\&\'*\+\-\/\=\?\^`{\|\}\~]+\.)*[\w\!\#$\%\&\'‌​*\+\-\/\=\?\^`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[‌​a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Địa chỉ E-mail không hợp lệ!")]
        [Column(TypeName = "varchar")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        [Display(Name = "Địa chỉ email")]
        [StringLength(50, ErrorMessage = "Địa chỉ email không được vượt quá 50 ký tự!")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "Điện thoại di động")]
        [StringLength(20, ErrorMessage = "Điện thoại di động không được vượt quá 20 ký tự!")]
        public string CellPhone { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "Điện thoại nhà riêng")]
        [StringLength(20, ErrorMessage = "Điện thoại nhà riêng không được vượt quá 20 ký tự!")]
        public string HomePhone { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "Điện thoại cơ quan")]
        [StringLength(20, ErrorMessage = "Điện thoại cơ quan không được vượt quá 20 ký tự!")]
        public string OfficePhone { get; set; }

        [Display(Name = "Thông tin mở rộng")]
        [StringLength(1000, ErrorMessage = "Thông tin mở rộng không được vượt quá 1000 ký tự!")]
        public string ExtraInfomation { get; set; }

        [Display(Name = "Dân tộc")]
        public long? NationId { get; set; }
        [ForeignKey("NationId")]
        public virtual Nation Nation { get; set; }

        [Display(Name = "Tôn giáo")]
        public long? ReligionId { get; set; }
        [ForeignKey("ReligionId")]
        public virtual Religion Religion { get; set; }

        [Display(Name = "Cơ quan làm việc hiện tại")]
        public long? CurrentOrganizationId { get; set; }
        [ForeignKey("CurrentOrganizationId")]
        public virtual Organization CurrentOrganization { get; set; }

        [Display(Name = "Chức vụ hiện tại")]
        public long? CurrentPositionId { get; set; }
        [ForeignKey("CurrentPositionId")]
        public virtual Position CurrentPosition { get; set; }

        [Display(Name = "Học vị hiện tại")]
        public long? CurrentDegreeId { get; set; }
        [ForeignKey("CurrentDegreeId")]
        public virtual Degree CurrentDegree { get; set; }

        [Display(Name = "Học hàm hiện tại")]
        public long? CurrentRankId { get; set; }
        [ForeignKey("CurrentRankId")]
        public virtual Rank CurrentRank { get; set; }

        [Display(Name = "Quốc tịch")]
        public long? CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        //NativeLand
        [Display(Name = "Tỉnh/TP (quê quán)")]
        public long? NativeLandProvinceId { get; set; }
        [ForeignKey("NativeLandProvinceId")]
        public virtual Province NativeLandProvince { get; set; }

        [Display(Name = "Quận/Huyện (quê quán)")]
        public long? NativeLandDistrictId { get; set; }
        [ForeignKey("NativeLandDistrictId")]
        public virtual District NativeLandDistrict { get; set; }

        //CurrentResidence
        [Display(Name = "Tỉnh/TP (nơi ở hiện tại)")]
        public long? CurrentResidenceProvinceId { get; set; }
        [ForeignKey("CurrentResidenceProvinceId")]
        public virtual Province CurrentResidenceProvince { get; set; }

        [Display(Name = "Quận/Huyện (nơi ở hiện tại)")]
        public long? CurrentResidenceDistrictId { get; set; }
        [ForeignKey("CurrentResidenceDistrictId")]
        public virtual District CurrentResidenceDistrict { get; set; }


        [Display(Name = "Ngày đăng ký")]//Ngày đăng ký hoặc ngày được admin nhập
        public DateTime CreateDate { get; set; }
        public long? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdateBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public long? ApproveBy { get; set; }
        public DateTime? UnApproveDate { get; set; }
        public long? UnApproveBy { get; set; }

        //[Display(Name = "Trạng thái hồ sơ")]//1: Mới đăng ký/mới nhập; 2: Đã duyệt; 3: Không được duyệt
        /// <summary>
        /// Trạng thái hồ sơ" //1: Mới đăng ký/mới nhập; 2: Đã duyệt; 3: Không được duyệt
        /// </summary>
        [Column(TypeName = "tinyint")]
        public byte Status { get; set; }

        //Lý do không duyệt
        public string UnApprovedMessage { get; set; }

        //public long? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public virtual ICollection<ProfileOrganization> ProfileOrganizations { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", Name : \"" + Name + "\", { Email : \"" + Email + "\", { CellPhone : \"" + CellPhone + "\" }";
        }
    }
}
