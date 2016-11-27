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

        #region -- Nhóm chức năng quản lý nhà, bắt đầu từ 250 --
        [ModuleGroupAttribute(ModuleGroupCode = 3, ModuleGroupName = "Quản lý nhà cho thuê")]
        [Description("Quản lý dữ liệu nhà")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        Nha = 250,
        //[ModuleGroupAttribute(ModuleGroupCode = 3, ModuleGroupName = "Quản lý nhà cho thuê")]
        //[Description("Phân công công việc")]
        //[ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        //PhanCongNha = 251,
        #endregion

        #region -- Nhóm chức năng quản lý khách, bắt đầu từ 300 --
        [ModuleGroupAttribute(ModuleGroupCode = 4, ModuleGroupName = "Quản lý khách thuê")]
        [Description("Quản lý dữ liệu khách")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        Khach = 300,
        //[ModuleGroupAttribute(ModuleGroupCode = 4, ModuleGroupName = "Quản lý khách thuê")]
        //[Description("Phân công công việc")]
        //[ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        //PhanCongKhach = 301,
        #endregion

        #region -- Nhóm chức năng nhu cầu thuê, bắt đầu từ 350 --
        [ModuleGroupAttribute(ModuleGroupCode = 5, ModuleGroupName = "Quản lý nhu cầu thuê")]
        [Description("Quản lý dữ liệu nhu cầu thuê")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        NhuCauThue = 351,
        //[ModuleGroupAttribute(ModuleGroupCode = 5, ModuleGroupName = "Quản lý khách thuê")]
        //[Description("Phân công công việc")]
        //[ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete)]
        //PheDuyetNhuCauThue = 352,
        #endregion

        #region -- Nhóm chức năng quản lý công việc, bắt đầu từ 400 --
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Nhận thông báo khi có dữ liệu nhà, khách mới")]
        [ActionAttribute(ActionType.Read)]
        NhanThongBao = 400,
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Nhận mail nhắc nhở công việc cho nhân viên")]
        [ActionAttribute(ActionType.Read)]
        GuiMailNhacViec = 401,
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Danh sách công việc")]
        [ActionAttribute(ActionType.Read, ActionType.Update)]
        DanhSachCongViec = 402,
        [ModuleGroupAttribute(ModuleGroupCode = 6, ModuleGroupName = "Quản lý công việc")]
        [Description("Phân công công việc")]
        [ActionAttribute(ActionType.Read, ActionType.Create, ActionType.Update, ActionType.Delete, ActionType.Verify)]
        PhanCongCongViec = 403,
        #endregion

        #region -- Nhóm chức năng quản lý dữ liệu, bắt đầu từ 450 --
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Import dữ liệu")]
        [ActionAttribute(ActionType.Read, ActionType.Create)]
        Import = 450,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Export dữ liệu")]
        [ActionAttribute(ActionType.Read, ActionType.Create)]
        Export = 451,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Xóa nhà trùng lặp")]
        [ActionAttribute(ActionType.Read, ActionType.Delete)]
        XoaNhaTrung = 452,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Xóa khách trùng lặp")]
        [ActionAttribute(ActionType.Read, ActionType.Delete)]
        XoaKhachTrungLap = 453,
        [ModuleGroupAttribute(ModuleGroupCode = 7, ModuleGroupName = "Quản lý dữ liệu")]
        [Description("Chuyển giao dữ liệu")]
        [ActionAttribute(ActionType.Read, ActionType.Update)]
        ChuyenGiaoDuLieu = 454
        #endregion
        #endregion
    }
}
