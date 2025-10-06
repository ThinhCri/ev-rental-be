using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Rental management controller
    /// </summary>
    public class RentalController : BaseController
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(IRentalService rentalService, ILogger<RentalController> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        /// <summary>
        /// Get all rentals endpoint (Admin and Staff only)
        /// </summary>
        /// <returns>List of all rentals</returns>
        [HttpGet]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetAllRentals()
        {
            var result = await _rentalService.GetAllRentalsAsync();
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get rental by ID endpoint
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Rental information</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetRentalById(int id)
        {
            var result = await _rentalService.GetRentalByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user's rental history endpoint
        /// </summary>
        /// <returns>User's rental history</returns>
        [HttpGet("my-rentals")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetMyRentals()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = await _rentalService.GetUserRentalsAsync(userId.Value);
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get rental details endpoint
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Detailed rental information</returns>
        [HttpGet("{id}/details")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetRentalDetails(int id)
        {
            var result = await _rentalService.GetRentalWithDetailsAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search available vehicles endpoint
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>List of available vehicles</returns>
        [HttpPost("available-vehicles")]
        public async Task<IActionResult> GetAvailableVehicles([FromBody] AvailableVehiclesSearchDto searchDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            // Validate date range
            if (searchDto.StartTime >= searchDto.EndTime)
            {
                return ErrorResponse("End time must be after start time");
            }

            if (searchDto.StartTime < DateTime.Now)
            {
                return ErrorResponse("Start time cannot be in the past");
            }

            var result = await _rentalService.GetAvailableVehiclesAsync(searchDto);
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new rental endpoint
        /// </summary>
        /// <param name="createDto">Rental information</param>
        /// <returns>Rental creation result</returns>
        [HttpPost]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto createDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            // Validate date range
            if (createDto.StartTime >= createDto.EndTime)
            {
                return ErrorResponse("End time must be after start time");
            }

            if (createDto.StartTime < DateTime.Now)
            {
                return ErrorResponse("Start time cannot be in the past");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = await _rentalService.CreateRentalAsync(createDto, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetRentalById), new { id = result.Data?.OrderId }, result);
        }

        /// <summary>
        /// Update rental endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateRental(int id, [FromBody] UpdateRentalDto updateDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _rentalService.UpdateRentalAsync(id, updateDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cancel rental endpoint
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Cancel result</returns>
        [HttpPut("{id}/cancel")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CancelRental(int id)
        {
            var result = await _rentalService.CancelRentalAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Complete rental endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Complete result</returns>
        [HttpPut("{id}/complete")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> CompleteRental(int id)
        {
            var result = await _rentalService.CompleteRentalAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search rentals endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>List of matching rentals</returns>
        [HttpPost("search")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> SearchRentals([FromBody] RentalSearchDto searchDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _rentalService.SearchRentalsAsync(searchDto);
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Calculate rental cost endpoint
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>Rental cost calculation</returns>
        [HttpGet("calculate-cost")]
        public async Task<IActionResult> CalculateRentalCost([FromQuery] int vehicleId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            if (startTime >= endTime)
            {
                return ErrorResponse("End time must be after start time");
            }

            var cost = await _rentalService.CalculateRentalCostAsync(vehicleId, startTime, endTime);
            
            return SuccessResponse(new
            {
                VehicleId = vehicleId,
                StartTime = startTime,
                EndTime = endTime,
                TotalDays = (int)Math.Ceiling((endTime - startTime).TotalDays),
                TotalCost = cost
            }, "Cost calculated successfully");
        }

        /// <summary>
        /// Check vehicle availability endpoint
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>Vehicle availability status</returns>
        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckVehicleAvailability([FromQuery] int vehicleId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            if (startTime >= endTime)
            {
                return ErrorResponse("End time must be after start time");
            }

            var isAvailable = await _rentalService.IsVehicleAvailableAsync(vehicleId, startTime, endTime);
            
            return SuccessResponse(new
            {
                VehicleId = vehicleId,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = isAvailable
            }, "Availability checked successfully");
        }
    }
}

