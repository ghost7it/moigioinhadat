using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Phân quyền tài khoản đối với 1 trường (đơn vị)
    /// </summary>
    [Table("AccountOrganization")]
    public class AccountOrganization : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        //Người dùng
        public long AccountId { get; set; }
        //Trường
        public long OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        public string Describe()
        {
            return "{ AccountId : \"" + AccountId + "\", OrganizationId : \"" + OrganizationId + "\" }";
        }
    }
}
