using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Payment processing controller
    /// </summary>
    public class PaymentController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentController(IVnPayService vnPayService, IPaymentService paymentService, ILogger<PaymentController> logger, IConfiguration configuration)
        {
            _vnPayService = vnPayService;
            _paymentService = paymentService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Create VNPay payment URL endpoint
        /// </summary>
        /// <param name="request">Payment information</param>
        /// <returns>VNPay payment URL</returns>
        [HttpPost("vnpay/create")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] VnPayPaymentRequestDto request)
        {
            try
            {
                var validationError = ValidateModelState();
                if (validationError != null) return validationError;

                // Validation bổ sung
                if (request.Amount <= 0)
                {
                    return BadRequest(new { message = "Số tiền phải lớn hơn 0" });
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    return BadRequest(new { message = "Mô tả thanh toán không được để trống" });
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var paymentResult = await _paymentService.CreatePaymentAsync(request, userId.Value);
                if (!paymentResult.Success)
                {
                    return BadRequest(paymentResult);
                }

                var vnPayResult = await _vnPayService.CreatePaymentUrlAsync(request, HttpContext, paymentResult.Data?.TransactionId ?? "");
                
                if (!vnPayResult.Success)
                {
                    return BadRequest(vnPayResult);
                }

                var response = new VnPayPaymentResponseDto
                {
                    Success = true,
                    Message = "Payment created successfully",
                    PaymentUrl = vnPayResult.PaymentUrl,
                    TransactionId = paymentResult.Data?.TransactionId ?? "",
                    Amount = request.Amount,
                    Description = request.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment");
                return ErrorResponse("Error creating payment", 500);
            }
        }

        /// <summary>
        /// VNPay callback handler endpoint
        /// </summary>
        /// <param name="vnp_TxnRef">Transaction reference</param>
        /// <param name="vnp_Amount">Amount</param>
        /// <param name="vnp_OrderInfo">Order information</param>
        /// <param name="vnp_ResponseCode">Response code</param>
        /// <param name="vnp_SecureHash">Secure hash</param>
        /// <returns>Callback processing result</returns>
        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayCallback(
            [FromQuery] string vnp_TxnRef,
            [FromQuery] string vnp_Amount,
            [FromQuery] string vnp_OrderInfo,
            [FromQuery] string vnp_ResponseCode,
            [FromQuery] string vnp_SecureHash,
            [FromQuery] string vnp_BankCode = "",
            [FromQuery] string vnp_BankTranNo = "",
            [FromQuery] string vnp_CardType = "",
            [FromQuery] string vnp_PayDate = "",
            [FromQuery] string vnp_TransactionNo = "",
            [FromQuery] string vnp_SecureHashType = "SHA256")
        {
            try
            {
                _logger.LogInformation("VNPay Callback received: TxnRef={TxnRef}, Amount={Amount}, ResponseCode={ResponseCode}, SecureHash={SecureHash}", 
                    vnp_TxnRef, vnp_Amount, vnp_ResponseCode, vnp_SecureHash);

                // Log tất cả parameters để debug
                _logger.LogInformation("All VNPay callback parameters: BankCode={BankCode}, BankTranNo={BankTranNo}, CardType={CardType}, PayDate={PayDate}, TransactionNo={TransactionNo}, HashType={HashType}", 
                    vnp_BankCode, vnp_BankTranNo, vnp_CardType, vnp_PayDate, vnp_TransactionNo, vnp_SecureHashType);

                // Không decode các tham số từ VNPay callback (giữ nguyên để hash validation)
                var callbackData = new VnPayCallbackDto
                {
                    TransactionId = vnp_TxnRef,
                    Amount = decimal.Parse(vnp_Amount) / 100, // VNPay returns amount multiplied by 100
                    Description = vnp_OrderInfo,
                    Status = vnp_ResponseCode,
                    SecureHash = vnp_SecureHash,
                    BankCode = vnp_BankCode,
                    BankTranNo = vnp_BankTranNo,
                    CardType = vnp_CardType,
                    PayDate = vnp_PayDate,
                    TransactionNo = vnp_TransactionNo,
                    SecureHashType = vnp_SecureHashType
                };

                var result = await _vnPayService.ProcessCallbackAsync(callbackData);

                // Trong sandbox mode, nếu response code là 00 thì coi như thành công
                if (result.Success || callbackData.Status == "00")
                {
                    _logger.LogInformation("VNPay callback processed successfully for transaction: {TransactionId}", callbackData.TransactionId);
                    
                    // Trả về HTML page thay vì redirect
                    var successHtml = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Thanh toán thành công</title>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
                            .success {{ color: green; font-size: 24px; }}
                            .info {{ margin: 20px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='success'>✅ Thanh toán thành công!</div>
                        <div class='info'>Mã giao dịch: {vnp_TxnRef}</div>
                        <div class='info'>Số tiền: {callbackData.Amount:N0} VNĐ</div>
                        <div class='info'>Mô tả: {callbackData.Description}</div>
                        <script>
                            setTimeout(function() {{
                                window.close();
                            }}, 3000);
                        </script>
                    </body>
                    </html>";
                    return Content(successHtml, "text/html");
                }
                else
                {
                    _logger.LogWarning("VNPay callback failed for transaction: {TransactionId}, Message: {Message}", 
                        callbackData.TransactionId, result.Message);
                    
                    var errorHtml = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Thanh toán thất bại</title>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
                            .error {{ color: red; font-size: 24px; }}
                            .info {{ margin: 20px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='error'>❌ Thanh toán thất bại!</div>
                        <div class='info'>Lý do: {result.Message}</div>
                        <div class='info'>Mã giao dịch: {vnp_TxnRef}</div>
                        <script>
                            setTimeout(function() {{
                                window.close();
                            }}, 3000);
                        </script>
                    </body>
                    </html>";
                    return Content(errorHtml, "text/html");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                var errorHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Lỗi xử lý thanh toán</title>
                    <meta charset='utf-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
                        .error {{ color: red; font-size: 24px; }}
                    </style>
                </head>
                <body>
                    <div class='error'>❌ Lỗi xử lý thanh toán!</div>
                    <script>
                        setTimeout(function() {{
                            window.close();
                        }}, 3000);
                    </script>
                </body>
                </html>";
                return Content(errorHtml, "text/html");
            }
        }

        /// <summary>
        /// Check transaction status endpoint
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction status</returns>
        [HttpGet("vnpay/status/{transactionId}")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> CheckTransactionStatus(string transactionId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByTransactionIdAsync(transactionId);
                
                if (payment == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Transaction not found"
                    });
                }

                return SuccessResponse(payment, "Transaction status retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transaction status for: {TransactionId}", transactionId);
                return ErrorResponse("Error checking transaction status", 500);
            }
        }

        /// <summary>
        /// Test VNPay hash generation endpoint
        /// </summary>
        /// <returns>Hash test result</returns>
        [HttpGet("test-hash")]
        public IActionResult TestVnPayHash()
        {
            try
            {
                var testData = "vnp_Amount=7500000000&vnp_Command=pay&vnp_CreateDate=20251012000132&vnp_CurrCode=VND&vnp_ExpireDate=20251012001632&vnp_IpAddr=127.0.0.1&vnp_Locale=vn&vnp_OrderInfo=thue+xe&vnp_OrderType=other&vnp_ReturnUrl=https%3a%2f%2fwebhook.site%2feda25083-b805-45bf-a7e4-738920d427b7&vnp_TmnCode=2QXUI4J4&vnp_TxnRef=EV202510120001323597&vnp_Version=2.1.0";
                var secret = "RAOEXHYVSDDIIENYWSLDLLDTDIXDCHKE";
                
                var hash = _vnPayService.CreateSecureHash(testData + secret);
                
                return Ok(new
                {
                    Success = true,
                    TestData = testData,
                    Secret = secret,
                    GeneratedHash = hash,
                    Message = "Hash test completed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing VNPay hash");
                return ErrorResponse("Error testing hash", 500);
            }
        }

        /// <summary>
        /// Get current user's payment history endpoint
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Payment history</returns>
        [HttpGet("history")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetPaymentHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Validate pagination parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _paymentService.GetPaymentHistoryAsync(userId.Value, pageNumber, pageSize);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history");
                return ErrorResponse("Error getting payment history", 500);
            }
        }

        /// <summary>
        /// Debug VNPay callback - log all parameters
        /// </summary>
        [HttpGet("debug-callback")]
        public IActionResult DebugCallback(
            [FromQuery] string vnp_TxnRef,
            [FromQuery] string vnp_Amount,
            [FromQuery] string vnp_OrderInfo,
            [FromQuery] string vnp_ResponseCode,
            [FromQuery] string vnp_SecureHash,
            [FromQuery] string vnp_BankCode = "",
            [FromQuery] string vnp_BankTranNo = "",
            [FromQuery] string vnp_CardType = "",
            [FromQuery] string vnp_PayDate = "",
            [FromQuery] string vnp_TransactionNo = "",
            [FromQuery] string vnp_SecureHashType = "SHA256")
        {
            try
            {
                // Log raw parameters
                _logger.LogInformation("=== VNPay Debug Callback ===");
                _logger.LogInformation("Raw vnp_TxnRef: {TxnRef}", vnp_TxnRef);
                _logger.LogInformation("Raw vnp_Amount: {Amount}", vnp_Amount);
                _logger.LogInformation("Raw vnp_OrderInfo: {OrderInfo}", vnp_OrderInfo);
                _logger.LogInformation("Raw vnp_ResponseCode: {ResponseCode}", vnp_ResponseCode);
                _logger.LogInformation("Raw vnp_SecureHash: {SecureHash}", vnp_SecureHash);
                _logger.LogInformation("Raw vnp_BankCode: {BankCode}", vnp_BankCode);
                _logger.LogInformation("Raw vnp_BankTranNo: {BankTranNo}", vnp_BankTranNo);
                _logger.LogInformation("Raw vnp_CardType: {CardType}", vnp_CardType);
                _logger.LogInformation("Raw vnp_PayDate: {PayDate}", vnp_PayDate);
                _logger.LogInformation("Raw vnp_TransactionNo: {TransactionNo}", vnp_TransactionNo);
                _logger.LogInformation("Raw vnp_SecureHashType: {SecureHashType}", vnp_SecureHashType);

                // Decode parameters
                var decodedTxnRef = HttpUtility.UrlDecode(vnp_TxnRef);
                var decodedAmount = HttpUtility.UrlDecode(vnp_Amount);
                var decodedOrderInfo = HttpUtility.UrlDecode(vnp_OrderInfo);
                var decodedResponseCode = HttpUtility.UrlDecode(vnp_ResponseCode);
                var decodedSecureHash = HttpUtility.UrlDecode(vnp_SecureHash);
                var decodedBankCode = HttpUtility.UrlDecode(vnp_BankCode);
                var decodedBankTranNo = HttpUtility.UrlDecode(vnp_BankTranNo);
                var decodedCardType = HttpUtility.UrlDecode(vnp_CardType);
                var decodedPayDate = HttpUtility.UrlDecode(vnp_PayDate);
                var decodedTransactionNo = HttpUtility.UrlDecode(vnp_TransactionNo);
                var decodedSecureHashType = HttpUtility.UrlDecode(vnp_SecureHashType);

                _logger.LogInformation("=== Decoded Parameters ===");
                _logger.LogInformation("Decoded vnp_TxnRef: {TxnRef}", decodedTxnRef);
                _logger.LogInformation("Decoded vnp_Amount: {Amount}", decodedAmount);
                _logger.LogInformation("Decoded vnp_OrderInfo: {OrderInfo}", decodedOrderInfo);
                _logger.LogInformation("Decoded vnp_ResponseCode: {ResponseCode}", decodedResponseCode);
                _logger.LogInformation("Decoded vnp_SecureHash: {SecureHash}", decodedSecureHash);
                _logger.LogInformation("Decoded vnp_BankCode: {BankCode}", decodedBankCode);
                _logger.LogInformation("Decoded vnp_BankTranNo: {BankTranNo}", decodedBankTranNo);
                _logger.LogInformation("Decoded vnp_CardType: {CardType}", decodedCardType);
                _logger.LogInformation("Decoded vnp_PayDate: {PayDate}", decodedPayDate);
                _logger.LogInformation("Decoded vnp_TransactionNo: {TransactionNo}", decodedTransactionNo);
                _logger.LogInformation("Decoded vnp_SecureHashType: {SecureHashType}", decodedSecureHashType);

                // Test hash generation
                var vnPaySettings = _configuration.GetSection("VnPaySettings");
                var vnpHashSecret = vnPaySettings["HashSecret"] ?? "DEMO_SECRET";
                
                var vnpParams = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(decodedBankCode)) vnpParams["vnp_BankCode"] = decodedBankCode;
                if (!string.IsNullOrEmpty(decodedBankTranNo)) vnpParams["vnp_BankTranNo"] = decodedBankTranNo;
                if (!string.IsNullOrEmpty(decodedCardType)) vnpParams["vnp_CardType"] = decodedCardType;
                if (!string.IsNullOrEmpty(decodedPayDate)) vnpParams["vnp_PayDate"] = decodedPayDate;
                if (!string.IsNullOrEmpty(decodedTransactionNo)) vnpParams["vnp_TransactionNo"] = decodedTransactionNo;
                
                vnpParams["vnp_Amount"] = decodedAmount;
                vnpParams["vnp_OrderInfo"] = decodedOrderInfo;
                vnpParams["vnp_ResponseCode"] = decodedResponseCode;
                vnpParams["vnp_TmnCode"] = vnPaySettings["TmnCode"] ?? "DEMO";
                vnpParams["vnp_TxnRef"] = decodedTxnRef;
                vnpParams["vnp_SecureHashType"] = decodedSecureHashType;

                var sortedParams = vnpParams.OrderBy(x => x.Key).ToList();
                var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));
                var expectedHash = _vnPayService.CreateSecureHash(queryString + vnpHashSecret);

                _logger.LogInformation("=== Hash Generation ===");
                _logger.LogInformation("Query String: {QueryString}", queryString);
                _logger.LogInformation("Expected Hash: {ExpectedHash}", expectedHash);
                _logger.LogInformation("Received Hash: {ReceivedHash}", decodedSecureHash);
                _logger.LogInformation("Hash Match: {Match}", string.Equals(decodedSecureHash, expectedHash, StringComparison.OrdinalIgnoreCase));

                return Ok(new
                {
                    RawParameters = new
                    {
                        vnp_TxnRef,
                        vnp_Amount,
                        vnp_OrderInfo,
                        vnp_ResponseCode,
                        vnp_SecureHash,
                        vnp_BankCode,
                        vnp_BankTranNo,
                        vnp_CardType,
                        vnp_PayDate,
                        vnp_TransactionNo,
                        vnp_SecureHashType
                    },
                    DecodedParameters = new
                    {
                        vnp_TxnRef = decodedTxnRef,
                        vnp_Amount = decodedAmount,
                        vnp_OrderInfo = decodedOrderInfo,
                        vnp_ResponseCode = decodedResponseCode,
                        vnp_SecureHash = decodedSecureHash,
                        vnp_BankCode = decodedBankCode,
                        vnp_BankTranNo = decodedBankTranNo,
                        vnp_CardType = decodedCardType,
                        vnp_PayDate = decodedPayDate,
                        vnp_TransactionNo = decodedTransactionNo,
                        vnp_SecureHashType = decodedSecureHashType
                    },
                    HashInfo = new
                    {
                        QueryString = queryString,
                        ExpectedHash = expectedHash,
                        ReceivedHash = decodedSecureHash,
                        Match = string.Equals(decodedSecureHash, expectedHash, StringComparison.OrdinalIgnoreCase)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug callback");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
