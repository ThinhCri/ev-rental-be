using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Controller xử lý các API liên quan đến loại bằng lái xe
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LicenseTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LicenseTypeController> _logger;

        public LicenseTypeController(IUnitOfWork unitOfWork, ILogger<LicenseTypeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// API lấy danh sách tất cả loại bằng lái xe
        /// </summary>
        /// <returns>Danh sách loại bằng lái xe</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllLicenseTypes()
        {
            try
            {
                var licenseTypes = await _unitOfWork.LicenseTypes.GetAllAsync();
                
                var result = licenseTypes.Select(lt => new
                {
                    LicenseTypeId = lt.LicenseTypeId,
                    TypeName = lt.TypeName,
                    Description = lt.Description
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Message = "License types retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving license types");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving license types"
                });
            }
        }

        /// <summary>
        /// API lấy thông tin loại bằng lái xe theo ID
        /// </summary>
        /// <param name="id">ID của loại bằng lái xe</param>
        /// <returns>Thông tin loại bằng lái xe</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicenseTypeById(int id)
        {
            try
            {
                var licenseType = await _unitOfWork.LicenseTypes.GetByIdAsync(id);
                
                if (licenseType == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "License type not found"
                    });
                }

                var result = new
                {
                    LicenseTypeId = licenseType.LicenseTypeId,
                    TypeName = licenseType.TypeName,
                    Description = licenseType.Description
                };

                return Ok(new
                {
                    Success = true,
                    Message = "License type retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving license type with ID: {Id}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving license type"
                });
            }
        }
    }
}
