<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (vehicle != null)
                {
                   return Ok(vehicle); 
                }
                return NotFound(new { message = "Vehicle not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleDto vehicleDto)
        {
            try
            {
                var result = await _vehicleService.CreateVehicleAsync(vehicleDto);
                return CreatedAtAction(nameof(GetVehicleById), new { id = result.VehicleId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleDto vehicleDto)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicleAsync(id, vehicleDto);
                if (result == null)
                {
                    return NotFound(new { message = "Vehicle not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            
                var result = await _vehicleService.DeleteVehicleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Vehicle not found" });
                }
                return Ok(new { message = "Vehicle deleted successfully" });
            }
            
        }
    }

=======
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Vehicle management controller
    /// </summary>
    public class VehicleController : BaseController
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
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
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto createDto)
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
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] UpdateVehicleDto updateDto)
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
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
