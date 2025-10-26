using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class CloudinaryService : ICloudService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File erorr .");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "ImgCar"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();

            throw new Exception("Upload failed: " + result.Error?.Message);
        }

        public async Task<string> UploadLicenseImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File error.");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "ImgLicense"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();

            throw new Exception("Upload failed: " + result.Error?.Message);
        }

        public async Task<string> UploadVehicleImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File error.");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "VehicleConditions"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();

            throw new Exception("Upload failed: " + result.Error?.Message);
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            try
            {
                var uri = new Uri(imageUrl);
                var pathSegments = uri.AbsolutePath.Split('/');

                // Check for ImgCar folder
                var carFolderIndex = Array.IndexOf(pathSegments, "ImgCar");
                if (carFolderIndex != -1 && carFolderIndex + 1 < pathSegments.Length)
                {
                    var publicId = $"ImgCar/{Path.GetFileNameWithoutExtension(pathSegments[carFolderIndex + 1])}";
                    var deleteParams = new DeletionParams(publicId);
                    var result = await _cloudinary.DestroyAsync(deleteParams);
                    return result.Result == "ok";
                }

                // Check for ImgLicense folder
                var licenseFolderIndex = Array.IndexOf(pathSegments, "ImgLicense");
                if (licenseFolderIndex != -1 && licenseFolderIndex + 1 < pathSegments.Length)
                {
                    var publicId = $"ImgLicense/{Path.GetFileNameWithoutExtension(pathSegments[licenseFolderIndex + 1])}";
                    var deleteParams = new DeletionParams(publicId);
                    var result = await _cloudinary.DestroyAsync(deleteParams);
                    return result.Result == "ok";
                }

                // Check for VehicleConditions folder
                var vehicleFolderIndex = Array.IndexOf(pathSegments, "VehicleConditions");
                if (vehicleFolderIndex != -1 && vehicleFolderIndex + 1 < pathSegments.Length)
                {
                    var publicId = $"VehicleConditions/{Path.GetFileNameWithoutExtension(pathSegments[vehicleFolderIndex + 1])}";
                    var deleteParams = new DeletionParams(publicId);
                    var result = await _cloudinary.DestroyAsync(deleteParams);
                    return result.Result == "ok";
                }

                // Fallback: Try to extract from the end of URL if folder structure is different
                var fileName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                if (string.IsNullOrEmpty(fileName)) return false;

                var deleteParamsFallback = new DeletionParams(fileName);
                var resultFallback = await _cloudinary.DestroyAsync(deleteParamsFallback);
                return resultFallback.Result == "ok";
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return false;
            }
        }
    }
}
