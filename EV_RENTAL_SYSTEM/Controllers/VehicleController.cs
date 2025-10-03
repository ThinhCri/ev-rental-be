using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Controller xử lý các API liên quan đến xe
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả xe
        /// </summary>
        /// <returns>Danh sách xe</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var result = await _vehicleService.GetAllAsync();
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin xe theo ID
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <returns>Thông tin xe</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var result = await _vehicleService.GetByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo xe mới (chỉ dành cho Admin)
        /// </summary>
        /// <param name="createDto">Thông tin xe mới</param>
        /// <returns>Kết quả tạo xe</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _vehicleService.CreateAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetVehicleById), new { id = result.Data?.VehicleId }, result);
        }

        /// <summary>
        /// Cập nhật thông tin xe (chỉ dành cho Admin và Staff)
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <param name="updateDto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] UpdateVehicleDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _vehicleService.UpdateAsync(id, updateDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa xe (chỉ dành cho Admin)
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await _vehicleService.DeleteAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm xe với các bộ lọc
        /// </summary>
        /// <param name="searchDto">Các tham số tìm kiếm</param>
        /// <returns>Danh sách xe tìm được</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchVehicles([FromBody] VehicleSearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _vehicleService.SearchVehiclesAsync(searchDto);
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách xe có sẵn để thuê
        /// </summary>
        /// <returns>Danh sách xe có sẵn</returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            var result = await _vehicleService.GetAvailableVehiclesAsync();
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách xe theo thương hiệu
        /// </summary>
        /// <param name="brandId">ID của thương hiệu</param>
        /// <returns>Danh sách xe theo thương hiệu</returns>
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetVehiclesByBrand(int brandId)
        {
            var result = await _vehicleService.GetVehiclesByBrandAsync(brandId);
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách xe theo loại
        /// </summary>
        /// <param name="vehicleType">Loại xe</param>
        /// <returns>Danh sách xe theo loại</returns>
        [HttpGet("type/{vehicleType}")]
        public async Task<IActionResult> GetVehiclesByType(string vehicleType)
        {
            var result = await _vehicleService.GetVehiclesByTypeAsync(vehicleType);
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách xe theo khoảng giá
        /// </summary>
        /// <param name="minPrice">Giá tối thiểu</param>
        /// <param name="maxPrice">Giá tối đa</param>
        /// <returns>Danh sách xe theo khoảng giá</returns>
        [HttpGet("price-range")]
        public async Task<IActionResult> GetVehiclesByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                return BadRequest(new { message = "Khoảng giá không hợp lệ" });
            }

            var result = await _vehicleService.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách xe theo số ghế
        /// </summary>
        /// <param name="minSeats">Số ghế tối thiểu</param>
        /// <param name="maxSeats">Số ghế tối đa</param>
        /// <returns>Danh sách xe theo số ghế</returns>
        [HttpGet("seat-range")]
        public async Task<IActionResult> GetVehiclesBySeatRange([FromQuery] int minSeats, [FromQuery] int maxSeats)
        {
            if (minSeats < 1 || maxSeats < 1 || minSeats > maxSeats)
            {
                return BadRequest(new { message = "Khoảng số ghế không hợp lệ" });
            }

            var result = await _vehicleService.GetVehiclesBySeatRangeAsync(minSeats, maxSeats);
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra xe có sẵn để thuê không
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <returns>Trạng thái sẵn sàng</returns>
        [HttpGet("{id}/availability")]
        public async Task<IActionResult> CheckVehicleAvailability(int id)
        {
            var isAvailable = await _vehicleService.IsVehicleAvailableAsync(id);
            
            return Ok(new
            {
                Success = true,
                Message = "Kiểm tra trạng thái xe thành công",
                Data = new { VehicleId = id, IsAvailable = isAvailable }
            });
        }

        /// <summary>
        /// Lấy số lượng xe có sẵn
        /// </summary>
        /// <returns>Số lượng xe có sẵn</returns>
        [HttpGet("available-count")]
        public async Task<IActionResult> GetAvailableVehicleCount()
        {
            var count = await _vehicleService.GetAvailableVehicleCountAsync();
            
            return Ok(new
            {
                Success = true,
                Message = "Lấy số lượng xe có sẵn thành công",
                Data = new { AvailableCount = count }
            });
        }

        /// <summary>
        /// Cập nhật trạng thái xe (chỉ dành cho Admin)
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        [HttpPut("{id}/toggle-availability")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ToggleVehicleAvailability(int id)
        {
            var result = await _vehicleService.ToggleAvailabilityAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
