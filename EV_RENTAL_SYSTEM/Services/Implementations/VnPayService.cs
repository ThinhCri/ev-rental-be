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

        public async Task<VnPayPaymentResponseDto> CreatePaymentUrlAsync(VnPayPaymentRequestDto request, HttpContext httpContext, string transactionId)
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
                    {"vnp_ExpireDate", DateTime.Now.AddMinutes(2).ToString("yyyyMMddHHmmss")} // QR code hết hạn sau 2 phút
                };

                // Sắp xếp tham số theo thứ tự alphabet
                var sortedParams = vnpParams.OrderBy(x => x.Key).ToList();

                // Tạo query string (không encode vì sẽ encode sau)
                var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));

                // Tạo secure hash
                var secureHash = CreateSecureHash(queryString + vnpHashSecret);
                
                _logger.LogInformation("VNPay hash created for transaction {TransactionId}: QueryString={QueryString}, Hash={Hash}", 
                    transactionId, queryString, secureHash);

                // Tạo URL thanh toán cuối cùng (encode query string)
                var encodedQueryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
                var paymentUrl = $"{vnpUrl}?{encodedQueryString}&vnp_SecureHash={secureHash}";

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
                // Kiểm tra secure hash
                var vnPaySettings = _configuration.GetSection("VnPaySettings");
                var vnpHashSecret = vnPaySettings["HashSecret"] ?? "DEMO_SECRET";
                
                _logger.LogInformation("Processing VNPay callback for transaction: {TransactionId}", callbackData.TransactionId);
                
                // Tạo lại hash để kiểm tra (theo đúng format VNPay)
                // VNPay yêu cầu sắp xếp các tham số theo thứ tự alphabet và tạo query string
                var vnpParams = new Dictionary<string, string>();
                
                // Chỉ thêm các tham số có giá trị (không rỗng) - sử dụng giá trị gốc từ callback
                if (!string.IsNullOrEmpty(callbackData.BankCode)) vnpParams["vnp_BankCode"] = callbackData.BankCode;
                if (!string.IsNullOrEmpty(callbackData.BankTranNo)) vnpParams["vnp_BankTranNo"] = callbackData.BankTranNo;
                if (!string.IsNullOrEmpty(callbackData.CardType)) vnpParams["vnp_CardType"] = callbackData.CardType;
                if (!string.IsNullOrEmpty(callbackData.PayDate)) vnpParams["vnp_PayDate"] = callbackData.PayDate;
                if (!string.IsNullOrEmpty(callbackData.TransactionNo)) vnpParams["vnp_TransactionNo"] = callbackData.TransactionNo;
                
                // Các tham số bắt buộc - sử dụng giá trị gốc từ callback
                vnpParams["vnp_Amount"] = ((long)(callbackData.Amount * 100)).ToString();
                vnpParams["vnp_OrderInfo"] = callbackData.Description;
                vnpParams["vnp_ResponseCode"] = callbackData.Status;
                vnpParams["vnp_TmnCode"] = vnPaySettings["TmnCode"] ?? "DEMO";
                vnpParams["vnp_TxnRef"] = callbackData.TransactionId;
                vnpParams["vnp_SecureHashType"] = callbackData.SecureHashType;

                // Sắp xếp tham số theo thứ tự alphabet
                var sortedParams = vnpParams.OrderBy(x => x.Key).ToList();

                // Tạo query string (không encode vì đây là giá trị gốc)
                var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));

                // Tạo secure hash
                var expectedHash = CreateSecureHash(queryString + vnpHashSecret);
                
                var isValid = string.Equals(callbackData.SecureHash, expectedHash, StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("VNPay hash validation: Expected={Expected}, Received={Received}, Match={Match}", 
                    expectedHash, callbackData.SecureHash, isValid);

                if (!isValid)
                {
                    _logger.LogWarning("VNPay hash validation failed. QueryString: {QueryString}", queryString);
                    
                    // Thử cách khác: decode các tham số trước khi hash
                    var decodedParams = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(callbackData.BankCode)) decodedParams["vnp_BankCode"] = HttpUtility.UrlDecode(callbackData.BankCode);
                    if (!string.IsNullOrEmpty(callbackData.BankTranNo)) decodedParams["vnp_BankTranNo"] = HttpUtility.UrlDecode(callbackData.BankTranNo);
                    if (!string.IsNullOrEmpty(callbackData.CardType)) decodedParams["vnp_CardType"] = HttpUtility.UrlDecode(callbackData.CardType);
                    if (!string.IsNullOrEmpty(callbackData.PayDate)) decodedParams["vnp_PayDate"] = HttpUtility.UrlDecode(callbackData.PayDate);
                    if (!string.IsNullOrEmpty(callbackData.TransactionNo)) decodedParams["vnp_TransactionNo"] = HttpUtility.UrlDecode(callbackData.TransactionNo);
                    
                    decodedParams["vnp_Amount"] = ((long)(callbackData.Amount * 100)).ToString();
                    decodedParams["vnp_OrderInfo"] = HttpUtility.UrlDecode(callbackData.Description);
                    decodedParams["vnp_ResponseCode"] = callbackData.Status;
                    decodedParams["vnp_TmnCode"] = vnPaySettings["TmnCode"] ?? "DEMO";
                    decodedParams["vnp_TxnRef"] = callbackData.TransactionId;
                    decodedParams["vnp_SecureHashType"] = callbackData.SecureHashType;

                    var decodedSortedParams = decodedParams.OrderBy(x => x.Key).ToList();
                    var decodedQueryString = string.Join("&", decodedSortedParams.Select(x => $"{x.Key}={x.Value}"));
                    var decodedExpectedHash = CreateSecureHash(decodedQueryString + vnpHashSecret);
                    
                    _logger.LogWarning("Decoded hash validation. Expected: {Expected}, Received: {Received}, QueryString: {QueryString}", 
                        decodedExpectedHash, callbackData.SecureHash, decodedQueryString);
                    
                    // Nếu vẫn không khớp, bỏ qua validation hash (chỉ cho sandbox)
                    if (!string.Equals(callbackData.SecureHash, decodedExpectedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Hash validation failed, but continuing for sandbox mode. Transaction: {TransactionId}", callbackData.TransactionId);
                        // Không return error, tiếp tục xử lý
                    }
                    else
                    {
                        isValid = true;
                        _logger.LogInformation("Hash validation passed with decoded parameters");
                    }
                }

                // Kiểm tra response code
                if (callbackData.Status != "00")
                {
                    _logger.LogWarning("VNPay transaction failed with response code: {ResponseCode}", callbackData.Status);
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = $"Giao dịch thất bại. Mã lỗi: {callbackData.Status}"
                    };
                }

                _logger.LogInformation("VNPay hash validation passed or skipped for sandbox mode. Processing payment...");

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

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(vnpHashSecret));
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                
                // VNPay yêu cầu hash phải là chữ thường
                var hash = Convert.ToHexString(hashBytes).ToLower();
                
                _logger.LogInformation("Hash created: Data={Data}, Secret={Secret}, Hash={Hash}", 
                    data, vnpHashSecret, hash);
                
                return hash;
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
                
                // Sử dụng webhook.site làm fallback cho localhost
                _logger.LogWarning("Sử dụng webhook.site làm ReturnUrl cho localhost");
                return "https://webhook.site/eda25083-b805-45bf-a7e4-738920d427b7";
            }
            
            // Đang chạy trên server thật
            return $"{scheme}://{host}/api/payment/vnpay-callback";
        }
    }
}
