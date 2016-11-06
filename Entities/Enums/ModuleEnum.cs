using Common.CustomAttributes;
using System.ComponentModel;
namespace Entities.Enums
{
    /// <summary>
    /// Danh sách mã chức năng của hệ thống
    /// </summary>
    public enum ModuleEnum
    {
        #region -- Nhóm chức năng quản trị hệ thống, bắt đầu từ 100 --
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
        #endregion

        #region -- Nhóm chức năng quản trị nội dung, bắt đầu từ 200 --
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
        #endregion

        #region -- Nhóm chức năng quản lý nhà, bắt đầu từ 250 --
        [ModuleGroupAttribute(ModuleGroupCode = 3, ModuleGroupName = "Quản lý nhà cho thuê")]
        [Description("Quản lý dữ liệu nhà")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        Nha = 250,
        [ModuleGroupAttribute(ModuleGroupCode = 3, ModuleGroupName = "Quản lý nhà cho thuê")]
        [Description("Phân công công việc")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        PhanCongNha = 251,

        #region -- Nhóm chức năng quản lý khách, bắt đầu từ 300 --
        [ModuleGroupAttribute(ModuleGroupCode = 4, ModuleGroupName = "Quản lý khách thuê")]
        [Description("Quản lý dữ liệu khách")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        Khach = 300,
        [ModuleGroupAttribute(ModuleGroupCode = 4, ModuleGroupName = "Quản lý khách thuê")]
        [Description("Phân công công việc")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        PhanCongKhach = 301,
        #endregion

        #region -- Nhóm chức năng nhu cầu thuê, bắt đầu từ 350 --
        [ModuleGroupAttribute(ModuleGroupCode = 5, ModuleGroupName = "Quản lý nhu cầu thuê")]
        [Description("Quản lý dữ liệu nhu cầu thuê")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        NhuCauThue = 351,
        [ModuleGroupAttribute(ModuleGroupCode = 5, ModuleGroupName = "Quản lý khách thuê")]
        [Description("Phân công công việc")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        PheDuyetNhuCauThue = 352,
        #endregion

        #region -- Nhóm chức năng quản lý công việc, bắt đầu từ 400 --
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Nhận thông báo khi có dữ liệu nhà, khách mới")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        NhanThongBao = 400,
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Nhận mail nhắc nhở công việc cho nhân viên")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        GuiMailNhacViec = 401,
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Danh sách công việc")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        DanhSachCongViec = 402,
        #endregion

        #region -- Nhóm chức năng quản lý dữ liệu, bắt đầu từ 450 --
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Import dữ liệu")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        Import = 450,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Export dữ liệu")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        Export = 451,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Xóa nhà trùng lặp")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        XoaNhaTrung = 452,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Xóa khách trùng lặp")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        XoaKhachTrungLap = 453,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Chuyển giao dữ liệu")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        ChuyenGiaoDuLieu = 454
        #endregion
        #endregion
    }
}
