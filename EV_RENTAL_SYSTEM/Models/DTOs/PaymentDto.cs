using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public int? ContractId { get; set; }
        public string? Note{ get; set; }
    }

    public class VnPayPaymentRequestDto
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(1000, double.MaxValue, ErrorMessage = "Amount must be greater than 1,000 VND")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment description is required")]
        [MaxLength(255, ErrorMessage = "Description must not exceed 255 characters")]
        public string Description { get; set; } = string.Empty;

        public int? OrderId { get; set; }
        public int? ContractId { get; set; }
    }

    public class VnPayPaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class VnPayCallbackDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SecureHash { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string BankTranNo { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string PayDate { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public string SecureHashType { get; set; } = string.Empty;
    }

    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentDto? Data { get; set; }
    }

    public class PaymentListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<PaymentDto> Data { get; set; } = new List<PaymentDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class CreatePaymentRequestDto
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment description is required")]
        [MaxLength(255, ErrorMessage = "Description must not exceed 255 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer name is required")]
        [MaxLength(100, ErrorMessage = "Customer name must not exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }

        [MaxLength(500, ErrorMessage = "Note must not exceed 500 characters")]
        public string? Note { get; set; }
    }

    public class PaymentStatusDto
    {
        public int OrderId { get; set; }
        public bool HasPayment { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
    }
}
