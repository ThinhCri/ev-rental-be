using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Station management controller
    /// </summary>
    public class StationController : BaseController
    {
        private readonly IStationService _stationService;
        private readonly ILogger<StationController> _logger;

        public StationController(IStationService stationService, ILogger<StationController> logger)
        {
            _stationService = stationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all stations endpoint
        /// </summary>
        /// <returns>List of all stations</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllStations()
        {
            var result = await _stationService.GetAllAsync();
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get station by ID endpoint
        /// </summary>
        /// <param name="id">Station ID</param>
        /// <returns>Station information</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStationById(int id)
        {
            var result = await _stationService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get station with vehicles endpoint
        /// </summary>
        /// <param name="id">Station ID</param>
        /// <returns>Station information with vehicles</returns>
        [HttpGet("{id}/vehicles")]
        public async Task<IActionResult> GetStationWithVehicles(int id)
        {
            var result = await _stationService.GetStationWithVehiclesAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get stations by province endpoint
        /// </summary>
        /// <param name="province">Province name</param>
        /// <returns>List of stations in province</returns>
        [HttpGet("province/{province}")]
        public async Task<IActionResult> GetStationsByProvince(string province)
        {
            var result = await _stationService.GetStationsByProvinceAsync(province);
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search stations endpoint
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>List of matching stations</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchStations([FromBody] StationSearchDto searchDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _stationService.SearchStationsAsync(searchDto);
            
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new station endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="createDto">Station information</param>
        /// <returns>Station creation result</returns>
        [HttpPost]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> CreateStation([FromBody] CreateStationDto createDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _stationService.CreateAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetStationById), new { id = result.Data?.StationId }, result);
        }

        /// <summary>
        /// Update station information endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Station ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStation(int id, [FromBody] UpdateStationDto updateDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _stationService.UpdateAsync(id, updateDto);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete station endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Station ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var result = await _stationService.DeleteAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}

