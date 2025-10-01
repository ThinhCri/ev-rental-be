using System.ComponentModel.DataAnnotations;

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
        public string Phone { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        // Thông tin bằng lái xe (bắt buộc nếu muốn thuê xe)
        [Required(ErrorMessage = "Ảnh bằng lái xe là bắt buộc để thuê xe")]
        public IFormFile LicenseImage { get; set; } = null!;

        [Required(ErrorMessage = "Số bằng lái xe là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Số bằng lái xe không được quá 50 ký tự")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày hết hạn bằng lái xe là bắt buộc")]
        public DateTime LicenseExpiryDate { get; set; }

        [Required(ErrorMessage = "Loại bằng lái xe là bắt buộc")]
        public int LicenseTypeId { get; set; }
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
        public Dictionary<string, string[]> Errors { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin user (trả về cho client)
    /// </summary>
    public class UserDto
    {
        public int UserId { get; set; } // ID user
        public string FullName { get; set; } = string.Empty; // Họ tên
        public string Email { get; set; } = string.Empty; // Email
        public DateTime? Birthday { get; set; } // Ngày sinh
        public DateTime CreatedAt { get; set; } // Ngày tạo tài khoản
        public string? Status { get; set; } // Trạng thái tài khoản
        public string RoleName { get; set; } = string.Empty; // Tên role (Admin, Staff, EV Renter)
    }
}
