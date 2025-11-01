using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var userIdClaim = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userIdClaim == null) return Unauthorized(new { message = "User ID claim not found" });
            var userId = int.Parse(userIdClaim);

            var roleClaim = User.FindFirst("role")?.Value ?? User.FindFirst(ClaimTypes.Role)?.Value;
            if(roleClaim == null) return Unauthorized(new { message = "Role claim not found" });

            if(roleClaim != "Admin" && userId != id)
            {
                return Forbid("You can only view your own information");
            }

            var result = await _staffService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

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
