namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface ICloudService
    {
        public Task<string> UploadImageAsync(IFormFile file);
        public Task<string> UploadLicenseImageAsync(IFormFile file);
        public Task<string> UploadVehicleImageAsync(IFormFile file);
        public Task<bool> DeleteImageAsync(string imageUrl);
    }
}