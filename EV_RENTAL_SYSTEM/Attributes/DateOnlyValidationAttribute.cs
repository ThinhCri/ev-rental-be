using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EV_RENTAL_SYSTEM.Attributes
{
    /// <summary>
    /// Custom validation attribute để kiểm tra định dạng ngày tháng chỉ có yyyy-mm-dd
    /// </summary>
    public class DateOnlyValidationAttribute : ValidationAttribute
    {
        private const string DateFormat = "yyyy-MM-dd";
        
        public DateOnlyValidationAttribute()
        {
            ErrorMessage = $"Ngày sinh phải có định dạng {DateFormat} (ví dụ: 1990-01-15)";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Cho phép null (optional field)
            }

            if (value is DateTime dateTime)
            {
                // Kiểm tra xem có phải chỉ có ngày không (không có giờ phút giây)
                if (dateTime.TimeOfDay != TimeSpan.Zero)
                {
                    return new ValidationResult($"Ngày sinh không được chứa giờ phút giây. Chỉ chấp nhận định dạng {DateFormat}");
                }

                // Kiểm tra xem ngày có hợp lệ không (không được trong tương lai)
                if (dateTime > DateTime.Today)
                {
                    return new ValidationResult("Ngày sinh không được là ngày trong tương lai");
                }

                // Kiểm tra tuổi hợp lệ (ít nhất 16 tuổi, tối đa 120 tuổi)
                var age = DateTime.Today.Year - dateTime.Year;
                if (DateTime.Today.DayOfYear < dateTime.DayOfYear)
                    age--;

                if (age < 16)
                {
                    return new ValidationResult("Bạn phải ít nhất 16 tuổi để đăng ký");
                }

                if (age > 120)
                {
                    return new ValidationResult("Ngày sinh không hợp lệ");
                }

                return ValidationResult.Success;
            }

            if (value is string dateString)
            {
                if (string.IsNullOrWhiteSpace(dateString))
                {
                    return ValidationResult.Success; // Cho phép empty string
                }

                // Kiểm tra định dạng yyyy-MM-dd
                if (!DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return new ValidationResult($"Ngày sinh phải có định dạng {DateFormat} (ví dụ: 1990-01-15)");
                }

                // Kiểm tra xem có phải chỉ có ngày không
                if (parsedDate.TimeOfDay != TimeSpan.Zero)
                {
                    return new ValidationResult($"Ngày sinh không được chứa giờ phút giây. Chỉ chấp nhận định dạng {DateFormat}");
                }

                // Kiểm tra ngày có hợp lệ không
                if (parsedDate > DateTime.Today)
                {
                    return new ValidationResult("Ngày sinh không được là ngày trong tương lai");
                }

                // Kiểm tra tuổi hợp lệ
                var age = DateTime.Today.Year - parsedDate.Year;
                if (DateTime.Today.DayOfYear < parsedDate.DayOfYear)
                    age--;

                if (age < 16)
                {
                    return new ValidationResult("Bạn phải ít nhất 16 tuổi để đăng ký");
                }

                if (age > 120)
                {
                    return new ValidationResult("Ngày sinh không hợp lệ");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Định dạng ngày sinh không hợp lệ");
        }
    }
}
