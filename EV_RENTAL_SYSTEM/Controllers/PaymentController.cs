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
        
        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create-payment-url")]
    
        public IActionResult CreatePaymentUrlVnpay([FromBody] PaymentInformationModel model)
        {
            var validationError = ValidateModelState();
            if (validationError != null) return validationError;

            try
            {
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                return SuccessResponse(url, "Payment URL created successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error creating payment URL: {ex.Message}", 500);
            }
        }

        [HttpGet("payment-callback")]
 
        public IActionResult PaymentCallbackVnpay()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);
                return SuccessResponse(response, "Payment callback processed successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error processing payment callback: {ex.Message}", 500);
            }
        }

    
    }
}