using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    /// <summary>
    /// Interface cho service quản lý nhân viên (Admin)
    /// </summary>
    public interface IStaffService
    {
        /// <summary>
        /// Lấy thông tin nhân viên theo ID
        /// </summary>
        /// <param name="id">ID của nhân viên</param>
        /// <returns>Thông tin nhân viên</returns>
        Task<StaffResponseDto> GetByIdAsync(int id);

        /// <summary>
        /// Lấy danh sách tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách nhân viên</returns>
        Task<StaffListResponseDto> GetAllAsync();

        /// <summary>
        /// Tìm kiếm nhân viên với phân trang
        /// </summary>
        /// <param name="searchDto">Thông tin tìm kiếm</param>
        /// <returns>Danh sách nhân viên đã lọc</returns>
        Task<StaffListResponseDto> SearchStaffAsync(StaffSearchDto searchDto);

        /// <summary>
        /// Lấy danh sách nhân viên theo trạm
        /// </summary>
        /// <param name="stationId">ID của trạm</param>
        /// <returns>Danh sách nhân viên của trạm</returns>
        Task<StaffListResponseDto> GetByStationIdAsync(int stationId);

        /// <summary>
        /// Tạo nhân viên mới
        /// </summary>
        /// <param name="createDto">Thông tin nhân viên mới</param>
        /// <returns>Kết quả tạo nhân viên</returns>
        Task<StaffResponseDto> CreateAsync(CreateStaffDto createDto);

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="id">ID của nhân viên</param>
        /// <param name="updateDto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto updateDto);

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="id">ID của nhân viên</param>
        /// <returns>Kết quả xóa</returns>
        Task<StaffResponseDto> DeleteAsync(int id);

        /// <summary>
        /// Đổi mật khẩu nhân viên
        /// </summary>
        /// <param name="id">ID của nhân viên</param>
        /// <param name="changePasswordDto">Thông tin mật khẩu mới</param>
        /// <returns>Kết quả đổi mật khẩu</returns>
        Task<StaffResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto changePasswordDto);

        /// <summary>
        /// Kích hoạt/vô hiệu hóa nhân viên
        /// </summary>
        /// <param name="id">ID của nhân viên</param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        Task<StaffResponseDto> UpdateStatusAsync(int id, string status);

        /// <summary>
        /// Chuyển nhân viên sang trạm khác
        /// </summary>
        /// <param name="id">ID của nhân viên</param>
        /// <param name="newStationId">ID trạm mới</param>
        /// <returns>Kết quả chuyển trạm</returns>
        Task<StaffResponseDto> TransferStationAsync(int id, int newStationId);
    }
}
