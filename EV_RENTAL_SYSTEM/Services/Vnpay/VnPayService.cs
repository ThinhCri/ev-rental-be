using EV_RENTAL_SYSTEM.Libraries;
using EV_RENTAL_SYSTEM.Models.VnPay;

namespace EV_RENTAL_SYSTEM.Services.Vnpay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"] ?? "SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var txnRef = model.OrderId?.ToString() ?? DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var configuredCallbackUrl = _configuration["Vnpay:PaymentBackReturnUrl"];
            var urlCallBack = !string.IsNullOrEmpty(configuredCallbackUrl) 
                ? configuredCallbackUrl 
                : $"{context.Request.Scheme}://{context.Request.Host}/api/Payment/payment-callback";

            var usdToVndRate = 24000;
            var amountInVnd = (int)(model.Amount * usdToVndRate);

            if (amountInVnd < 5000)
            {
                amountInVnd = 5000;
            }

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"] );
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"] );
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"] );
            pay.AddRequestData("vnp_Amount", (amountInVnd * 100).ToString()); 
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"] ?? "en");
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} ${model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", txnRef);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", 
                                   _configuration["Vnpay:HashSecret"] ?? "");

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"] ?? "");

            return response;
        }

    }
}
