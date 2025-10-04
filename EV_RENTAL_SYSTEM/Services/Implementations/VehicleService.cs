using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(v => new VehicleDto
            {
                VehicleId = v.VehicleId,
                Model = v.Model,
                SeatNumber = v.SeatNumber ?? 0,
                VehicleImage = v.VehicleImage ?? string.Empty,
                PricePerDay = v.PricePerDay ?? 0,
                Battery = v.Battery ?? string.Empty,
                Status = "Available", // Default status
                ModelYear = v.ModelYear,
                BrandId = v.BrandId,
                Description = v.Description
            });
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return null;

            return new VehicleDto
            {
                VehicleId = vehicle.VehicleId,
                Model = vehicle.Model,
                SeatNumber = vehicle.SeatNumber ?? 0,
                VehicleImage = vehicle.VehicleImage ?? string.Empty,
                PricePerDay = vehicle.PricePerDay ?? 0,
                Battery = vehicle.Battery ?? string.Empty,
                Status = "Available", // Default status
                ModelYear = vehicle.ModelYear,
                BrandId = vehicle.BrandId,
                Description = vehicle.Description
            };
        }

        public async Task<VehicleDto> CreateVehicleAsync(VehicleDto dto)
        {
            var vehicle = new Vehicle
            {
                Model = dto.Model,
                SeatNumber = dto.SeatNumber,
                VehicleImage = dto.VehicleImage,
                PricePerDay = dto.PricePerDay,
                Battery = dto.Battery,
                ModelYear = dto.ModelYear,
                BrandId = dto.BrandId,
                Description = dto.Description
            };

            await _vehicleRepository.AddAsync(vehicle);
            dto.VehicleId = vehicle.VehicleId;
            return dto;
        }

        public async Task<VehicleDto?> UpdateVehicleAsync(int id, VehicleDto dto)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return null;

            vehicle.Model = dto.Model;
            vehicle.SeatNumber = dto.SeatNumber;
            vehicle.VehicleImage = dto.VehicleImage;
            vehicle.PricePerDay = dto.PricePerDay;
            vehicle.Battery = dto.Battery;
            vehicle.ModelYear = dto.ModelYear;
            vehicle.BrandId = dto.BrandId;
            vehicle.Description = dto.Description;

            await _vehicleRepository.UpdateAsync(vehicle);

            return dto;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return false;

            await _vehicleRepository.DeleteAsync(id);
            return true;
        }
    }
}
