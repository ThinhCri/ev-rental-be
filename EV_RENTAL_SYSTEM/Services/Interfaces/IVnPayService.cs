using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IVnPayService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPay
        /// </summary>
        /// <param name="request">Thông tin thanh toán</param>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="transactionId">Transaction ID đã tạo</param>
        /// <returns>URL thanh toán và thông tin giao dịch</returns>
        Task<VnPayPaymentResponseDto> CreatePaymentUrlAsync(VnPayPaymentRequestDto request, HttpContext httpContext, string transactionId);

        /// <summary>
        /// Xử lý callback từ VNPay
        /// </summary>
        /// <param name="callbackData">Dữ liệu callback</param>
        /// <returns>Kết quả xử lý callback</returns>
        Task<PaymentResponseDto> ProcessCallbackAsync(VnPayCallbackDto callbackData);

        /// <summary>
        /// Kiểm tra trạng thái giao dịch
        /// </summary>
        /// <param name="transactionId">ID giao dịch</param>
        /// <returns>Trạng thái giao dịch</returns>
        Task<string> CheckTransactionStatusAsync(string transactionId);

        /// <summary>
        /// Tạo mã hash bảo mật
        /// </summary>
        /// <param name="data">Dữ liệu cần hash</param>
        /// <returns>Mã hash</returns>
        string CreateSecureHash(string data);
    }
}
