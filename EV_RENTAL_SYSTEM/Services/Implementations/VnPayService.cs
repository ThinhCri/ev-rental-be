using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VnPayService> _logger;
        private readonly IPaymentService _paymentService;

        public VnPayService(IConfiguration configuration, ILogger<VnPayService> logger, IPaymentService paymentService)
        {
            _configuration = configuration;
            _logger = logger;
            _paymentService = paymentService;
        }

        public async Task<VnPayPaymentResponseDto> CreatePaymentUrlAsync(VnPayPaymentRequestDto request, HttpContext httpContext)
        {
            try
            {
                // Cấu hình VNPay (mô phỏng)
                var vnPaySettings = _configuration.GetSection("VnPaySettings");
                var vnpUrl = vnPaySettings["Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                var vnpTmnCode = vnPaySettings["TmnCode"] ?? "DEMO";
                var vnpHashSecret = vnPaySettings["HashSecret"] ?? "DEMO_SECRET";
                // Tạo ReturnUrl động từ HttpContext
                var scheme = httpContext.Request.Scheme;
                var host = httpContext.Request.Host;
                
                // Kiểm tra nếu đang chạy trên localhost, sử dụng ngrok URL
                var vnpReturnUrl = GetPublicUrl(scheme, host, httpContext);

                // Tạo transaction ID
                var transactionId = $"EV{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";

                // Tạo các tham số cho VNPay (sandbox mode)
                var vnpParams = new Dictionary<string, string>
                {
                    {"vnp_Version", "2.1.0"},
                    {"vnp_Command", "pay"},
                    {"vnp_TmnCode", vnpTmnCode},
                    {"vnp_Amount", ((long)(request.Amount * 100)).ToString()}, // VNPay yêu cầu số tiền nhân 100
                    {"vnp_CurrCode", "VND"},
                    {"vnp_TxnRef", transactionId},
                    {"vnp_OrderInfo", request.Description},
                    {"vnp_OrderType", "other"},
                    {"vnp_Locale", "vn"},
                    {"vnp_ReturnUrl", vnpReturnUrl},
                    {"vnp_IpAddr", "127.0.0.1"},
                    {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
                    {"vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss")} // Thêm thời gian hết hạn
                };

                // Sắp xếp tham số theo thứ tự alphabet
                var sortedParams = vnpParams.OrderBy(x => x.Key).ToList();

                // Tạo query string
                var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));

                // Tạo secure hash
                var secureHash = CreateSecureHash(queryString + vnpHashSecret);

                // Tạo URL thanh toán cuối cùng
                var paymentUrl = $"{vnpUrl}?{queryString}&vnp_SecureHash={secureHash}";

                _logger.LogInformation("Created VNPay payment URL for transaction: {TransactionId}", transactionId);

                return new VnPayPaymentResponseDto
                {
                    Success = true,
                    Message = "Tạo URL thanh toán thành công",
                    PaymentUrl = paymentUrl,
                    TransactionId = transactionId,
                    Amount = request.Amount,
                    Description = request.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment URL");
                return new VnPayPaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi tạo URL thanh toán"
                };
            }
        }

        public async Task<PaymentResponseDto> ProcessCallbackAsync(VnPayCallbackDto callbackData)
        {
            try
            {
                // Kiểm tra secure hash (mô phỏng)
                var vnPaySettings = _configuration.GetSection("VnPaySettings");
                var vnpHashSecret = vnPaySettings["HashSecret"] ?? "DEMO_SECRET";
                
                // Trong thực tế, cần kiểm tra hash từ VNPay
                // Ở đây mô phỏng luôn thành công
                var isValid = true; // Mô phỏng hash hợp lệ

                if (!isValid)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Hash không hợp lệ"
                    };
                }

                // Sử dụng PaymentService để xử lý callback
                var result = await _paymentService.ProcessPaymentCallbackAsync(callbackData);

                _logger.LogInformation("Processed VNPay callback for transaction: {TransactionId}, Success: {Success}", 
                    callbackData.TransactionId, result.Success);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi xử lý callback"
                };
            }
        }

        public async Task<string> CheckTransactionStatusAsync(string transactionId)
        {
            try
            {
                // Mô phỏng kiểm tra trạng thái giao dịch
                // Trong thực tế, cần gọi API VNPay để kiểm tra
                
                // Mô phỏng: 80% thành công, 20% thất bại
                var random = new Random();
                var isSuccess = random.Next(1, 101) <= 80;

                return isSuccess ? "Success" : "Failed";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transaction status for: {TransactionId}", transactionId);
                return "Unknown";
            }
        }

        public string CreateSecureHash(string data)
        {
            try
            {
                var vnPaySettings = _configuration.GetSection("VnPaySettings");
                var vnpHashSecret = vnPaySettings["HashSecret"] ?? "DEMO_SECRET";

                using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(vnpHashSecret));
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToHexString(hashBytes).ToLower();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating secure hash");
                return "DEMO_HASH";
            }
        }

        private string GetPublicUrl(string scheme, HostString host, HttpContext httpContext)
        {
            // Kiểm tra nếu đang chạy trên localhost
            if (host.Host == "localhost" || host.Host == "127.0.0.1")
            {
                // Kiểm tra header X-Forwarded-Proto và X-Forwarded-Host (từ ngrok)
                var forwardedProto = httpContext.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
                var forwardedHost = httpContext.Request.Headers["X-Forwarded-Host"].FirstOrDefault();
                
                if (!string.IsNullOrEmpty(forwardedProto) && !string.IsNullOrEmpty(forwardedHost))
                {
                    // Đang chạy qua ngrok
                    return $"{forwardedProto}://{forwardedHost}/api/payment/vnpay-callback";
                }
                
                // Kiểm tra config có ReturnUrl cố định không
                var vnPaySettings = _configuration.GetSection("VnPaySettings");
                var fixedReturnUrl = vnPaySettings["ReturnUrl"];
                if (!string.IsNullOrEmpty(fixedReturnUrl))
                {
                    return fixedReturnUrl;
                }
                
                // Nếu không có ngrok và không có config, sử dụng localhost (sẽ bị lỗi)
                _logger.LogWarning("VNPay ReturnUrl sẽ bị lỗi vì localhost không accessible từ VNPay. Hãy sử dụng ngrok hoặc cấu hình ReturnUrl trong appsettings.json");
                return $"{scheme}://{host}/api/payment/vnpay-callback";
            }
            
            // Đang chạy trên server thật
            return $"{scheme}://{host}/api/payment/vnpay-callback";
        }
    }
}
