using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Base controller providing common functionality for all controllers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        /// <returns>User ID or null if not found</returns>
        protected int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("uid");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Get current user role from JWT token
        /// </summary>
        /// <returns>User role or null if not found</returns>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Check if current user is authenticated
        /// </summary>
        /// <returns>True if authenticated, false otherwise</returns>
        protected bool IsAuthenticated()
        {
            return User.Identity?.IsAuthenticated == true;
        }

        /// <summary>
        /// Standardized error response
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>Error response</returns>
        protected IActionResult ErrorResponse(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                Success = false,
                Message = message,
                Data = (object?)null
            });
        }

        /// <summary>
        /// Standardized success response
        /// </summary>
        /// <param name="data">Response data</param>
        /// <param name="message">Success message</param>
        /// <returns>Success response</returns>
        protected IActionResult SuccessResponse<T>(T data, string message = "Success")
        {
            return Ok(new
            {
                Success = true,
                Message = message,
                Data = data
            });
        }

        /// <summary>
        /// Validate model state and return error if invalid
        /// </summary>
        /// <returns>BadRequest if invalid, null if valid</returns>
        protected IActionResult? ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid input data",
                    Data = (object?)null,
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }
            return null;
        }

        /// <summary>
        /// Check if user is authorized and return error if not
        /// </summary>
        /// <returns>Unauthorized if not authenticated, null if authorized</returns>
        protected IActionResult? CheckAuthentication()
        {
            if (!IsAuthenticated())
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Authentication required",
                    Data = (object?)null
                });
            }
            return null;
        }

        /// <summary>
        /// Check if user has required role and return error if not
        /// </summary>
        /// <param name="requiredRoles">Required roles</param>
        /// <returns>Forbidden if not authorized, null if authorized</returns>
        protected IActionResult? CheckAuthorization(params string[] requiredRoles)
        {
            var authCheck = CheckAuthentication();
            if (authCheck != null) return authCheck;

            var userRole = GetCurrentUserRole();
            if (userRole == null || !requiredRoles.Contains(userRole))
            {
                return StatusCode(403, new
                {
                    Success = false,
                    Message = "Insufficient permissions",
                    Data = (object?)null
                });
            }
            return null;
        }
    }
}
