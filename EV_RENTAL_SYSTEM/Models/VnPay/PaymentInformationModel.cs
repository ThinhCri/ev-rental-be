using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.VnPay
{
    public class PaymentInformationModel
    {
        [Required(ErrorMessage = "Order type is required")]
        public string OrderType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Order description is required")]
        public string OrderDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
