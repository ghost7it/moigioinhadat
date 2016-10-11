using Common.CustomAttributes;
using System.ComponentModel;
namespace Entities.Enums
{
    /// <summary>
    /// Danh sách mã chức năng của hệ thống
    /// </summary>
    public enum ModuleEnum
    {
        //Nhóm chức năng quản trị hệ thống, bắt đầu từ 100
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Quản lý tài khoản")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Account = 100,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Quản lý nhóm quyền")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Role = 101,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Tham số hệ thống")]
        [ActionAttribute(ActionType.Read, ActionType.Update)]
        SystemInformation = 102,

        //Chuyên mục
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Quản lý loại chuyên mục")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        CategoryType = 103,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Quản lý chuyên mục")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Category = 104,

        //Danh mục dữ liệu
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục quốc gia")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Country = 105,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục dân tộc")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Nation = 106,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục tôn giáo")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Religion = 107,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục học vị")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Degree = 108,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục học hàm")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Rank = 109,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục chức vụ")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Position = 110,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục tỉnh/tp")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Province = 111,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục đơn vị")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Organization = 112,
        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Danh mục ngành/khóa")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Majors = 113,

        [ModuleGroupAttribute(ModuleGroupCode = 1, ModuleGroupName = "Quản trị hệ thống")]
        [Description("Thông tin phản hồi")]
        [ActionAttribute(ActionType.Read, ActionType.Update, ActionType.Delete)]
        Feedback = 114,


        //Nhóm chức năng quản trị nội dung, bắt đầu từ 200
        [ModuleGroupAttribute(ModuleGroupCode = 2, ModuleGroupName = "Quản trị nội dung")]
        [Description("Quản lý bài viết")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Article = 200,
        [ModuleGroupAttribute(ModuleGroupCode = 2, ModuleGroupName = "Quản trị nội dung")]
        [Description("Xuất bản bài viết")]
        [ActionAttribute(ActionType.Read, ActionType.Update, ActionType.Verify, ActionType.Publish)]
        ArticlePublish = 201,
        [ModuleGroupAttribute(ModuleGroupCode = 2, ModuleGroupName = "Quản trị nội dung")]
        [Description("Quản lý album ảnh")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Album = 203,


        //Nhóm chức năng quản lý hồ sơ cựu sinh viên, bắt đầu từ 250
        [ModuleGroupAttribute(ModuleGroupCode = 3, ModuleGroupName = "Quản lý hồ sơ cựu sinh viên")]
        [Description("Quản lý hồ sơ")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        Profile = 250,
        [ModuleGroupAttribute(ModuleGroupCode = 3, ModuleGroupName = "Quản lý hồ sơ cựu sinh viên")]
        [Description("Phê duyệt hồ sơ")]
        [ActionAttribute(ActionType.Read, ActionType.Update, ActionType.Verify)]
        ProfileApprove = 251,

        [ModuleGroupAttribute(ModuleGroupCode = 4, ModuleGroupName = "Ban liên lạc")]
        [Description("Phê duyệt hồ sơ thành viên")]
        [ActionAttribute(ActionType.Read, ActionType.Verify)]
        ProfileApproveCommittee = 252,
    }
}
