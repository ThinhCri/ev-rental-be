using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

namespace EV_RENTAL_SYSTEM.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _authService.LoginAsync(loginRequest);
            
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto registerRequest)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            var result = await _authService.RegisterAsync(registerRequest);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return ErrorResponse("Authorization header not provided");
                }

                var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authHeader.Substring("Bearer ".Length).Trim()
                    : authHeader;
                
                if (string.IsNullOrEmpty(token))
                {
                    return ErrorResponse("Token not provided");
                }

                var result = await _authService.LogoutAsync(token);
                
                if (!result)
                {
                    return ErrorResponse("Invalid token");
                }

                return SuccessResponse<object?>(null, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return ErrorResponse("Error during logout");
            }
        }

        [HttpGet("validate")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> ValidateToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
            {
                return ErrorResponse("Authorization header not provided");
            }

            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader.Substring("Bearer ".Length).Trim()
                : authHeader;
            
            if (string.IsNullOrEmpty(token))
            {
                return ErrorResponse("Token not provided");
            }

            var isValid = await _authService.ValidateTokenAsync(token);
            
            if (!isValid)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            return SuccessResponse<object?>(null, "Token is valid");
        }

        [HttpGet("me")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = user.Role.RoleName;

                return SuccessResponse(userDto, "User information retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user");
                return ErrorResponse("An error occurred while retrieving user information", 500);
            }
        }

        [HttpGet("order-history")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetOrderHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var orders = await _unitOfWork.Orders.GetUserOrderHistoryAsync(userId.Value, pageNumber, pageSize);
                
                if (orders == null || !orders.Any())
                {
                    return SuccessResponse(new OrderHistoryListResponseDto
                    {
                        Success = true,
                        Message = "No order history found",
                        Data = new List<OrderHistoryDto>(),
                        TotalCount = 0,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = 0
                    });
                }

                var orderHistoryDtos = new List<OrderHistoryDto>();
                foreach (var order in orders)
                {
                    var orderDto = new OrderHistoryDto
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate,
                        StartTime = order.StartTime,
                        EndTime = order.EndTime,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status
                    };

                    if (order.OrderLicensePlates.Any())
                    {
                        var orderLicensePlate = order.OrderLicensePlates.First();
                        var licensePlate = orderLicensePlate.LicensePlate;
                        
                        orderDto.LicensePlateId = licensePlate.LicensePlateId.ToString();
                        orderDto.VehicleModel = licensePlate.Vehicle?.Model;
                        orderDto.VehicleBrand = licensePlate.Vehicle?.Brand?.BrandName;
                        orderDto.VehicleType = null;
                        orderDto.StationName = licensePlate.Station?.StationName;
                        orderDto.StationAddress = $"{licensePlate.Station?.Street}, {licensePlate.Station?.District}, {licensePlate.Station?.Province}";
                    }

                    var userLicenses = await _unitOfWork.Licenses.GetByUserIdAsync(userId.Value);
                    if (userLicenses.Any())
                    {
                        var license = userLicenses.First();
                        orderDto.LicenseNumber = license.LicenseNumber;
                        orderDto.LicenseType = license.LicenseType?.TypeName;
                    }

                    orderHistoryDtos.Add(orderDto);
                }

                var totalCount = await _unitOfWork.Orders.GetUserOrderCountAsync(userId.Value);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return SuccessResponse(new OrderHistoryListResponseDto
                {
                    Success = true,
                    Message = "Order history retrieved successfully",
                    Data = orderHistoryDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order history for user");
                return ErrorResponse("An error occurred while retrieving order history", 500);
            }
        }

    }
}
