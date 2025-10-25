using System.ComponentModel.DataAnnotations;
using EV_RENTAL_SYSTEM.Models.Enums;
using EV_RENTAL_SYSTEM.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho request đăng nhập
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho request đăng ký
    /// </summary>
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Họ tên không được quá 255 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [MaxLength(255, ErrorMessage = "Mật khẩu không được quá 255 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        [DateOnlyValidation(ErrorMessage = "Ngày sinh phải có định dạng yyyy-MM-dd (ví dụ: 1990-01-15)")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Ảnh bằng lái xe là bắt buộc để thuê xe")]
        [LicenseImageValidation(ErrorMessage = "Ảnh bằng lái xe không hợp lệ. Vui lòng upload ảnh rõ nét, kích thước tối thiểu 300x200px, định dạng JPG/PNG")]
        public IFormFile LicenseImage { get; set; } = null!;

        [Required(ErrorMessage = "Số bằng lái xe là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Số bằng lái xe không được quá 50 ký tự")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày hết hạn bằng lái xe là bắt buộc")]
        public DateTime LicenseExpiryDate { get; set; }

        [Required(ErrorMessage = "Loại bằng lái xe là bắt buộc")]
        public LicenseTypeEnum LicenseTypeId { get; set; }
    }


    /// <summary>
    /// DTO cho response xác thực (đăng nhập/đăng ký)
    /// </summary>
    public class AuthResponseDto
    {
        public bool Success { get; set; } // Có thành công hay không
        public string Message { get; set; } = string.Empty; // Thông báo kết quả
        public string? Token { get; set; } // JWT token (nếu thành công)
        public UserDto? User { get; set; } // Thông tin user (nếu thành công)
    }

    /// <summary>
    /// DTO cho thông tin user (trả về cho client)
    /// </summary>
    public class UserDto
    {
        public int UserId { get; set; } 
        public string FullName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string PhoneNumber { get; set; } = string.Empty; 
        public DateTime? Birthday { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public string? Status { get; set; } 
        public string RoleName { get; set; } = string.Empty; 
        public int? StationId { get; set; }
        public string? DriverLicenseNumber { get; set; }   
        public string? DriverLicenseImage { get; set; }
    }

    /// <summary>
    /// DTO cho admin tạo user mới
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Họ tên không được quá 255 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Role là bắt buộc")]
        public int RoleId { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; } = "Active";
    }

    /// <summary>
    /// DTO cho admin cập nhật user
    /// </summary>
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Họ tên không được quá 255 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Role là bắt buộc")]
        public int RoleId { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO cho admin đổi mật khẩu user
    /// </summary>
    public class ChangeUserPasswordDto
    {
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho response API user
    /// </summary>
    public class UserResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách user
    /// </summary>
    public class UserListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<UserDto> Data { get; set; } = new List<UserDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm user
    /// </summary>
    public class UserSearchDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt"; // FullName, Email, CreatedAt
        public string? SortOrder { get; set; } = "desc"; // asc, desc
    }

    /// <summary>
    /// DTO cho admin tạo nhân viên mới
    /// </summary>
    public class CreateStaffDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Họ tên không được quá 255 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Trạm làm việc là bắt buộc")]
        public int StationId { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; } = "Active";

        [MaxLength(255, ErrorMessage = "Ghi chú không được quá 255 ký tự")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO cho admin cập nhật nhân viên
    /// </summary>
    public class UpdateStaffDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Họ tên không được quá 255 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255, ErrorMessage = "Email không được quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Trạm làm việc là bắt buộc")]
        public int StationId { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; }

        [MaxLength(255, ErrorMessage = "Ghi chú không được quá 255 ký tự")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin nhân viên (trả về cho client)
    /// </summary>
    public class StaffDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO cho response API nhân viên
    /// </summary>
    public class StaffResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StaffDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách nhân viên
    /// </summary>
    public class StaffListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StaffDto> Data { get; set; } = new List<StaffDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm nhân viên
    /// </summary>
    public class StaffSearchDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? StationId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt"; // FullName, Email, CreatedAt, StationName
        public string? SortOrder { get; set; } = "desc"; // asc, desc
    }
}
