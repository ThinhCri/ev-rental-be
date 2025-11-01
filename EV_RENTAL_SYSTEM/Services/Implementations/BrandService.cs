using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class BrandService : BaseService, IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandService(IBrandRepository brandRepository, IMapper mapper, ILogger<BrandService> logger) : base(logger)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<BrandListResponseDto> GetAllAsync()
        {
           
                var brands = await _brandRepository.GetAllAsync();
                var brandDtos = _mapper.Map<List<BrandDto>>(brands);

                return new BrandListResponseDto
                {
                    Success = true,
                    Message = "Take Brand Success",
                    Data = brandDtos
                };
        }
    }
}
