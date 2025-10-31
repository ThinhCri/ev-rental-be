using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : BaseController
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(IMaintenanceService maintenanceService, ILogger<MaintenanceController> logger)
        {
            _maintenanceService = maintenanceService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetAllMaintenances()
        {
            try
            {
                var result = await _maintenanceService.GetAllMaintenancesAsync();
                
                if (!result.Success)
                {
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all maintenances: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving maintenance list",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetMaintenanceById(int id)
        {
            try
            {
                var result = await _maintenanceService.GetMaintenanceByIdAsync(id);
                
                if (!result.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance by ID {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving maintenance information",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("license-plate/{licensePlateId}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetMaintenancesByLicensePlateId(int licensePlateId)
        {
            try
            {
                var result = await _maintenanceService.GetMaintenancesByLicensePlateIdAsync(licensePlateId);
                
                if (!result.Success)
                {
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for license plate {LicensePlateId}: {Error}", licensePlateId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving maintenance list",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("station/{stationId}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetMaintenancesByStationId(int stationId)
        {
            try
            {
                var result = await _maintenanceService.GetMaintenancesByStationIdAsync(stationId);
                
                if (!result.Success)
                {
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for station {StationId}: {Error}", stationId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving maintenance list",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}/complete")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] decimal? cost = null)
        {
            try
            {
                var result = await _maintenanceService.CompleteMaintenanceAsync(id, cost);
                
                if (!result.Success)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing maintenance {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error completing maintenance",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> CreateMaintenance([FromBody] CreateMaintenanceDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid data"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid data",
                        Errors = errors
                    });
                }

                var result = await _maintenanceService.CreateMaintenanceAsync(createDto);
                
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error creating maintenance",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] UpdateMaintenanceDto updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid update data"
                    });
                }

                var result = await _maintenanceService.UpdateMaintenanceAsync(id, updateDto);
                
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error updating maintenance",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            try
            {
                var result = await _maintenanceService.DeleteMaintenanceAsync(id);
                
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error deleting maintenance",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("search")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> SearchMaintenances([FromBody] MaintenanceSearchDto searchDto)
        {
            try
            {
                if (searchDto == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid search data"
                    });
                }

                var result = await _maintenanceService.SearchMaintenancesAsync(searchDto);
                
                if (!result.Success)
                {
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching maintenances: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error searching maintenance",
                    Error = ex.Message
                });
            }
        }
    }
}
