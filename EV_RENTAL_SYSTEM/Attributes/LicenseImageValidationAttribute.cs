using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace EV_RENTAL_SYSTEM.Attributes
{
    /// <summary>
    /// Custom validation attribute để kiểm tra file upload có phải ảnh bằng lái xe hợp lệ không
    /// </summary>
    public class LicenseImageValidationAttribute : ValidationAttribute
    {
        private readonly long _maxFileSizeInBytes;
        private readonly string[] _allowedExtensions;
        private readonly int _minWidth;
        private readonly int _minHeight;

        public LicenseImageValidationAttribute(
            long maxFileSizeInMB = 5,
            string allowedExtensions = ".jpg,.jpeg,.png,.gif",
            int minWidth = 300,
            int minHeight = 200)
        {
            _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024; // Convert MB to bytes
            _allowedExtensions = allowedExtensions.Split(',').Select(ext => ext.Trim().ToLower()).ToArray();
            _minWidth = minWidth;
            _minHeight = minHeight;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Ảnh bằng lái xe là bắt buộc");
            }

            if (value is not IFormFile file)
            {
                return new ValidationResult("Dữ liệu không hợp lệ");
            }

            // Kiểm tra file có tồn tại không
            if (file.Length == 0)
            {
                return new ValidationResult("File ảnh không được để trống");
            }

            // Kiểm tra kích thước file
            if (file.Length > _maxFileSizeInBytes)
            {
                return new ValidationResult($"Kích thước file không được vượt quá {_maxFileSizeInBytes / (1024 * 1024)}MB");
            }

            // Kiểm tra extension
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult($"Chỉ chấp nhận các định dạng: {string.Join(", ", _allowedExtensions)}");
            }

            // Kiểm tra MIME type
            if (!IsValidImageMimeType(file.ContentType))
            {
                return new ValidationResult("File không phải là ảnh hợp lệ");
            }

            // Kiểm tra kích thước ảnh (chỉ khi có thể đọc được)
            try
            {
                using var stream = file.OpenReadStream();
                using var image = Image.FromStream(stream);
                
                if (image.Width < _minWidth || image.Height < _minHeight)
                {
                    return new ValidationResult($"Kích thước ảnh tối thiểu phải là {_minWidth}x{_minHeight} pixels");
                }

                // Kiểm tra tỷ lệ khung hình (bằng lái xe thường có tỷ lệ 3:2 hoặc 4:3)
                var aspectRatio = (double)image.Width / image.Height;
                if (aspectRatio < 1.2 || aspectRatio > 2.0)
                {
                    return new ValidationResult("Tỷ lệ khung hình ảnh không phù hợp với bằng lái xe (nên là 3:2 hoặc 4:3)");
                }

                // Kiểm tra thêm: ảnh có quá tối không (bằng lái xe thường sáng)
                var brightness = CalculateAverageBrightness(image);
                if (brightness < 100) // Quá tối
                {
                    return new ValidationResult("Ảnh quá tối, vui lòng chụp lại với ánh sáng tốt hơn");
                }
            }
            catch (Exception)
            {
                return new ValidationResult("Không thể đọc file ảnh, vui lòng kiểm tra lại");
            }

            return ValidationResult.Success;
        }

        private bool IsValidImageMimeType(string contentType)
        {
            var validMimeTypes = new[]
            {
                "image/jpeg",
                "image/jpg", 
                "image/png",
                "image/gif"
            };

            return validMimeTypes.Contains(contentType.ToLower());
        }

        private float CalculateAverageBrightness(Image image)
        {
            try
            {
                using var bitmap = new Bitmap(image);
                var totalBrightness = 0f;
                var pixelCount = 0;

                // Lấy mẫu một số pixel để tính độ sáng trung bình
                for (int x = 0; x < bitmap.Width; x += 10)
                {
                    for (int y = 0; y < bitmap.Height; y += 10)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        var brightness = (pixel.R + pixel.G + pixel.B) / 3.0f;
                        totalBrightness += brightness;
                        pixelCount++;
                    }
                }

                return pixelCount > 0 ? totalBrightness / pixelCount : 0f;
            }
            catch
            {
                return 0f;
            }
        }
    }
}
