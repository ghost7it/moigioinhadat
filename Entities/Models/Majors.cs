using Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Danh mục ngành học
    /// </summary>
    [Table("Majors")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Majors : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Mã ngành")]
        [StringLength(10, ErrorMessage = "Mã ngành không được vượt quá 10 ký tự!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngành!")]
        [Display(Name = "Tên ngành")]
        [StringLength(100, ErrorMessage = "Tên ngành không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        //Đã được duyệt hay chưa. Chỉ dùng trong trường hợp ngành học là do người dùng (cựu sinh viên) nhập
        public bool IsApproved { get; set; }

        //Ngành học này thuộc trường nào
        [Display(Name = "Chọn đơn vị")]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị!")]
        public long OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        //Ngành học này thuộc khoa hay bộ môn nào, là cấp con của Trường (đơn vị)
        [Display(Name = "Thuộc khoa/bộ môn")]
        public long? SubordinatedOrganizationId { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<ProfileOrganization> ProfileOrganizations { get; set; }

        public string Describe()
        {
            return "{ MajorsId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
