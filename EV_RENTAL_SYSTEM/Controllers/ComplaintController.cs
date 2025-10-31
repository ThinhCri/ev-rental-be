using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintController : BaseController
    {
        private readonly IComplaintService _complaintService;
        private readonly ILogger<ComplaintController> _logger;

        public ComplaintController(IComplaintService complaintService, ILogger<ComplaintController> logger)
        {
            _complaintService = complaintService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid user authentication"
                    });
                }

                var result = await _complaintService.CreateComplaintAsync(createDto, userId.Value);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return CreatedAtAction(nameof(CreateComplaint), new { id = result.Data?.ComplaintId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating complaint: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error creating complaint",
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllComplaints()
        {
            try
            {
                var result = await _complaintService.GetAllComplaintsAsync();

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
                _logger.LogError(ex, "Error getting all complaints: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving complaints",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("my-complaints")]
        [Authorize]
        public async Task<IActionResult> GetMyComplaints()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid user authentication"
                    });
                }

                var result = await _complaintService.GetUserComplaintsAsync(userId.Value);

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
                _logger.LogError(ex, "Error getting user complaints: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving user complaints",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("station/{stationId}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetComplaintsByStation(int stationId)
        {
            try
            {
                var result = await _complaintService.GetComplaintsByStationAsync(stationId);

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
                _logger.LogError(ex, "Error getting complaints for station {StationId}: {Error}", stationId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error retrieving station complaints",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] UpdateComplaintStatusDto updateDto)
        {
            try
            {
                var result = await _complaintService.UpdateComplaintStatusAsync(id, updateDto);

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
                _logger.LogError(ex, "Error updating complaint status {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error updating complaint status",
                    Error = ex.Message
                });
            }
        }
    }
}

