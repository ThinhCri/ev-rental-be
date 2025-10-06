using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_RENTAL_SYSTEM.Controllers
{
    /// <summary>
    /// Admin revenue management controller
    /// </summary>
    [Route("api/admin/revenue")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminRevenueController : BaseController
    {
        private readonly IRevenueService _revenueService;

        public AdminRevenueController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        /// <summary>
        /// Get revenue overview endpoint
        /// </summary>
        [HttpGet("overview")]
        public async Task<IActionResult> GetRevenueOverview()
        {
            var result = await _revenueService.GetRevenueOverviewAsync();
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get daily revenue endpoint
        /// </summary>
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyRevenue([FromQuery] RevenueFilterDto filter)
        {
            var result = await _revenueService.GetDailyRevenueAsync(filter);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get monthly revenue endpoint
        /// </summary>
        [HttpGet("monthly/{year}")]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            var result = await _revenueService.GetMonthlyRevenueAsync(year);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get revenue by station endpoint
        /// </summary>
        [HttpGet("by-station")]
        public async Task<IActionResult> GetRevenueByStation([FromQuery] RevenueFilterDto filter)
        {
            var result = await _revenueService.GetRevenueByStationAsync(filter);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get revenue by vehicle type endpoint
        /// </summary>
        [HttpGet("by-vehicle-type")]
        public async Task<IActionResult> GetRevenueByVehicleType([FromQuery] RevenueFilterDto filter)
        {
            var result = await _revenueService.GetRevenueByVehicleTypeAsync(filter);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get top rented vehicles endpoint
        /// </summary>
        [HttpGet("top-vehicles")]
        public async Task<IActionResult> GetTopVehicles([FromQuery] RevenueFilterDto filter, [FromQuery] int top = 5)
        {
            var result = await _revenueService.GetTopRentedVehiclesAsync(filter, top);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get detailed revenue report endpoint
        /// </summary>
        [HttpGet("detailed-report")]
        public async Task<IActionResult> GetDetailedReport([FromQuery] RevenueFilterDto filter)
        {
            var result = await _revenueService.GetDetailedRevenueReportAsync(filter);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Export revenue report to Excel endpoint
        /// </summary>
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] RevenueFilterDto filter)
        {
            var result = await _revenueService.ExportRevenueReportToExcelAsync(filter);
            if (!result.Success)
            {
                return ErrorResponse(result.Message, 500);
            }
            return Ok(result);
        }

        /// <summary>
        /// Debug user information endpoint
        /// </summary>
        [HttpGet("debug-user")]
        public IActionResult DebugUser()
        {
            var userInfo = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Name = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                Roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList()
            };
            
            return SuccessResponse(userInfo, "User information retrieved");
        }
    }
}