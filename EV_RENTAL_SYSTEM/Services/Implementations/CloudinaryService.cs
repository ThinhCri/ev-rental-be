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

            var uniqueId = Guid.NewGuid().ToString();
            var publicId = $"ImgCar/{uniqueId}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "ImgCar",
                PublicId = publicId,
                Overwrite = false,
                UniqueFilename = true
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

            var uniqueId = Guid.NewGuid().ToString();
            var publicId = $"ImgLicense/{uniqueId}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "ImgLicense",
                PublicId = publicId,
                Overwrite = false,
                UniqueFilename = true
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

            var uniqueId = Guid.NewGuid().ToString();
            var publicId = $"VehicleConditions/{uniqueId}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "VehicleConditions",
                PublicId = publicId,
                Overwrite = false,
                UniqueFilename = true
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
                var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                string? publicId = null;

                var carFolderIndex = Array.IndexOf(pathSegments, "ImgCar");
                var licenseFolderIndex = Array.IndexOf(pathSegments, "ImgLicense");
                var vehicleFolderIndex = Array.IndexOf(pathSegments, "VehicleConditions");

                if (carFolderIndex != -1 && carFolderIndex + 1 < pathSegments.Length)
                {
                    var fileName = pathSegments[carFolderIndex + 1];
                    if (fileName.StartsWith("v") && carFolderIndex + 2 < pathSegments.Length)
                    {
                        publicId = $"ImgCar/{pathSegments[carFolderIndex + 2]}";
                    }
                    else
                    {
                        publicId = $"ImgCar/{Path.GetFileNameWithoutExtension(fileName)}";
                    }
                }
                else if (licenseFolderIndex != -1 && licenseFolderIndex + 1 < pathSegments.Length)
                {
                    var fileName = pathSegments[licenseFolderIndex + 1];
                    if (fileName.StartsWith("v") && licenseFolderIndex + 2 < pathSegments.Length)
                    {
                        publicId = $"ImgLicense/{pathSegments[licenseFolderIndex + 2]}";
                    }
                    else
                    {
                        publicId = $"ImgLicense/{Path.GetFileNameWithoutExtension(fileName)}";
                    }
                }
                else if (vehicleFolderIndex != -1 && vehicleFolderIndex + 1 < pathSegments.Length)
                {
                    var fileName = pathSegments[vehicleFolderIndex + 1];
                    if (fileName.StartsWith("v") && vehicleFolderIndex + 2 < pathSegments.Length)
                    {
                        publicId = $"VehicleConditions/{pathSegments[vehicleFolderIndex + 2]}";
                    }
                    else
                    {
                        publicId = $"VehicleConditions/{Path.GetFileNameWithoutExtension(fileName)}";
                    }
                }
                else
                {
                    var lastSegment = pathSegments.LastOrDefault();
                    if (!string.IsNullOrEmpty(lastSegment))
                    {
                        publicId = Path.GetFileNameWithoutExtension(lastSegment);
                    }
                }

                if (string.IsNullOrEmpty(publicId))
                    return false;

                publicId = Path.GetFileNameWithoutExtension(publicId);

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
