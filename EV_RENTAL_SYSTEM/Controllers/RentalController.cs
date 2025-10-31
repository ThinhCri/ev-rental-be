using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_RENTAL_SYSTEM.Controllers
{
    public class RentalController : BaseController
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(IRentalService rentalService, ILogger<RentalController> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

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
                    Message = "Server error retrieving rental list",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error retrieving rental information",
                    Error = ex.Message
                });
            }
        }

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
                        Message = "Invalid token" 
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
                    Message = "Server error retrieving rental history",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error retrieving rental details",
                    Error = ex.Message
                });
            }
        }

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
                        Message = "Invalid search data"
                    });
                }

                if (searchDto.StartTime >= searchDto.EndTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "End time must be after start time"
                    });
                }

                if (searchDto.StartTime < DateTime.Now.AddMinutes(-5))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Start time cannot be in the past"
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
                    Message = "Server error searching available vehicles",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("with-payment")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateRentalWithPayment([FromForm] CreateRentalDto createDto)
        {
            try
            {
                _logger.LogInformation("CreateRentalWithPayment called - VehicleId: {VehicleId}, IsBookingForOthers: {IsBookingForOthers}", 
                    createDto?.VehicleId, createDto?.IsBookingForOthers);

                if (createDto == null)
                {
                    _logger.LogWarning("CreateRentalWithPayment: createDto is null");
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid or malformed JSON",
                        Hint = "Check commas, brackets and JSON format"
                    });
                }

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
                        Message = "Invalid data",
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

                if (createDto.StartTime >= createDto.EndTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "End time must be after start time"
                    });
                }

                if (createDto.StartTime < DateTime.Now.AddMinutes(-5))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Start time cannot be in the past"
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
                        Message = "Invalid token" 
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
                    Message = "Rental information is ready. Please complete payment.",
                    Data = result.Data,
                    RequiresPayment = true,
                    PaymentRequired = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental with payment: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error creating rental",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateRental([FromForm] CreateRentalDto createDto)
        {
            try
            {
                _logger.LogInformation("CreateRental called - VehicleId: {VehicleId}, IsBookingForOthers: {IsBookingForOthers}", 
                    createDto?.VehicleId, createDto?.IsBookingForOthers);

                if (createDto == null)
                {
                    _logger.LogWarning("CreateRental: createDto is null");
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid or malformed JSON",
                        Hint = "Check commas, brackets and JSON format"
                    });
                }

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
                        Message = "Invalid data",
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

                if (createDto.StartTime >= createDto.EndTime)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "End time must be after start time"
                    });
                }

                if (createDto.StartTime < DateTime.Now.AddMinutes(-5))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Start time cannot be in the past"
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
                        Message = "Invalid token" 
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
                    Message = "Rental created successfully",
                    Data = result.Data,
                    OrderId = result.OrderId,
                    ContractId = result.ContractId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental: {Error}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error creating rental",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateRental(int id, [FromForm] UpdateRentalDto updateDto)
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
                    Message = "Server error updating rental",
                    Error = ex.Message
                });
            }
        }

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
                var userRole = GetCurrentUserRole();
                var result = await _rentalService.CancelRentalAsync(id, userId, userRole);

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
                    Message = "Server error cancelling rental",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error completing rental",
                    Error = ex.Message
                });
            }
        }

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
                        Message = "Invalid search data"
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
                    Message = "Server error searching rentals",
                    Error = ex.Message
                });
            }
        }

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
                        Message = "End time must be after start time"
                    });
                }

                var cost = await _rentalService.CalculateRentalCostAsync(vehicleId, startTime, endTime);
                
                return Ok(new
                {
                    Success = true,
                    Message = "Cost calculated successfully",
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
                    Message = "Server error calculating cost",
                    Error = ex.Message
                });
            }
        }

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
                        Message = "End time must be after start time"
                    });
                }

                var isAvailable = await _rentalService.IsVehicleAvailableAsync(vehicleId, startTime, endTime);
                
                return Ok(new
                {
                    Success = true,
                    Message = "Availability check completed successfully",
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
                    Message = "Server error checking availability",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error retrieving contract information",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error confirming contract",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error retrieving pending orders",
                    Error = ex.Message
                });
            }
        }

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
                        Message = "Cancellation reason is required"
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
                    Message = "Server error cancelling order",
                    Error = ex.Message
                });
            }
        }

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
                    Message = "Server error confirming rental",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{orderId}/handover")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> HandoverVehicle(int orderId)
        {
            try
            {
                var result = await _rentalService.HandoverVehicleAsync(orderId);
                
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
                _logger.LogError(ex, "Error handing over vehicle for order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error handing over vehicle",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{orderId}/handover-details")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> HandoverVehicleWithDetails(int orderId, [FromForm] HandoverVehicleDto handoverDto)
        {
            try
            {
                if (handoverDto == null)
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

                var result = await _rentalService.HandoverVehicleWithDetailsAsync(orderId, handoverDto);
                
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
                _logger.LogError(ex, "Error handing over vehicle with details for order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error handing over vehicle",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{orderId}/return")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> ReturnVehicle(int orderId, [FromForm] ReturnVehicleDto returnDto)
        {
            try
            {
                if (returnDto == null)
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

                var result = await _rentalService.ReturnVehicleAsync(orderId, returnDto);
                
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
                _logger.LogError(ex, "Error returning vehicle for order {OrderId}: {Error}", orderId, ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Server error returning vehicle",
                    Error = ex.Message
                });
            }
        }
    }
}
