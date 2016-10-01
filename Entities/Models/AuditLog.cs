using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    [Table("AuditLog")]
    public class AuditLog : Entity
    {
        [Key]
        public Guid AuditLogId { get; set; }

        [Required]
        public long AccountId { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(1)]//CRUD
        public string EventType { get; set; }

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; }

        [Required]
        public long RecordId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public string Describe()
        {
            return "Logging";
        }
    }
}
