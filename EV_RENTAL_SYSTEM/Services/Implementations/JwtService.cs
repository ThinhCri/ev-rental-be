using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo JWT token cho user
        /// </summary>
        /// <param name="user">Thông tin user</param>
        /// <returns>JWT token string</returns>
        public string GenerateToken(User user)
        {
            // Lấy cấu hình JWT từ appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]!);

            // Tạo key để ký token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

<<<<<<< HEAD
            // Tạo claims (thông tin lưu trong token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // ID user
                new Claim(ClaimTypes.Email, user.Email), // Email user
                new Claim(ClaimTypes.Name, user.FullName), // Tên đầy đủ
                new Claim(ClaimTypes.Role, user.Role.RoleName), // Role (Admin, Staff, EV Renter)
                new Claim("UserId", user.UserId.ToString()) // User ID (custom claim)
=======
            // Tạo claims tối giản để token ngắn hơn
            var claims = new[]
            {
                new Claim("uid", user.UserId.ToString()), // User ID (ngắn hơn)
                new Claim(ClaimTypes.Role, user.Role.RoleName), // Role (sử dụng ClaimTypes.Role)
                new Claim("role", user.Role.RoleName), // Role (backup)
                new Claim("email", user.Email) // Email
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
            };

            // Tạo JWT token với các thông tin trên
            var token = new JwtSecurityToken(
                issuer: issuer, // Người phát hành token
                audience: audience, // Đối tượng sử dụng token
                claims: claims, // Thông tin trong token
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes), // Thời hạn token (60 phút)
                signingCredentials: credentials // Chữ ký để verify token
            );

            // Chuyển token thành string để trả về
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Kiểm tra JWT token có hợp lệ không
        /// </summary>
        /// <param name="token">JWT token cần kiểm tra</param>
        /// <returns>Token có hợp lệ hay không</returns>
        public bool ValidateToken(string token)
        {
            try
            {
                // Lấy cấu hình JWT từ appsettings.json
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];

                // Tạo key để verify token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
                var tokenHandler = new JwtSecurityTokenHandler();

                // Validate token với các tham số
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Kiểm tra chữ ký
                    IssuerSigningKey = key, // Key để verify
                    ValidateIssuer = true, // Kiểm tra issuer
                    ValidIssuer = issuer, // Issuer hợp lệ
                    ValidateAudience = true, // Kiểm tra audience
                    ValidAudience = audience, // Audience hợp lệ
                    ValidateLifetime = true, // Kiểm tra thời hạn
                    ClockSkew = TimeSpan.Zero // Không cho phép sai lệch thời gian
                }, out SecurityToken validatedToken);

                return true; // Token hợp lệ
            }
            catch
            {
                return false; // Token không hợp lệ
            }
        }

        /// <summary>
        /// Lấy User ID từ JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID</returns>
        public int GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
<<<<<<< HEAD
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId");
=======
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "uid");
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        /// <summary>
        /// Lấy Email từ JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Email của user</returns>
        public string GetUserEmailFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
<<<<<<< HEAD
            var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
=======
            var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
            return emailClaim?.Value ?? string.Empty;
        }
    }
}
