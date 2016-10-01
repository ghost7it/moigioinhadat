using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    [Table("RespondFeedback")]
    public class RespondFeedback : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung trả lời!")]
        [Display(Name = "Nội dung trả lời")]
        [StringLength(2000, ErrorMessage = "Nội dung trả lời không được vượt quá 2000 ký tự!")]
        public string Content { get; set; }

        public long AccountId { get; set; }

        public long FeedbackId { get; set; }

        public DateTime RespondDate { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("FeedbackId")]
        public virtual Feedback Feedback { get; set; }

        public string Describe()
        {
            return "{ Id : \"" + Id + "\", AccountId : \"" + AccountId + "\", { FeedbackId : \"" + FeedbackId + "\", { Content : \"" + Content + "\" }";
        }
    }
}
