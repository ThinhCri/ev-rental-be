using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(VnPayPaymentRequestDto request, int userId)
        {
            try
            {
                // Tạo Payment record
                var payment = new Payment
                {
                    Amount = request.Amount,
                    Status = "Pending",
                    ContractId = request.ContractId ?? 1, // Tạm thời sử dụng ContractId = 1
                    PaymentDate = DateTime.Now
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // Tạo Transaction record
                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    PaymentId = payment.PaymentId,
                    UserId = userId,
                    TransactionDate = DateTime.Now
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Created payment {PaymentId} for user {UserId}", payment.PaymentId, userId);

                return new PaymentResponseDto
                {
                    Success = true,
                    Message = "Tạo thanh toán thành công",
                    Data = new PaymentDto
                    {
                        PaymentId = payment.PaymentId,
                        PaymentDate = payment.PaymentDate,
                        Amount = payment.Amount ?? 0,
                        Status = payment.Status ?? "Pending",
                        PaymentMethod = "VNPay",
                        TransactionId = transaction.TransactionId.ToString(),
                        ContractId = payment.ContractId,
                        Description = request.Description
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for user {UserId}", userId);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi tạo thanh toán"
                };
            }
        }

        public async Task<PaymentResponseDto> ProcessPaymentCallbackAsync(VnPayCallbackDto callbackData)
        {
            try
            {
                // Tìm payment theo transaction ID
                var transaction = await _unitOfWork.Transactions.GetTransactionByTransactionIdAsync(callbackData.TransactionId);
                if (transaction == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy giao dịch"
                    };
                }

                var payment = await _unitOfWork.Payments.GetByIdAsync(transaction.PaymentId);
                if (payment == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy thanh toán"
                    };
                }

                // Cập nhật trạng thái payment
                var paymentStatus = callbackData.Status switch
                {
                    "00" => "Success",
                    "07" => "Trash",
                    "09" => "Unpaid",
                    _ => "Failed"
                };

                payment.Status = paymentStatus;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated payment {PaymentId} status to {Status}", payment.PaymentId, paymentStatus);

                return new PaymentResponseDto
                {
                    Success = true,
                    Message = paymentStatus == "Success" ? "Thanh toán thành công" : "Thanh toán thất bại",
                    Data = new PaymentDto
                    {
                        PaymentId = payment.PaymentId,
                        PaymentDate = payment.PaymentDate,
                        Amount = payment.Amount ?? 0,
                        Status = payment.Status ?? "Unknown",
                        PaymentMethod = "VNPay",
                        TransactionId = callbackData.TransactionId,
                        ContractId = payment.ContractId,
                        Description = callbackData.Description
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback for transaction {TransactionId}", callbackData.TransactionId);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi xử lý callback thanh toán"
                };
            }
        }

        public async Task<PaymentListResponseDto> GetPaymentHistoryAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetPaymentsByUserIdAsync(userId);
                var totalCount = payments.Count();

                var pagedPayments = payments
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PaymentDto
                    {
                        PaymentId = p.PaymentId,
                        PaymentDate = p.PaymentDate,
                        Amount = p.Amount ?? 0,
                        Status = p.Status ?? "Unknown",
                        PaymentMethod = "VNPay",
                        TransactionId = p.Transactions.FirstOrDefault()?.TransactionId.ToString() ?? "",
                        ContractId = p.ContractId,
                        Description = "Thanh toán thuê xe điện"
                    })
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return new PaymentListResponseDto
                {
                    Success = true,
                    Message = "Lấy lịch sử thanh toán thành công",
                    Data = pagedPayments,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history for user {UserId}", userId);
                return new PaymentListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy lịch sử thanh toán",
                    Data = new List<PaymentDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }
        }

        public async Task<PaymentDto?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetPaymentByTransactionIdAsync(transactionId);
                if (payment == null)
                    return null;

                return new PaymentDto
                {
                    PaymentId = payment.PaymentId,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount ?? 0,
                    Status = payment.Status ?? "Unknown",
                    PaymentMethod = "VNPay",
                    TransactionId = transactionId,
                    ContractId = payment.ContractId,
                    Description = "Thanh toán thuê xe điện"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment by transaction ID {TransactionId}", transactionId);
                return null;
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
                if (payment == null)
                    return false;

                payment.Status = status;
                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated payment {PaymentId} status to {Status}", paymentId, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment {PaymentId} status to {Status}", paymentId, status);
                return false;
            }
        }
    }
}
