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
    /// Danh mục khóa học
    /// </summary>
    [Table("Course")]
    [DropDown(ValueField = "Id", TextField = "Name")]
    public class Course : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Display(Name = "Mã khóa học")]
        [StringLength(10, ErrorMessage = "Mã khóa học không được vượt quá 10 ký tự!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khóa học!")]
        [Display(Name = "Tên khóa học")]
        [StringLength(100, ErrorMessage = "Tên khóa học không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        //Đã được duyệt hay chưa. Chỉ dùng trong trường hợp khóa học học là do người dùng (cựu sinh viên) nhập
        public bool IsApproved { get; set; }

        //khóa học học này thuộc ngành nào
        [Display(Name = "Chọn ngành học")]
        [Required(ErrorMessage = "Vui lòng chọn ngành học!")]
        public long MajorsId { get; set; }
        [ForeignKey("MajorsId")]
        public virtual Majors Majors { get; set; }

        [Display(Name = "Năm bắt đầu khóa học")]
        public int? StartYear { get; set; }

        [Display(Name = "Năm kết thúc khóa học")]
        public int? FinishYear { get; set; }

        public virtual ICollection<ProfileOrganization> ProfileOrganizations { get; set; }

        public string Describe()
        {
            return "{ CourseId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
