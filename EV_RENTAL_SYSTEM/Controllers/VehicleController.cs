using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EV_RENTAL_SYSTEM.Data;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Vehicle management controller
    /// </summary>
    public class VehicleController : BaseController
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleController> _logger;
        private readonly ApplicationDbContext _context;

        public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger, ApplicationDbContext context)
        {
            _vehicleService = vehicleService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Get all vehicles endpoint
        /// </summary>
        /// <returns>List of all vehicles</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var result = await _vehicleService.GetAllAsync();

            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get vehicle by ID endpoint
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Vehicle information</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var result = await _vehicleService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new vehicle endpoint (Admin only)
        /// </summary>
        /// <param name="createDto">Vehicle information</param>
        /// <returns>Vehicle creation result</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateVehicle([FromForm] CreateVehicleDto createDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _vehicleService.CreateAsync(createDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetVehicleById), new { id = result.Data?.VehicleId }, result);
        }

        /// <summary>
        /// Update vehicle information endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromForm] UpdateVehicleDto updateDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _vehicleService.UpdateAsync(id, updateDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete vehicle endpoint (Admin only)
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await _vehicleService.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get available vehicles endpoint
        /// </summary>
        /// <returns>List of available vehicles</returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            var result = await _vehicleService.GetAvailableVehiclesAsync();

            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get vehicles by station endpoint
        /// </summary>
        /// <param name="stationId">Station ID</param>
        /// <returns>List of vehicles in station</returns>
        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetVehiclesByStation(int stationId)
        {
            var result = await _vehicleService.GetVehiclesByStationAsync(stationId);

            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get license plates by status endpoint (xem xe nào đang ở trạng thái gì)
        /// </summary>
        /// <param name="status">License plate status (Available, Rented, Maintenance, Reserved)</param>
        /// <returns>List of license plates with status</returns>
        [HttpGet("license-plates/status/{status}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetLicensePlatesByStatus(string status)
        {
            try
            {
                var licensePlates = await _context.LicensePlates
                    .Where(lp => lp.Status == status)
                    .Include(lp => lp.Vehicle)
                        .ThenInclude(v => v.Brand)
                    .Include(lp => lp.Station)
                    .Select(lp => new
                    {
                        lp.LicensePlateId,
                        lp.PlateNumber,
                        lp.Status,
                        Vehicle = new
                        {
                            lp.Vehicle.VehicleId,
                            lp.Vehicle.Model,
                            lp.Vehicle.ModelYear,
                            lp.Vehicle.Description,
                            lp.Vehicle.RangeKm,
                            lp.Vehicle.Battery,
                            lp.Vehicle.VehicleImage,
                            Brand = lp.Vehicle.Brand != null ? new
                            {
                                lp.Vehicle.Brand.BrandId,
                                lp.Vehicle.Brand.BrandName
                            } : null
                        },
                        Station = new
                        {
                            lp.Station.StationId,
                            lp.Station.StationName,
                            Street = lp.Station.Street,
                            District = lp.Station.District,
                            Province = lp.Station.Province,
                            Country = lp.Station.Country
                        }
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Lấy danh sách biển số xe trạng thái '{status}' thành công",
                    Data = licensePlates,
                    Count = licensePlates.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting license plates by status {Status}: {Error}", status, ex.Message);
                return ErrorResponse($"Lỗi khi lấy danh sách xe: {ex.Message}", 500);
            }
        }
    }

}
