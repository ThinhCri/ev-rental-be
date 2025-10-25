using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;

namespace EV_RENTAL_SYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get user by ID endpoint (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User information</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all users endpoint (Admin only)
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search users with filters and pagination endpoint (Admin only)
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>Filtered list of users</returns>
        [HttpPost("search")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SearchUsers([FromBody] UserSearchDto searchDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _userService.SearchUsersAsync(searchDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new user endpoint (Admin only)
        /// </summary>
        /// <param name="createDto">User information</param>
        /// <returns>User creation result</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _userService.CreateAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data?.UserId }, result);
        }

        /// <summary>
        /// Update user information endpoint (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _userService.UpdateAsync(id, updateDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete user endpoint (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Change user password endpoint (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="changePasswordDto">New password information</param>
        /// <returns>Password change result</returns>
        [HttpPut("{id}/password")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangeUserPasswordDto changePasswordDto)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _userService.ChangePasswordAsync(id, changePasswordDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update user status endpoint (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="status">New status</param>
        /// <returns>Status update result</returns>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            var result = await _userService.UpdateStatusAsync(id, status);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
