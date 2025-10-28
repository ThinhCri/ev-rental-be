using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// License type management controller
    /// </summary>
    public class LicenseTypeController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LicenseTypeController> _logger;

        public LicenseTypeController(IUnitOfWork unitOfWork, ILogger<LicenseTypeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all license types endpoint
        /// </summary>
        /// <returns>List of license types</returns>
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

                return SuccessResponse(result, "License types retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving license types");
                return ErrorResponse("An error occurred while retrieving license types", 500);
            }
        }

        /// <summary>
        /// Get license type by ID endpoint
        /// </summary>
        /// <param name="id">License type ID</param>
        /// <returns>License type information</returns>
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

                return SuccessResponse(result, "License type retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving license type with ID: {Id}", id);
                return ErrorResponse("An error occurred while retrieving license type", 500);
            }
        }
    }
}

