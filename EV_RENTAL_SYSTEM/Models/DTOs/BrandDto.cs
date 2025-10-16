namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }

    public class BrandListResponseDto : ServiceResponse<List<BrandDto>>
    {
    }
}
