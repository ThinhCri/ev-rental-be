using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
        Task<bool> LogoutAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
}

