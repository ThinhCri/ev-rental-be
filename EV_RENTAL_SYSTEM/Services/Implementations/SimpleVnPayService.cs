using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    /// <summary>
    /// VnPayService đơn giản cho testing
    /// </summary>
    public class SimpleVnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SimpleVnPayService> _logger;
        private readonly IPaymentService _paymentService;

        public SimpleVnPayService(IConfiguration configuration, ILogger<SimpleVnPayService> logger, IPaymentService paymentService)
        {
            _configuration = configuration;
            _logger = logger;
            _paymentService = paymentService;
        }

        public async Task<VnPayPaymentResponseDto> CreatePaymentUrlAsync(VnPayPaymentRequestDto request, HttpContext httpContext, string transactionId)
        {
            try
            {
                // Tạo payment trong database trước
                var paymentResult = await _paymentService.CreatePaymentAsync(request, 1); // Mock userId = 1

                if (!paymentResult.Success)
                {
                    return new VnPayPaymentResponseDto
                    {
                        Success = false,
                        Message = "Không thể tạo payment trong database"
                    };
                }

                // Tạo URL thanh toán giả lập (cho testing)
                var paymentUrl = $"https://webhook.site/eda25083-b805-45bf-a7e4-738920d427b7?amount={request.Amount}&orderId={request.OrderId}&transactionId={transactionId}&description={HttpUtility.UrlEncode(request.Description)}";

                _logger.LogInformation("Created mock VNPay payment URL for transaction: {TransactionId}", transactionId);

                return new VnPayPaymentResponseDto
                {
                    Success = true,
                    Message = "Tạo URL thanh toán thành công (Mock mode)",
                    PaymentUrl = paymentUrl,
                    TransactionId = transactionId,
                    Amount = request.Amount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment URL");
                return new VnPayPaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi tạo URL thanh toán: " + ex.Message
                };
            }
        }

        public async Task<PaymentResponseDto> ProcessCallbackAsync(VnPayCallbackDto callbackData)
        {
            try
            {
                var transactionId = callbackData.TransactionId;
                var responseCode = callbackData.ResponseCode;
                var amount = callbackData.Amount;

                var isSuccess = responseCode == "00";

                // Cập nhật payment status
                if (!string.IsNullOrEmpty(transactionId))
                {
                    await _paymentService.ProcessPaymentCallbackAsync(callbackData);
                }

                return new PaymentResponseDto
                {
                    Success = isSuccess,
                    Message = isSuccess ? "Thanh toán thành công" : "Thanh toán thất bại"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi xử lý callback: " + ex.Message
                };
            }
        }

        public async Task<string> CheckTransactionStatusAsync(string transactionId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByTransactionIdAsync(transactionId);
                return payment?.Status ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transaction status for: {TransactionId}", transactionId);
                return "Unknown";
            }
        }

        public string CreateSecureHash(string data)
        {
            // Mock hash cho testing
            return "MOCK_HASH_FOR_TESTING";
        }
    }
}
