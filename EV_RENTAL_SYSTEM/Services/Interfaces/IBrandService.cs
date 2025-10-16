using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IBrandService
    {
        Task<BrandListResponseDto> GetAllAsync();
    }
}
