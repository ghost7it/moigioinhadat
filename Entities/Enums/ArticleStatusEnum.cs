using System.ComponentModel;
namespace Entities.Enums
{
    /// <summary>
    /// Trạng thái bài viết
    /// </summary>
    public enum ArticleStatusEnum
    {
        [Description("Đang biên tập")]
        RecentlyAdded = 1,
        [Description("Đã gửi xuất bản")]
        PendingPublish = 2,
        [Description("Trả biên tập")]
        NotAllowed = 3,
        [Description("Đã xuất bản")]
        Publish = 4,
        [Description("Hủy xuất bản")]
        UnPublished = 5
    }
}
