using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Rental management controller
    /// </summary>
    public class RentalController : BaseController
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(IRentalService rentalService, ILogger<RentalController> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        /// <summary>
        /// Get all rentals endpoint (Admin and Staff only)
        /// </summary>
        /// <returns>List of all rentals</returns>
        [HttpGet]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetAllRentals()
        {
            try
            {
                var result = await _rentalService.GetAllRentalsAsync();
                
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
                _logger.LogError(ex, "Error getting all rentals: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi lấy danh sách đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get rental by ID endpoint
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Rental information</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetRentalById(int id)
        {
            try
            {
                var result = await _rentalService.GetRentalByIdAsync(id);
                
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
                _logger.LogError(ex, "Error getting rental by ID {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi lấy thông tin đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get current user's rental history endpoint
        /// </summary>
        /// <returns>User's rental history</returns>
        [HttpGet("my-rentals")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetMyRentals()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new 
                    { 
                        Success = false,
                        Message = "Token không hợp lệ" 
                    });
                }

                var result = await _rentalService.GetUserRentalsAsync(userId.Value);
                
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
                _logger.LogError(ex, "Error getting user rentals: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi lấy lịch sử thuê xe",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get rental details endpoint
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Detailed rental information</returns>
        [HttpGet("{id}/details")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetRentalDetails(int id)
        {
            try
            {
                var result = await _rentalService.GetRentalWithDetailsAsync(id);
                
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
                _logger.LogError(ex, "Error getting rental details for ID {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi lấy chi tiết đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Search available vehicles endpoint
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>List of available vehicles</returns>
        [HttpPost("available-vehicles")]
        public async Task<IActionResult> GetAvailableVehicles([FromBody] AvailableVehiclesSearchDto searchDto)
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

                // Validate date range
                if (searchDto.StartTime >= searchDto.EndTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu"
                    });
                }

                if (searchDto.StartTime < DateTime.Now.AddMinutes(-5)) // Cho phép 5 phút trước
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian bắt đầu không thể trong quá khứ"
                    });
                }

                var result = await _rentalService.GetAvailableVehiclesAsync(searchDto);
                
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
                _logger.LogError(ex, "Error getting available vehicles: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi tìm kiếm xe có sẵn",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Create new rental endpoint (với thanh toán bắt buộc)
        /// </summary>
        /// <param name="createDto">Rental information</param>
        /// <returns>Rental creation result</returns>
        [HttpPost("with-payment")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateRentalWithPayment([FromForm] CreateRentalDto createDto)
        {
            try
            {
                _logger.LogInformation("CreateRentalWithPayment called - VehicleId: {VehicleId}, IsBookingForOthers: {IsBookingForOthers}", 
                    createDto?.VehicleId, createDto?.IsBookingForOthers);

                // Kiểm tra JSON parsing
                if (createDto == null)
                {
                    _logger.LogWarning("CreateRentalWithPayment: createDto is null");
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "JSON không hợp lệ hoặc bị lỗi cú pháp",
                        Hint = "Kiểm tra dấu phẩy, ngoặc vuông và format JSON"
                    });
                }

                // Kiểm tra model validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    var fieldErrors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    
                    _logger.LogWarning("CreateRentalWithPayment: Model validation failed. Errors: {Errors}, FieldErrors: {FieldErrors}", 
                        string.Join(", ", errors), System.Text.Json.JsonSerializer.Serialize(fieldErrors));
                    
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ",
                        Errors = errors,
                        FieldErrors = fieldErrors,
                        ReceivedData = new
                        {
                            StartTime = createDto?.StartTime,
                            EndTime = createDto?.EndTime,
                            VehicleId = createDto?.VehicleId,
                            DepositAmount = createDto?.DepositAmount,
                            IsBookingForOthers = createDto?.IsBookingForOthers,
                            HasRenterLicenseImage = createDto?.RenterLicenseImage != null
                        }
                    });
                }

                // Validate date range
                if (createDto.StartTime >= createDto.EndTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu"
                    });
                }

                if (createDto.StartTime < DateTime.Now.AddMinutes(-5)) // Cho phép 5 phút trước
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian bắt đầu không thể trong quá khứ"
                    });
                }

                if (createDto.IsBookingForOthers)
                {
                    if (createDto.RenterLicenseImage == null)
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = "Renter license image is required"
                        });
                    }
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new 
                    { 
                        Success = false,
                        Message = "Token không hợp lệ" 
                    });
                }

                var result = await _rentalService.CreateRentalWithMandatoryPaymentAsync(createDto, userId.Value);
                
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Thông tin đơn thuê đã sẵn sàng. Vui lòng thanh toán để hoàn tất.",
                    Data = result.Data,
                    RequiresPayment = true,
                    PaymentRequired = true // Flag cho frontend
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental with payment: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi tạo đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Create new rental endpoint (cách cũ - không bắt buộc thanh toán)
        /// </summary>
        /// <param name="createDto">Rental information</param>
        /// <returns>Rental creation result</returns>
        [HttpPost]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateRental([FromForm] CreateRentalDto createDto)
        {
            try
            {
                _logger.LogInformation("CreateRental called - VehicleId: {VehicleId}, IsBookingForOthers: {IsBookingForOthers}", 
                    createDto?.VehicleId, createDto?.IsBookingForOthers);

                // Kiểm tra JSON parsing
                if (createDto == null)
                {
                    _logger.LogWarning("CreateRental: createDto is null");
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "JSON không hợp lệ hoặc bị lỗi cú pháp",
                        Hint = "Kiểm tra dấu phẩy, ngoặc vuông và format JSON"
                    });
                }

                // Kiểm tra model validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    var fieldErrors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    
                    _logger.LogWarning("CreateRental: Model validation failed. Errors: {Errors}, FieldErrors: {FieldErrors}", 
                        string.Join(", ", errors), System.Text.Json.JsonSerializer.Serialize(fieldErrors));
                    
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ",
                        Errors = errors,
                        FieldErrors = fieldErrors,
                        ReceivedData = new
                        {
                            StartTime = createDto?.StartTime,
                            EndTime = createDto?.EndTime,
                            VehicleId = createDto?.VehicleId,
                            DepositAmount = createDto?.DepositAmount,
                            IsBookingForOthers = createDto?.IsBookingForOthers,
                            HasRenterLicenseImage = createDto?.RenterLicenseImage != null
                        }
                    });
                }

                // Validate date range
                if (createDto.StartTime >= createDto.EndTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu"
                    });
                }

                if (createDto.StartTime < DateTime.Now.AddMinutes(-5)) // Cho phép 5 phút trước
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian bắt đầu không thể trong quá khứ"
                    });
                }

				if (createDto.IsBookingForOthers)
				{
					if (createDto.RenterLicenseImage == null)
					{
						return BadRequest(new
						{
							Success = false,
							Message = "Renter license image is required"
						});
					}
				}

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new 
                    { 
                        Success = false,
                        Message = "Token không hợp lệ" 
                    });
                }

                var result = await _rentalService.CreateRentalAsync(createDto, userId.Value);
                
                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Tạo đơn thuê thành công",
                    Data = result.Data,
                    OrderId = result.OrderId, // Thêm OrderId vào response
                    ContractId = result.ContractId // Thêm ContractId vào response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi tạo đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Update rental endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <param name="updateDto">Update information</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateRental(int id, [FromBody] UpdateRentalDto updateDto)
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

                var result = await _rentalService.UpdateRentalAsync(id, updateDto);
                
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
                _logger.LogError(ex, "Error updating rental {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi cập nhật đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Cancel rental endpoint
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Cancel result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CancelRental(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }
                
                var userId = int.Parse(userIdClaim);
                var result = await _rentalService.CancelRentalAsync(id, userId);
                
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
                _logger.LogError(ex, "Error cancelling rental {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi hủy đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Complete rental endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="id">Rental ID</param>
        /// <returns>Complete result</returns>
        [HttpPut("{id}/complete")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> CompleteRental(int id)
        {
            try
            {
                var result = await _rentalService.CompleteRentalAsync(id);
                
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
                _logger.LogError(ex, "Error completing rental {Id}: {Error}", id, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi hoàn thành đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Search rentals endpoint (Admin and Staff only)
        /// </summary>
        /// <param name="searchDto">Search criteria</param>
        /// <returns>List of matching rentals</returns>
        [HttpPost("search")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> SearchRentals([FromBody] RentalSearchDto searchDto)
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

                var result = await _rentalService.SearchRentalsAsync(searchDto);
                
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
                _logger.LogError(ex, "Error searching rentals: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi tìm kiếm đơn thuê",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Calculate rental cost endpoint
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>Rental cost calculation</returns>
        [HttpGet("calculate-cost")]
        public async Task<IActionResult> CalculateRentalCost([FromQuery] int vehicleId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            try
            {
                if (startTime >= endTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu"
                    });
                }

                var cost = await _rentalService.CalculateRentalCostAsync(vehicleId, startTime, endTime);
                
                return Ok(new
                {
                    Success = true,
                    Message = "Tính toán chi phí thành công",
                    Data = new
                    {
                        VehicleId = vehicleId,
                        StartTime = startTime,
                        EndTime = endTime,
                        TotalDays = (int)Math.Ceiling((endTime - startTime).TotalDays),
                        TotalCost = cost
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating rental cost: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi tính toán chi phí",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Check vehicle availability endpoint
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>Vehicle availability status</returns>
        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckVehicleAvailability([FromQuery] int vehicleId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            try
            {
                if (startTime >= endTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu"
                    });
                }

                var isAvailable = await _rentalService.IsVehicleAvailableAsync(vehicleId, startTime, endTime);
                
                return Ok(new
                {
                    Success = true,
                    Message = "Kiểm tra tính khả dụng thành công",
                    Data = new
                    {
                        VehicleId = vehicleId,
                        StartTime = startTime,
                        EndTime = endTime,
                        IsAvailable = isAvailable
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking vehicle availability: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi kiểm tra tính khả dụng",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin bảng hợp đồng để hiển thị
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Contract summary information</returns>
        [HttpGet("{orderId}/contract-summary")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetContractSummary(int orderId)
        {
            try
            {
                var result = await _rentalService.GetContractSummaryAsync(orderId);
                
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
                _logger.LogError(ex, "Error getting contract summary for order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi lấy thông tin hợp đồng",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Xác nhận hợp đồng và tạo QR code thanh toán
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>QR code payment information</returns>
        [HttpPost("{orderId}/confirm-contract")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> ConfirmContract(int orderId)
        {
            try
            {
                var result = await _rentalService.ConfirmContractAsync(orderId);
                
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
                _logger.LogError(ex, "Error confirming contract for order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi xác nhận hợp đồng",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng pending để staff quản lý
        /// </summary>
        /// <returns>List of pending orders with timeout information</returns>
        [HttpGet("pending-orders")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetPendingOrders()
        {
            try
            {
                var result = await _rentalService.GetPendingOrdersAsync();
                
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
                _logger.LogError(ex, "Error getting pending orders: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi lấy danh sách đơn hàng chờ xử lý",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Staff hủy đơn hàng thủ công
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="reason">Lý do hủy</param>
        /// <returns>Cancellation result</returns>
        [HttpPost("{orderId}/staff-cancel")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> StaffCancelOrder(int orderId, [FromBody] string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Lý do hủy đơn hàng là bắt buộc"
                    });
                }

                var result = await _rentalService.StaffCancelOrderAsync(orderId, reason);
                
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
                _logger.LogError(ex, "Error cancelling order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi hủy đơn hàng",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Staff xác nhận GPLX và cập nhật trạng thái xe
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Staff confirmation request</param>
        /// <returns>Confirmation result</returns>
        [HttpPost("{orderId}/staff-confirm")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> StaffConfirmRental(int orderId, [FromBody] StaffConfirmationDto request)
        {
            try
            {
                var validationError = ValidateModelState();
                if (validationError != null) return validationError;

                var result = await _rentalService.StaffConfirmRentalAsync(orderId, request);
                
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
                _logger.LogError(ex, "Error in staff confirmation for order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Lỗi server khi xác nhận đơn thuê",
                    Error = ex.Message
                });
            }
        }
    }

}
