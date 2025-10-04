namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string VehicleImage { get; set; } = string.Empty;
        public int PricePerDay { get; set; }
        public string Battery { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? ModelYear { get; set; }
        public int BrandId { get; set; }
        public string? Description { get; set; }
    }
}
