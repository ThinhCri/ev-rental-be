using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Models.VnPay;
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
                var transactionId = $"EV{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";

                Contract? contract = null;
                if (request.OrderId.HasValue)
                {
                    contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(request.OrderId.Value);
                }
                
                if (contract == null)
                {
                    var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);
                    var latestOrder = orders.FirstOrDefault();
                    
                    if (latestOrder != null)
                    {
                        contract = new Contract
                        {
                            OrderId = latestOrder.OrderId,
                            CreatedDate = DateTime.Now,
                            Status = "Active",
                            Deposit = request.Amount * 0.3m, 
                            RentalFee = request.Amount,
                            ExtraFee = 0
                        };
                        
                        await _unitOfWork.Contracts.AddAsync(contract);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new PaymentResponseDto
                        {
                            Success = false,
                            Message = "Không tìm thấy đơn thuê để thanh toán"
                        };
                    }
                }

                var existingPayment = await _unitOfWork.Payments.GetPaymentByContractIdAsync(contract.ContractId);
                if (existingPayment != null)
                {
                    if (existingPayment.Status == "Success")
                    {
                        return new PaymentResponseDto
                        {
                            Success = false,
                            Message = "This order has been paid successfully"
                        };
                    }
                    
                    if (existingPayment.Status == "Pending" || existingPayment.Status == "Unpaid" || existingPayment.Status == "Failed")
                    {
                        return new PaymentResponseDto
                        {
                            Success = false,
                            Message = "Payment is already in progress for this order"
                        };
                    }
                    
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "This order already has a payment record"
                    };
                }

                var payment = new Payment
                {
                    Amount = request.Amount,
                    Status = "Pending",
                    ContractId = contract.ContractId,
                    PaymentDate = DateTime.Now
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var transaction = new Transaction
                {
                    PaymentId = payment.PaymentId,
                    UserId = userId,
                    TransactionDate = DateTime.Now
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Created payment {PaymentId} for user {UserId} with transaction {TransactionId}", 
                    payment.PaymentId, userId, transactionId);

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
                        TransactionId = transactionId,
                        ContractId = payment.ContractId,
                        Note = request.Description
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
                if (string.IsNullOrEmpty(callbackData.TransactionId))
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Transaction ID không hợp lệ"
                    };
                }


                var transaction = await _unitOfWork.Transactions.GetTransactionByTransactionIdAsync(callbackData.TransactionId);
                if (transaction == null)
                {
                    _logger.LogWarning("Transaction not found: {TransactionId}", callbackData.TransactionId);
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy giao dịch"
                    };
                }

                var payment = await _unitOfWork.Payments.GetByIdAsync(transaction.PaymentId);
                if (payment == null)
                {
                    _logger.LogWarning("Payment not found for transaction: {TransactionId}", callbackData.TransactionId);
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy thanh toán"
                    };
                }

                if (payment.Status == "Success")
                {
                    _logger.LogInformation("Payment {PaymentId} already processed successfully", payment.PaymentId);
                    return new PaymentResponseDto
                    {
                        Success = true,
                        Message = "Thanh toán đã được xử lý thành công trước đó",
                        Data = new PaymentDto
                        {
                            PaymentId = payment.PaymentId,
                            PaymentDate = payment.PaymentDate,
                            Amount = payment.Amount ?? 0,
                            Status = payment.Status ?? "Unknown",
                            PaymentMethod = "VNPay",
                            TransactionId = callbackData.TransactionId,
                            ContractId = payment.ContractId,
                            Note = callbackData.Description
                        }
                    };
                }

                var paymentStatus = callbackData.Status switch
                {
                    "00" => "Success",
                    "07" => "Trash",
                    "09" => "Unpaid",
                    _ => "Failed"
                };

                payment.Status = paymentStatus;
                _unitOfWork.Payments.Update(payment);

                if (paymentStatus == "Success")
                {
                    try
                    {
                        var contract = await _unitOfWork.Contracts.GetByIdAsync(payment.ContractId);
                        if (contract != null)
                        {
                            var order = await _unitOfWork.Orders.GetByIdAsync(contract.OrderId);
                            if (order != null)
                            {
                                var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetByOrderIdAsync(order.OrderId);
                                foreach (var orderLicensePlate in orderLicensePlates)
                                {
                                    var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(orderLicensePlate.LicensePlateId);
                                    if (licensePlate != null && licensePlate.Status == "Reserved")
                                    {
                                        licensePlate.Status = "Rented";
                                        _unitOfWork.LicensePlates.Update(licensePlate);
                                        _logger.LogInformation("Updated LicensePlate {LicensePlateId} status to Rented after payment success", 
                                            licensePlate.LicensePlateId);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating LicensePlate status after payment success");
                    }
                }

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
                        Note = callbackData.Description
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
                        Note = "Deposit 30%"
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
                    Note = "Deposit 30%"
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

        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request, int userId)
        {
            try
            {
                var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(request.OrderId);
                if (contract == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy hợp đồng cho đơn hàng này"
                    };
                }

                var existingPayment = await _unitOfWork.Payments.GetPaymentByContractIdAsync(contract.ContractId);
                if (existingPayment != null)
                {
                    if (existingPayment.Status == "Success")
                    {
                        return new PaymentResponseDto
                        {
                            Success = false,
                            Message = "This order has been paid successfully"
                        };
                    }
                    
                    if (existingPayment.Status == "Pending" || existingPayment.Status == "Unpaid" || existingPayment.Status == "Failed")
                    {
                        return new PaymentResponseDto
                        {
                            Success = false,
                            Message = "Payment is already in progress for this order"
                        };
                    }
                    
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "This order already has a payment record"
                    };
                }

                var payment = new Payment
                {
                    Amount = request.Amount,
                    Status = "Pending",
                    ContractId = contract.ContractId,
                    PaymentDate = DateTime.Now,
                    Method = "VNPay",
                    Note = request.Note
                };

                await _unitOfWork.BeginTransactionAsync();
                Transaction? transactionRecord = null;
                try
                {
                    await _unitOfWork.Payments.AddAsync(payment);
                    await _unitOfWork.SaveChangesAsync();

                    transactionRecord = new Transaction
                    {
                        PaymentId = payment.PaymentId,
                        UserId = userId,
                        TransactionDate = DateTime.Now,
                        Status = "Pending"
                    };

                    await _unitOfWork.Transactions.AddAsync(transactionRecord);
                    await _unitOfWork.SaveChangesAsync();
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                _logger.LogInformation("Created payment {PaymentId} for user {UserId} with contract {ContractId}", 
                    payment.PaymentId, userId, contract.ContractId);

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
                        TransactionId = transactionRecord?.TransactionId.ToString() ?? "0",
                        ContractId = payment.ContractId,
                        OrderId = request.OrderId,
                        Note = request.Note
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

        public async Task<PaymentResponseDto> ProcessPaymentSuccessAsync(PaymentResponseModel vnPayResponse)
        {
            try
            {
                // Tìm payment dựa trên OrderId từ VnPay response
                if (!int.TryParse(vnPayResponse.OrderId, out int orderId))
                {
                    _logger.LogWarning("Invalid OrderId format: {OrderId}", vnPayResponse.OrderId);
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "OrderId không hợp lệ"
                    };
                }
                
                // Tìm contract từ orderId
                var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(orderId);
                if (contract == null)
                {
                    _logger.LogWarning("Contract not found for order {OrderId}", orderId);
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy hợp đồng"
                    };
                }

                // Tìm payment từ contract
                var payment = await _unitOfWork.Payments.GetPaymentByContractIdAsync(contract.ContractId);
                if (payment == null)
                {
                    _logger.LogWarning("Payment not found for contract {ContractId}", contract.ContractId);
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy thanh toán"
                    };
                }


                payment.Status = "Success";
                payment.Method = "VNPay";
                _unitOfWork.Payments.Update(payment);

                var transaction = payment.Transactions.FirstOrDefault();
                if (transaction != null)
                {
                    transaction.Status = "Success";
                    _unitOfWork.Transactions.Update(transaction);
                }

                if (contract?.Order != null)
                {
                    contract.Order.Status = "Paid";
                    _unitOfWork.Orders.Update(contract.Order);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Payment {PaymentId} processed successfully", payment.PaymentId);

                return new PaymentResponseDto
                {
                    Success = true,
                    Message = "Thanh toán thành công",
                    Data = new PaymentDto
                    {
                        PaymentId = payment.PaymentId,
                        PaymentDate = payment.PaymentDate,
                        Amount = payment.Amount ?? 0,
                        Status = "Success",
                        PaymentMethod = "VNPay",
                        TransactionId = vnPayResponse.TransactionId,
                        OrderId = orderId,
                        ContractId = payment.ContractId,
                        Note = payment.Note
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment success for VnPay response {TransactionId}", vnPayResponse.TransactionId);
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi xử lý thanh toán thành công"
                };
            }
        }

        public async Task<PaymentStatusDto> GetPaymentStatusByOrderIdAsync(int orderId)
        {
            try
            {
                // Tìm contract từ orderId
                var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(orderId);
                if (contract == null)
                {
                    return new PaymentStatusDto
                    {
                        OrderId = orderId,
                        HasPayment = false,
                        PaymentStatus = "No Payment",
                        PaymentDate = null
                    };
                }

                // Tìm payment của contract này
                var payment = await _unitOfWork.Payments.GetPaymentByContractIdAsync(contract.ContractId);
                if (payment == null)
                {
                    return new PaymentStatusDto
                    {
                        OrderId = orderId,
                        HasPayment = false,
                        PaymentStatus = "No Payment",
                        PaymentDate = null
                    };
                }

                return new PaymentStatusDto
                {
                    OrderId = orderId,
                    HasPayment = true,
                    PaymentStatus = payment.Status ?? "Unknown",
                    PaymentDate = payment.PaymentDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for order {OrderId}", orderId);
                return new PaymentStatusDto
                {
                    OrderId = orderId,
                    HasPayment = false,
                    PaymentStatus = "Error",
                    PaymentDate = null
                };
            }
        }
    }
}
