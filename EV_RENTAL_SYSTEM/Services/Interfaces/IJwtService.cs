using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int GetUserIdFromToken(string token);
        string GetUserEmailFromToken(string token);
    }
}

