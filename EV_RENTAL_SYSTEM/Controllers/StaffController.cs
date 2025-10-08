using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;

namespace EV_RENTAL_SYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : BaseController
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Get staff by ID endpoint (Admin only)
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <returns>Staff information</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var result = await _staffService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all staff endpoint (Admin only)
        /// </summary>
        /// <returns>List of all staff</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllStaff()
        {
            var result = await _staffService.GetAllAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search staff with filters and pagination endpoint (Admin only)
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>Filtered list of staff</returns>
        [HttpPost("search")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SearchStaff([FromBody] StaffSearchDto searchDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _staffService.SearchStaffAsync(searchDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get staff by station ID endpoint (Admin only)
        /// </summary>
        /// <param name="stationId">Station ID</param>
        /// <returns>List of staff in the station</returns>
        [HttpGet("station/{stationId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetStaffByStation(int stationId)
        {
            var result = await _staffService.GetByStationIdAsync(stationId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new staff endpoint (Admin only)
        /// </summary>
        /// <param name="createDto">Staff information</param>
        /// <returns>Staff creation result</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto createDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _staffService.CreateAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetStaffById), new { id = result.Data?.UserId }, result);
        }

        /// <summary>
        /// Update staff information endpoint (Admin only)
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffDto updateDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _staffService.UpdateAsync(id, updateDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete staff endpoint (Admin only)
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Change staff password endpoint (Admin only)
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="changePasswordDto">New password information</param>
        /// <returns>Password change result</returns>
        [HttpPut("{id}/password")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeStaffPassword(int id, [FromBody] ChangeUserPasswordDto changePasswordDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _staffService.ChangePasswordAsync(id, changePasswordDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update staff status endpoint (Admin only)
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="status">New status</param>
        /// <returns>Status update result</returns>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStaffStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            var result = await _staffService.UpdateStatusAsync(id, status);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Transfer staff to another station endpoint (Admin only)
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="newStationId">New station ID</param>
        /// <returns>Transfer result</returns>
        [HttpPut("{id}/transfer")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> TransferStaff(int id, [FromBody] int newStationId)
        {
            var result = await _staffService.TransferStationAsync(id, newStationId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
