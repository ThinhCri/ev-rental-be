using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    /// <summary>
    /// Interface cho service quản lý user (Admin)
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Lấy thông tin user theo ID
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <returns>Thông tin user</returns>
        Task<UserResponseDto> GetByIdAsync(int id);

        /// <summary>
        /// Lấy danh sách tất cả user
        /// </summary>
        /// <returns>Danh sách user</returns>
        Task<UserListResponseDto> GetAllAsync();

        /// <summary>
        /// Tìm kiếm user với phân trang
        /// </summary>
        /// <param name="searchDto">Thông tin tìm kiếm</param>
        /// <returns>Danh sách user đã lọc</returns>
        Task<UserListResponseDto> SearchUsersAsync(UserSearchDto searchDto);

        /// <summary>
        /// Tạo user mới
        /// </summary>
        /// <param name="createDto">Thông tin user mới</param>
        /// <returns>Kết quả tạo user</returns>
        Task<UserResponseDto> CreateAsync(CreateUserDto createDto);

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <param name="updateDto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto updateDto);

        /// <summary>
        /// Xóa user
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <returns>Kết quả xóa</returns>
        Task<UserResponseDto> DeleteAsync(int id);

        /// <summary>
        /// Đổi mật khẩu user
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <param name="changePasswordDto">Thông tin mật khẩu mới</param>
        /// <returns>Kết quả đổi mật khẩu</returns>
        Task<UserResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto changePasswordDto);

        /// <summary>
        /// Kích hoạt/vô hiệu hóa user
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        Task<UserResponseDto> UpdateStatusAsync(int id, string status);
    }
}
