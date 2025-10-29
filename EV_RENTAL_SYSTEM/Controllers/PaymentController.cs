using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Models.VnPay;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using EV_RENTAL_SYSTEM.Services.Vnpay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web;

namespace EV_RENTAL_SYSTEM.Controllers
{
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

        [HttpPost("create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentUrlVnpay([FromBody] CreatePaymentRequestDto request)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return ErrorResponse("User not authenticated", 401);
                }

                var vnPayModel = new PaymentInformationModel
                {
                    OrderType = "other",
                    Amount = (double)request.Amount,
                    OrderDescription = request.Description,
                    Name = request.CustomerName,
                    OrderId = request.OrderId
                };

                var url = _vnPayService.CreatePaymentUrl(vnPayModel, HttpContext);
                
                var result = await _paymentService.CreatePaymentAsync(request, userId.Value);
                
                return SuccessResponse(new { PaymentUrl = url, PaymentId = result.Data?.PaymentId }, "Payment URL created successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error creating payment URL: {ex.Message}", 500);
            }
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);
                
                _logger.LogInformation("VnPay callback received - Success: {Success}, ResponseCode: {ResponseCode}, Message: {Message}", 
                    response.Success, response.VnPayResponseCode, response.Message);
                
                if (response.Success)
                {
                    var result = await _paymentService.ProcessPaymentSuccessAsync(response);
                    
                    var frontendUrl = "http://localhost:5173/history?payment=success";
                    return Redirect(frontendUrl);
                }
                else
                {
                    _logger.LogWarning("Payment failed - ResponseCode: {ResponseCode}, Message: {Message}", 
                        response.VnPayResponseCode, response.Message);
                    
                    var frontendUrl = "http://localhost:5173/history?payment=failed";
                    return Redirect(frontendUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VnPay callback");
                var frontendUrl = "http://localhost:5173/history?payment=failed";
                return Redirect(frontendUrl);
            }
        }

        [HttpGet("payment-status/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusByOrderIdAsync(orderId);
                return SuccessResponse(result, "Payment status retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error getting payment status: {ex.Message}", 500);
            }
        }
    }
}