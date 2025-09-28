using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Controller xử lý các API xác thực (đăng ký, đăng nhập, đăng xuất)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// API đăng nhập người dùng
        /// </summary>
        /// <param name="loginRequest">Thông tin đăng nhập (email, password)</param>
        /// <returns>Kết quả đăng nhập kèm JWT token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            // Kiểm tra dữ liệu đầu vào có hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi service xử lý đăng nhập
            var result = await _authService.LoginAsync(loginRequest);
            
            // Nếu đăng nhập thất bại, trả về lỗi 401
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            // Đăng nhập thành công, trả về thông tin user và token
            return Ok(result);
        }

        /// <summary>
        /// API đăng ký tài khoản người dùng mới (có thể kèm thông tin bằng lái xe)
        /// </summary>
        /// <param name="registerRequest">Thông tin đăng ký (tên, email, password, ngày sinh, thông tin bằng lái xe)</param>
        /// <returns>Kết quả đăng ký kèm JWT token</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto registerRequest)
        {
            // Kiểm tra dữ liệu đầu vào có hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi service xử lý đăng ký
            var result = await _authService.RegisterAsync(registerRequest);
            
            // Nếu đăng ký thất bại, trả về lỗi 400
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            // Đăng ký thành công, trả về thông tin user và token
            return Ok(result);
        }

        /// <summary>
        /// API đăng xuất người dùng
        /// </summary>
        /// <returns>Xác nhận đăng xuất thành công</returns>
        [HttpPost("logout")]
        [Authorize] // Yêu cầu JWT token hợp lệ
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy token từ header Authorization
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(new { message = "Invalid authorization header" });
                }

                // Tách token từ "Bearer TOKEN"
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Token not provided" });
                }

                // Gọi service xử lý đăng xuất
                var result = await _authService.LogoutAsync(token);
                
                if (!result)
                {
                    return BadRequest(new { message = "Invalid token" });
                }

                // Đăng xuất thành công
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và trả về thông báo lỗi
                _logger.LogError(ex, "Error during logout");
                return BadRequest(new { message = "Error during logout" });
            }
        }

        /// <summary>
        /// API kiểm tra JWT token có hợp lệ không
        /// </summary>
        /// <returns>Kết quả kiểm tra token</returns>
        [HttpGet("validate")]
        [Authorize] // Yêu cầu JWT token hợp lệ
        public async Task<IActionResult> ValidateToken()
        {
            // Lấy token từ header Authorization
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token not provided" });
            }

            // Gọi service kiểm tra token
            var isValid = await _authService.ValidateTokenAsync(token);
            
            if (!isValid)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Token hợp lệ
            return Ok(new { message = "Token is valid" });
        }

    }
}
