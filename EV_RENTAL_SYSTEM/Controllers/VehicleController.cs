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
    }

}
