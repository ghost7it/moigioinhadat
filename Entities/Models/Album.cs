using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Album ảnh  
    /// </summary>
    [Table("Album")]
    public class Album : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên album!")]
        [Display(Name = "Tên album")]
        [StringLength(100, ErrorMessage = "Tên album không được vượt quá 100 ký tự!")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự!")]
        public string Description { get; set; }

        //Ngày tạo
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<AlbumDetail> AlbumDetails { get; set; }
        public string Describe()
        {
            return "{ AlbumId : \"" + Id + "\", Name : \"" + Name + "\" }";
        }
    }
}
