using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Models.VnPay;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(VnPayPaymentRequestDto request, int userId);
        Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request, int userId);
        Task<PaymentResponseDto> ProcessPaymentCallbackAsync(VnPayCallbackDto callbackData);
        Task<PaymentResponseDto> ProcessPaymentSuccessAsync(PaymentResponseModel vnPayResponse);
        Task<PaymentListResponseDto> GetPaymentHistoryAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<PaymentDto?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, string status);
        Task<PaymentStatusDto> GetPaymentStatusByOrderIdAsync(int orderId);
    }
}

