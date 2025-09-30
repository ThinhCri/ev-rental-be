using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace EV_RENTAL_SYSTEM.Attributes
{
    /// <summary>
    /// Custom attribute để hiển thị thông tin loại bằng lái xe trong Swagger
    /// </summary>
    public class LicenseTypeIdAttribute : ValidationAttribute
    {
        public LicenseTypeIdAttribute()
        {
            ErrorMessage = "Loại bằng lái xe không hợp lệ. Chọn: 1=A1, 2=A2, 3=A, 4=B1, 5=B2";
        }

        public override bool IsValid(object? value)
        {
            if (value is int licenseTypeId)
            {
                return licenseTypeId >= 1 && licenseTypeId <= 5;
            }
            return false;
        }
    }
}
