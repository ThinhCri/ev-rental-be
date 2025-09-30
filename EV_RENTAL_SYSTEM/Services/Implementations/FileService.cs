using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    /// <summary>
    /// Service xử lý file upload
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly string _uploadPath;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
            _uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "licenses");
            
            // Tạo thư mục upload nếu chưa có
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        /// <summary>
        /// Upload file ảnh bằng lái xe
        /// </summary>
        /// <param name="file">File ảnh</param>
        /// <param name="userId">ID của user</param>
        /// <returns>URL của file đã upload</returns>
        public async Task<string> UploadCccdImageAsync(IFormFile file, int userId)
        {
            try
            {
                // Kiểm tra file có hợp lệ không
                if (!IsValidImageFile(file))
                {
                    throw new ArgumentException("File ảnh không hợp lệ. Chỉ chấp nhận file JPG, PNG, JPEG");
                }

                // Tạo tên file unique
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"license_{userId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_uploadPath, fileName);

                // Upload file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tạo URL để trả về
                var fileUrl = $"/uploads/licenses/{fileName}";
                
                _logger.LogInformation("File uploaded successfully: {FileName} for user {UserId}", fileName, userId);
                
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Xóa file ảnh
        /// </summary>
        /// <param name="filePath">Đường dẫn file</param>
        /// <returns>Kết quả xóa file</returns>
        public Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                // Chuyển đổi URL thành đường dẫn file thực
                var relativePath = filePath.TrimStart('/');
                var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Kiểm tra file có hợp lệ không
        /// </summary>
        /// <param name="file">File cần kiểm tra</param>
        /// <returns>File có hợp lệ hay không</returns>
        public bool IsValidImageFile(IFormFile file)
        {
            // Kiểm tra file có tồn tại không
            if (file == null || file.Length == 0)
                return false;

            // Kiểm tra kích thước file (tối đa 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return false;

            // Kiểm tra MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        /// <summary>
        /// Kiểm tra ảnh có phải là ảnh bằng lái xe hợp lệ không
        /// </summary>
        /// <param name="file">File ảnh cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        public async Task<(bool IsValid, string ErrorMessage)> ValidateLicenseImageAsync(IFormFile file)
        {
            try
            {
                // 1. Kiểm tra file cơ bản
                if (!IsValidImageFile(file))
                {
                    return (false, "File ảnh không hợp lệ. Chỉ chấp nhận file JPG, PNG, JPEG và kích thước tối đa 5MB");
                }

                // 2. Kiểm tra tên file có chứa từ khóa liên quan đến bằng lái xe không
                var fileName = file.FileName.ToLowerInvariant();
                var licenseKeywords = new[] { "license", "bằng", "lái", "xe", "driving", "permit", "cccd", "cmnd" };
                var hasLicenseKeyword = licenseKeywords.Any(keyword => fileName.Contains(keyword));
                
                if (!hasLicenseKeyword)
                {
                    _logger.LogWarning("File name does not contain license-related keywords: {FileName}", file.FileName);
                    // Không trả về lỗi, chỉ warning
                }

                // 3. Kiểm tra kích thước ảnh (tối thiểu 300x200 pixels)
                using (var stream = file.OpenReadStream())
                {
                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(stream))
                    {
                        if (image.Width < 300 || image.Height < 200)
                        {
                            return (false, "Ảnh quá nhỏ. Kích thước tối thiểu 300x200 pixels");
                        }

                        if (image.Width > 4000 || image.Height > 4000)
                        {
                            return (false, "Ảnh quá lớn. Kích thước tối đa 4000x4000 pixels");
                        }

                        // Kiểm tra tỷ lệ khung hình (không quá vuông hoặc quá dài)
                        var aspectRatio = (double)image.Width / image.Height;
                        if (aspectRatio < 0.5 || aspectRatio > 3.0)
                        {
                            return (false, "Tỷ lệ khung hình không phù hợp. Ảnh bằng lái xe thường có tỷ lệ 1:1.4 đến 1:2.5");
                        }
                    }
                }

                return (true, "Ảnh bằng lái xe hợp lệ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating license image: {FileName}", file.FileName);
                return (false, "Lỗi khi kiểm tra ảnh. Vui lòng thử lại");
            }
        }
    }
}
