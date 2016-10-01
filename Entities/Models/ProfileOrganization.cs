using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    [Table("ProfileOrganization")]
    public class ProfileOrganization : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

        //Trường (đơn vị)
        [Display(Name = "Trường")]
        public long OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        //Năm bắt đầu học
        [Display(Name = "Năm nhập học")]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Vui lòng chỉ nhập số nguyên dương!")]
        public int? StartYear { get; set; }

        //Năm tốt nghiệp
        [Display(Name = "Năm tốt nghiệp")]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Vui lòng chỉ nhập số nguyên dương!")]
        public int? GraduateYear { get; set; }

        [Display(Name = "Ngành học")]
        public long? MajorsId { get; set; }
        [ForeignKey("MajorsId")]
        public virtual Majors Majors { get; set; }

        [Display(Name = "Khóa học")]
        public long? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        //Là thành viên ban liên lạc. Nếu được đánh dấu, người này sẽ có quyền duyệt hồ sơ (phải được phân quyền duyệt)
        public bool IsLiaisonCommittee { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", ProfileId : \"" + ProfileId + "\", { OrganizationId : \"" + OrganizationId + "\" }";
        }
    }
}
