using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Models
{
    /// <summary>
    /// Bài viết thuộc chuyên mục nào
    /// </summary>
    [Table("ArticleCategory")]
    public class ArticleCategory : Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        //Bài viết
        public long ArticleId { get; set; }
        //Chuyên mục
        public long CategoryId { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public string Describe()
        {
            return "{ ArticleId : \"" + ArticleId + "\", CategoryId : \"" + CategoryId + "\" }";
        }
    }
}
