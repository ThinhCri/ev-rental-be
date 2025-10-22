using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho thông tin thanh toán
    /// </summary>
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

    /// <summary>
    /// DTO cho request tạo thanh toán VNPay
    /// </summary>
    public class VnPayPaymentRequestDto
    {
        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1,000 VND")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Mô tả thanh toán là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Mô tả không được quá 255 ký tự")]
        public string Description { get; set; } = string.Empty;

        public int? OrderId { get; set; }
        public int? ContractId { get; set; }
    }

    /// <summary>
    /// DTO cho response tạo thanh toán VNPay
    /// </summary>
    public class VnPayPaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho callback VNPay
    /// </summary>
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

    /// <summary>
    /// DTO cho response API
    /// </summary>
    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách thanh toán
    /// </summary>
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

    /// <summary>
    /// DTO cho request tạo thanh toán
    /// </summary>
    public class CreatePaymentRequestDto
    {
        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Mô tả thanh toán là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Mô tả không được quá 255 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên khách hàng không được quá 100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order ID là bắt buộc")]
        public int OrderId { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO cho payment status
    /// </summary>
    public class PaymentStatusDto
    {
        public int OrderId { get; set; }
        public bool HasPayment { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
    }
}

