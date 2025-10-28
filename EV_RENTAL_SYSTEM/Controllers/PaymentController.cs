using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Payment processing controller
    /// </summary>
    public class PaymentController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IVnPayService vnPayService, IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _vnPayService = vnPayService;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create VNPay payment URL endpoint
        /// </summary>
        /// <param name="request">Payment information</param>
        /// <returns>VNPay payment URL</returns>
        [HttpPost("vnpay/create")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] VnPayPaymentRequestDto request)
        {
            try
            {
                var validationError = ValidateModelState();
                if (validationError != null) return validationError;

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var paymentResult = await _paymentService.CreatePaymentAsync(request, userId.Value);
                if (!paymentResult.Success)
                {
                    return BadRequest(paymentResult);
                }

                var vnPayResult = await _vnPayService.CreatePaymentUrlAsync(request, HttpContext);
                
                if (!vnPayResult.Success)
                {
                    return BadRequest(vnPayResult);
                }

                var response = new VnPayPaymentResponseDto
                {
                    Success = true,
                    Message = "Payment created successfully",
                    PaymentUrl = vnPayResult.PaymentUrl,
                    TransactionId = paymentResult.Data?.TransactionId ?? "",
                    Amount = request.Amount,
                    Description = request.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment");
                return ErrorResponse("Error creating payment", 500);
            }
        }

        /// <summary>
        /// VNPay callback handler endpoint
        /// </summary>
        /// <param name="vnp_TxnRef">Transaction reference</param>
        /// <param name="vnp_Amount">Amount</param>
        /// <param name="vnp_OrderInfo">Order information</param>
        /// <param name="vnp_ResponseCode">Response code</param>
        /// <param name="vnp_SecureHash">Secure hash</param>
        /// <returns>Callback processing result</returns>
        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayCallback(
            [FromQuery] string vnp_TxnRef,
            [FromQuery] string vnp_Amount,
            [FromQuery] string vnp_OrderInfo,
            [FromQuery] string vnp_ResponseCode,
            [FromQuery] string vnp_SecureHash)
        {
            try
            {
                var callbackData = new VnPayCallbackDto
                {
                    TransactionId = vnp_TxnRef,
                    Amount = decimal.Parse(vnp_Amount) / 100, // VNPay returns amount multiplied by 100
                    Description = vnp_OrderInfo,
                    Status = vnp_ResponseCode,
                    SecureHash = vnp_SecureHash
                };

                var result = await _vnPayService.ProcessCallbackAsync(callbackData);

                if (result.Success && result.Data?.Status == "Success")
                {
                    return Redirect("https://localhost:3000/payment/success");
                }
                else
                {
                    return Redirect("https://localhost:3000/payment/failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                return Redirect("https://localhost:3000/payment/error");
            }
        }

        /// <summary>
        /// Check transaction status endpoint
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction status</returns>
        [HttpGet("vnpay/status/{transactionId}")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CheckTransactionStatus(string transactionId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByTransactionIdAsync(transactionId);
                
                if (payment == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Transaction not found"
                    });
                }

                return SuccessResponse(payment, "Transaction status retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transaction status for: {TransactionId}", transactionId);
                return ErrorResponse("Error checking transaction status", 500);
            }
        }

        /// <summary>
        /// Get current user's payment history endpoint
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Payment history</returns>
        [HttpGet("history")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetPaymentHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Validate pagination parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _paymentService.GetPaymentHistoryAsync(userId.Value, pageNumber, pageSize);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history");
                return ErrorResponse("Error getting payment history", 500);
            }
        }
    }
}
