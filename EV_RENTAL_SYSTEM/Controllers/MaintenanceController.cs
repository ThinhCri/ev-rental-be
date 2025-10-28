using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Maintenance management controller
    /// </summary>
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

        /// <summary>
        /// Get all maintenances endpoint (Staff and Admin only)
        /// </summary>
        /// <returns>List of all maintenances</returns>
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
                    Message = "Lỗi server khi lấy danh sách bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get maintenance by ID endpoint
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <returns>Maintenance information</returns>
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
                    Message = "Lỗi server khi lấy thông tin bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get maintenances by license plate ID endpoint
        /// </summary>
        /// <param name="licensePlateId">License Plate ID</param>
        /// <returns>List of maintenances for the license plate</returns>
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
                    Message = "Lỗi server khi lấy danh sách bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get maintenances by station ID endpoint
        /// </summary>
        /// <param name="stationId">Station ID</param>
        /// <returns>List of maintenances for the station</returns>
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
                    Message = "Lỗi server khi lấy danh sách bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Complete maintenance endpoint (auto set battery 100%)
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <param name="cost">Optional cost</param>
        /// <returns>Maintenance completion result</returns>
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
                    Message = "Lỗi server khi hoàn thành bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Create new maintenance endpoint
        /// </summary>
        /// <param name="createDto">Maintenance information</param>
        /// <returns>Maintenance creation result</returns>
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
                        Message = "Dữ liệu không hợp lệ"
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
                        Message = "Dữ liệu không hợp lệ",
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
                    Message = "Lỗi server khi tạo bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Update maintenance endpoint
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
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
                        Message = "Dữ liệu cập nhật không hợp lệ"
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
                    Message = "Lỗi server khi cập nhật bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete maintenance endpoint
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <returns>Delete result</returns>
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
                    Message = "Lỗi server khi xóa bảo trì",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Search maintenances endpoint
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>List of matching maintenances</returns>
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
                        Message = "Dữ liệu tìm kiếm không hợp lệ"
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
                    Message = "Lỗi server khi tìm kiếm bảo trì",
                    Error = ex.Message
                });
            }
        }
    }
}

