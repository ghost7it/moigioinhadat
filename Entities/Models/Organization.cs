using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục đơn vị
    /// </summary>
    [Table("Organization")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Organization : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đơn vị!")]
        [Display(Name = "Tên đơn vị")]
        [StringLength(500, ErrorMessage = "Tên đơn vị không được vượt quá 500 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Chọn đơn vị cấp trên nếu có")]
        public long? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization ParentOrganization { get; set; }
        //Danh sách đơn vị con nếu có
        public virtual ICollection<Organization> Organizations { get; set; }


        public virtual ICollection<Majors> Majors { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }

        //Là đơn vị làm chủ hệ thống này. Mục đích khi cựu sinh viên đăng ký chỉ hiển thị các đơn vị này.
        public bool IsSystemOwner { get; set; }

        //Đã được duyệt hay chưa. Chỉ dùng trong trường hợp đơn vị là do người dùng (cựu sinh viên) nhập
        public bool IsApproved { get; set; }

        public virtual ICollection<AccountOrganization> AccountOrganizations { get; set; }

        public virtual ICollection<ProfileOrganization> ProfileOrganizations { get; set; }

        public string Describe()
        {
            return "{ OrganizationId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
