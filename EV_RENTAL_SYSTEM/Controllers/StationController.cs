using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    public class StationController : BaseController
    {
        private readonly IStationService _stationService;
        private readonly ILogger<StationController> _logger;

        public StationController(IStationService stationService, ILogger<StationController> logger)
        {
            _stationService = stationService;
            _logger = logger;
        }

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
