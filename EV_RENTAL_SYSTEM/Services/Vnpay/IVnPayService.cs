using EV_RENTAL_SYSTEM.Models.VnPay;

namespace EV_RENTAL_SYSTEM.Services.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
