using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RentalService> _logger;
        private readonly ICloudService _cloudService;

        public RentalService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RentalService> logger, ICloudService cloudService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cloudService = cloudService;
        }

        /// <summary>
        /// Helper method to populate RentalDto from Order entity
        /// </summary>
        private async Task<RentalDto> PopulateRentalDtoAsync(Order order)
        {
            var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(order.OrderId);
            
            // Lấy thông tin xe từ Order_LicensePlate
            var vehicles = new List<RentalVehicleDto>();
            if (order.OrderLicensePlates != null && order.OrderLicensePlates.Any())
            {
                foreach (var orderLicensePlate in order.OrderLicensePlates)
                {
                    if (orderLicensePlate.LicensePlate?.Vehicle != null)
                    {
                        var vehicle = orderLicensePlate.LicensePlate.Vehicle;
                        vehicles.Add(new RentalVehicleDto
                        {
                            VehicleId = vehicle.VehicleId,
                            Model = vehicle.Model ?? "",
                            BrandName = vehicle.Brand?.BrandName ?? "",
                            VehicleType = "Electric",
                            PricePerDay = vehicle.PricePerDay,
                            SeatNumber = vehicle.SeatNumber,
                            VehicleImage = vehicle.VehicleImage,
                            StationName = orderLicensePlate.LicensePlate.Station?.StationName ?? "",
                            LicensePlates = new List<RentalLicensePlateDto>
                            {
                                new RentalLicensePlateDto
                                {
                                    LicensePlateId = orderLicensePlate.LicensePlate.LicensePlateId,
                                    PlateNumber = orderLicensePlate.LicensePlate.PlateNumber ?? "",
                                    Status = orderLicensePlate.LicensePlate.Status
                                }
                            }
                        });
                    }
                }
            }

            // Xác định ảnh GPLX hiển thị:
            // - Nếu User.Notes có giá trị (đặt hộ): dùng User.Notes
            // - Ngược lại (tự đặt): dùng license đầu tiên của user
            string? licenseImageUrl = null;
            if (!string.IsNullOrWhiteSpace(order.User?.Notes))
            {
                licenseImageUrl = order.User.Notes;
            }
            else
            {
                var userLicenses = await _unitOfWork.Licenses.GetByUserIdAsync(order.UserId);
                licenseImageUrl = userLicenses?.FirstOrDefault()?.LicenseImageUrl;
            }

            return new RentalDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                StartTime = order.StartTime,
                EndTime = order.EndTime,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                UserId = order.UserId,
                UserName = order.User?.FullName ?? "Unknown User",
                UserEmail = order.User?.Email ?? "unknown@example.com",
                LicenseImageUrl = licenseImageUrl,
                ContractId = contract?.ContractId,
                TotalDays = order.StartTime.HasValue && order.EndTime.HasValue 
                    ? (int)Math.Ceiling((order.EndTime.Value - order.StartTime.Value).TotalDays)
                    : 0,
                DailyRate = vehicles.FirstOrDefault()?.PricePerDay ?? 0,
                DepositAmount = contract?.Deposit ?? 0,
                RentalFee = contract?.RentalFee ?? order.TotalAmount,
                ExtraFee = contract?.ExtraFee ?? 0,
                Vehicles = vehicles,
                Contracts = new List<RentalContractDto>()
            };
        }

        public async Task<RentalListResponseDto> GetAllRentalsAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var rentalDtos = new List<RentalDto>();

                foreach (var order in orders)
                {
                    var rentalDto = await PopulateRentalDtoAsync(order);
                    rentalDtos.Add(rentalDto);
                }

                return new RentalListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách đơn thuê thành công",
                    Data = rentalDtos,
                    TotalCount = rentalDtos.Count,
                    PageNumber = 1,
                    PageSize = rentalDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rentals: {Error}", ex.Message);
                return new RentalListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách đơn thuê: {ex.Message}",
                    Data = new List<RentalDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<RentalResponseDto> GetRentalByIdAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                var rentalDto = await PopulateRentalDtoAsync(order);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Lấy thông tin đơn thuê thành công",
                    Data = rentalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rental by ID {OrderId}: {Error}", orderId, ex.Message);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy thông tin đơn thuê: {ex.Message}"
                };
            }
        }

        public async Task<RentalListResponseDto> GetUserRentalsAsync(int userId)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);
                var rentalDtos = new List<RentalDto>();

                foreach (var order in orders)
                {
                    var rentalDto = await PopulateRentalDtoAsync(order);
                    rentalDtos.Add(rentalDto);
                }

                return new RentalListResponseDto
                {
                    Success = true,
                    Message = "Lấy lịch sử thuê xe thành công",
                    Data = rentalDtos,
                    TotalCount = rentalDtos.Count,
                    PageNumber = 1,
                    PageSize = rentalDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user rentals for user {UserId}: {Error}", userId, ex.Message);
                return new RentalListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy lịch sử thuê xe: {ex.Message}",
                    Data = new List<RentalDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto createDto, int userId)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Creating rental for user {UserId} with vehicle {VehicleId}", 
                    userId, createDto.VehicleId);

                // Validate date range
                if (createDto.StartTime >= createDto.EndTime)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu"
                    };
                }

                if (createDto.StartTime < DateTime.Now.AddMinutes(-5))
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Thời gian bắt đầu không thể trong quá khứ"
                    };
                }
                if (createDto.IsBookingForOthers && createDto.RenterLicenseImage != null)
                {
                    var licenseImageUrl = await _cloudService.UploadLicenseImageAsync(createDto.RenterLicenseImage);
                    
                    var currentUser = await _unitOfWork.Users.GetByIdAsync(userId);
                    if (currentUser != null)
                    {
                        currentUser.Notes = licenseImageUrl;
                        await _unitOfWork.Users.UpdateAsync(currentUser);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
  
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(createDto.VehicleId);

                if (!await IsVehicleAvailableAsync(createDto.VehicleId, createDto.StartTime, createDto.EndTime))
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = $"Xe ID {createDto.VehicleId} không có sẵn trong khoảng thời gian đã chọn"
                    };
                }

                // Kiểm tra có biển số Available không
                var allLicensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
                var availableLicensePlates = allLicensePlates
                    .Where(lp => lp.VehicleId == createDto.VehicleId && lp.Status == "Available")
                    .ToList();
                
                if (!availableLicensePlates.Any())
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = $"Không có biển số khả dụng cho xe ID {createDto.VehicleId}"
                    };
                }

                // Tính tổng tiền
                var totalAmount = 0m;
                if (vehicle.PricePerDay != null)
                {
                    var days = (int)Math.Ceiling((createDto.EndTime - createDto.StartTime).TotalDays);
                    totalAmount = vehicle.PricePerDay.Value * days;
                }

                // Tạo Order với tổng tiền đã tính
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    StartTime = createDto.StartTime,
                    EndTime = createDto.EndTime,
                    Status = "Pending", // Trạng thái chờ xác nhận hợp đồng
                    TotalAmount = totalAmount
                };

    
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var contractCode = $"EV{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";

                var contract = new Contract
                {
                    OrderId = order.OrderId, // Bây giờ OrderId đã có
                    ContractCode = contractCode,
                    CreatedDate = DateTime.Now,
                    Status = "Pending", // Trạng thái chờ xác nhận
                    Deposit = createDto.DepositAmount ?? (totalAmount * 0.2m),
                    RentalFee = totalAmount,
                    ExtraFee = 0
                };

                await _unitOfWork.Contracts.AddAsync(contract);
                await _unitOfWork.SaveChangesAsync();

                // Tạo Order_LicensePlate cho xe (sau khi Order đã có OrderId)
                var availableLicensePlate = availableLicensePlates.FirstOrDefault();
                if (availableLicensePlate != null)
                {
                    var orderLicensePlate = new Order_LicensePlate
                    {
                        OrderId = order.OrderId,
                        LicensePlateId = availableLicensePlate.LicensePlateId
                    };
                    await _unitOfWork.OrderLicensePlates.AddAsync(orderLicensePlate);
                }
                else
                {
                    // Nếu không có biển số Available, rollback transaction
                    await _unitOfWork.RollbackTransactionAsync();
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = $"Không có biển số khả dụng cho xe ID {createDto.VehicleId}"
                    };
                }
                
                await _unitOfWork.SaveChangesAsync();

                // Commit transaction nếu tất cả thành công
                await _unitOfWork.CommitTransactionAsync();

                // Reload order với đầy đủ navigation properties
                var createdOrder = await _unitOfWork.Orders.GetByIdAsync(order.OrderId);
                if (createdOrder == null)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Lỗi khi tải lại thông tin đơn thuê"
                    };
                }

                var rentalDto = await PopulateRentalDtoAsync(createdOrder);

                _logger.LogInformation("Created rental {OrderId} for user {UserId} with total amount {TotalAmount}", 
                    order.OrderId, userId, totalAmount);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Tạo đơn thuê thành công",
                    Data = rentalDto,
                    OrderId = order.OrderId,
                    ContractId = contract.ContractId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental for user {UserId}: {Error}", userId, ex.Message);
                
                // Rollback transaction nếu có lỗi
                await _unitOfWork.RollbackTransactionAsync();
                
                return new RentalResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi tạo đơn thuê: {ex.Message}"
                };
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task<RentalResponseDto> UpdateRentalAsync(int orderId, UpdateRentalDto updateDto)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                // Cập nhật thông tin
                if (updateDto.StartTime.HasValue) order.StartTime = updateDto.StartTime.Value;
                if (updateDto.EndTime.HasValue) order.EndTime = updateDto.EndTime.Value;
                if (!string.IsNullOrEmpty(updateDto.Status)) order.Status = updateDto.Status;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Cập nhật đơn thuê thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rental {OrderId}: {Error}", orderId, ex.Message);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi cập nhật đơn thuê: {ex.Message}"
                };
            }
        }

        public async Task<RentalResponseDto> CancelRentalAsync(int orderId, int userId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                if (order.UserId != userId)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "You don't have permission to cancel this rental"
                    };
                }

                // Lấy danh sách biển số xe trong đơn thuê
                var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetByOrderIdAsync(orderId);
                var licensePlateIds = orderLicensePlates.Select(olp => olp.LicensePlateId).ToList();

                // Cập nhật trạng thái biển số xe về Available
                foreach (var licensePlateId in licensePlateIds)
                {
                    var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(licensePlateId);
                    if (licensePlate != null)
                    {
                        licensePlate.Status = "Available";
                        _unitOfWork.LicensePlates.Update(licensePlate);
                    }
                }

                // Xóa các bản ghi Order_LicensePlate
                foreach (var orderLicensePlate in orderLicensePlates)
                {
                    _unitOfWork.OrderLicensePlates.Remove(orderLicensePlate);
                }

                // Xóa các Contract liên quan
                var contracts = await _unitOfWork.Contracts.GetContractsByOrderIdAsync(orderId);
                foreach (var contract in contracts)
                {
                    // Xóa Contract (Payment sẽ được xóa cascade nếu có)
                    _unitOfWork.Contracts.Remove(contract);
                }

                // Xóa Order
                _unitOfWork.Orders.Remove(order);

                await _unitOfWork.SaveChangesAsync();

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Hủy đơn thuê thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling rental {OrderId}: {Error}", orderId, ex.Message);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi hủy đơn thuê: {ex.Message}"
                };
            }
        }

        public async Task<RentalResponseDto> CompleteRentalAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                order.Status = "Completed";
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Hoàn thành đơn thuê thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing rental {OrderId}: {Error}", orderId, ex.Message);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi hoàn thành đơn thuê: {ex.Message}"
                };
            }
        }

        public async Task<AvailableVehicleListResponseDto> GetAvailableVehiclesAsync(AvailableVehiclesSearchDto searchDto)
        {
            try
            {
                var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
                var availableVehicles = new List<AvailableVehicleDto>();

                foreach (var vehicle in vehicles)
                {
                    var availableVehicle = new AvailableVehicleDto
                    {
                        VehicleId = vehicle.VehicleId,
                        Model = vehicle.Model ?? "",
                        BrandName = vehicle.Brand?.BrandName ?? "",
                        VehicleType = "Electric", // Tạm thời
                        PricePerDay = vehicle.PricePerDay,
                        SeatNumber = vehicle.SeatNumber,
                        VehicleImage = vehicle.VehicleImage,
                        StationName = "Station", // Tạm thời
                        StationId = 1,
                        Battery = vehicle.Battery,
                        RangeKm = vehicle.RangeKm,
                        Status = "Available",
                        AvailableLicensePlates = new List<AvailableLicensePlateDto>(),
                        EstimatedTotalCost = vehicle.PricePerDay * (int)Math.Ceiling((searchDto.EndTime - searchDto.StartTime).TotalDays)
                    };

                    availableVehicles.Add(availableVehicle);
                }

                return new AvailableVehicleListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách xe có sẵn thành công",
                    Data = availableVehicles,
                    TotalCount = availableVehicles.Count,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = (int)Math.Ceiling((double)availableVehicles.Count / searchDto.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available vehicles: {Error}", ex.Message);
                return new AvailableVehicleListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách xe có sẵn: {ex.Message}",
                    Data = new List<AvailableVehicleDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<RentalListResponseDto> SearchRentalsAsync(RentalSearchDto searchDto)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var filteredOrders = orders.AsQueryable();

                if (searchDto.UserId.HasValue)
                    filteredOrders = filteredOrders.Where(o => o.UserId == searchDto.UserId.Value);

                if (!string.IsNullOrEmpty(searchDto.Status))
                    filteredOrders = filteredOrders.Where(o => o.Status == searchDto.Status);

                if (searchDto.StartDate.HasValue)
                    filteredOrders = filteredOrders.Where(o => o.OrderDate >= searchDto.StartDate.Value);

                if (searchDto.EndDate.HasValue)
                    filteredOrders = filteredOrders.Where(o => o.OrderDate <= searchDto.EndDate.Value);

                var rentalDtos = new List<RentalDto>();
                foreach (var order in filteredOrders)
                {
                    var rentalDto = await PopulateRentalDtoAsync(order);
                    rentalDtos.Add(rentalDto);
                }

                return new RentalListResponseDto
                {
                    Success = true,
                    Message = "Tìm kiếm đơn thuê thành công",
                    Data = rentalDtos,
                    TotalCount = rentalDtos.Count,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = (int)Math.Ceiling((double)rentalDtos.Count / searchDto.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching rentals: {Error}", ex.Message);
                return new RentalListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi tìm kiếm đơn thuê: {ex.Message}",
                    Data = new List<RentalDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<RentalResponseDto> GetRentalWithDetailsAsync(int orderId)
        {
            return await GetRentalByIdAsync(orderId);
        }

        public async Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(vehicleId);
                if (vehicle?.PricePerDay == null)
                    return 0;

                var days = (int)Math.Ceiling((endTime - startTime).TotalDays);
                return vehicle.PricePerDay.Value * days;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating rental cost: {Error}", ex.Message);
                return 0;
            }
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime)
        {
            try
            {
                // Kiểm tra xe có tồn tại không
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(vehicleId);
                if (vehicle == null)
                    return false;

                // Kiểm tra xe có biển số Available không
                var licensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
                var availableLicensePlates = licensePlates
                    .Where(lp => lp.VehicleId == vehicleId && lp.Status == "Available")
                    .ToList();

                if (!availableLicensePlates.Any())
                    return false;

                // Kiểm tra xe có bị thuê trong khoảng thời gian này không
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var conflictingOrders = orders.Where(o => 
                    o.Status != "Cancelled" && 
                    o.Status != "Completed" &&
                    o.StartTime.HasValue && 
                    o.EndTime.HasValue &&
                    !(endTime <= o.StartTime.Value || startTime >= o.EndTime.Value))
                    .ToList();

                // Kiểm tra xem có xe nào trong danh sách bị conflict không
                foreach (var order in conflictingOrders)
                {
                    var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetByOrderIdAsync(order.OrderId);
                    var orderVehicleIds = orderLicensePlates
                        .Select(olp => availableLicensePlates.FirstOrDefault(lp => lp.LicensePlateId == olp.LicensePlateId)?.VehicleId)
                        .Where(vid => vid.HasValue)
                        .Select(vid => vid.Value)
                        .ToList();

                    if (orderVehicleIds.Contains(vehicleId))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking vehicle availability: {Error}", ex.Message);
                return false;
            }
        }

        public async Task<ServiceResponse<ContractSummaryDto>> GetContractSummaryAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ServiceResponse<ContractSummaryDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(orderId);
                if (contract == null)
                {
                    return new ServiceResponse<ContractSummaryDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy hợp đồng"
                    };
                }

                var contractSummary = new ContractSummaryDto
                {
                    ContractCode = contract.ContractCode ?? "",
                    RentalFee = contract.RentalFee ?? 0,
                    Deposit = contract.Deposit ?? 0,
                    OverKmFee = 0, // Sẽ tính sau khi có thông tin km thực tế
                    ElectricityFee = 0, // Sẽ tính sau khi có thông tin điện năng
                    TotalAmount = (contract.RentalFee ?? 0) + (contract.Deposit ?? 0),
                    Status = contract.Status ?? "Pending",
                    CreatedDate = contract.CreatedDate,
                    ExpiryDate = DateTime.Now.AddMinutes(2), // QR code hết hạn sau 2 phút
                    FeeDetails = new List<ContractFeeDetailDto>
                    {
                        new ContractFeeDetailDto
                        {
                            FeeType = "Rental",
                            FeeName = "Phí thuê xe",
                            Amount = contract.RentalFee ?? 0,
                            Description = $"Thuê xe từ {order.StartTime:dd/MM/yyyy HH:mm} đến {order.EndTime:dd/MM/yyyy HH:mm}"
                        },
                        new ContractFeeDetailDto
                        {
                            FeeType = "Deposit",
                            FeeName = "Phí cọc",
                            Amount = contract.Deposit ?? 0,
                            Description = "Phí cọc xe (sẽ hoàn trả khi trả xe)"
                        }
                    }
                };

                return new ServiceResponse<ContractSummaryDto>
                {
                    Success = true,
                    Message = "Lấy thông tin hợp đồng thành công",
                    Data = contractSummary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contract summary for order {OrderId}: {Error}", orderId, ex.Message);
                return new ServiceResponse<ContractSummaryDto>
                {
                    Success = false,
                    Message = "Lỗi server khi lấy thông tin hợp đồng"
                };
            }
        }

        public async Task<ServiceResponse<PaymentQrResponseDto>> ConfirmContractAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ServiceResponse<PaymentQrResponseDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                // Kiểm tra trạng thái Order
                if (order.Status != "Pending")
                {
                    return new ServiceResponse<PaymentQrResponseDto>
                    {
                        Success = false,
                        Message = $"Không thể xác nhận hợp đồng. Trạng thái hiện tại: {order.Status}"
                    };
                }

                var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(orderId);
                if (contract == null)
                {
                    return new ServiceResponse<PaymentQrResponseDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy hợp đồng"
                    };
                }

                // Kiểm tra trạng thái Contract
                if (contract.Status != "Pending")
                {
                    return new ServiceResponse<PaymentQrResponseDto>
                    {
                        Success = false,
                        Message = $"Không thể xác nhận hợp đồng. Trạng thái hợp đồng hiện tại: {contract.Status}"
                    };
                }

                // Cập nhật trạng thái order thành "Confirmed"
                order.Status = "Confirmed";
                _unitOfWork.Contracts.Update(contract);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                // Tạo QR code thanh toán (mock)
                var qrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=EV{orderId}_{contract.ContractCode}";
                var paymentUrl = $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?orderId={orderId}&contractCode={contract.ContractCode}";

                var response = new PaymentQrResponseDto
                {
                    Success = true,
                    Message = "Tạo QR code thanh toán thành công",
                    QrCodeUrl = qrCodeUrl,
                    PaymentUrl = paymentUrl,
                    ExpiryDate = DateTime.Now.AddMinutes(2), // QR code hết hạn sau 2 phút
                    ContractCode = contract.ContractCode ?? "",
                    TotalAmount = (contract.RentalFee ?? 0) + (contract.Deposit ?? 0)
                };

                return new ServiceResponse<PaymentQrResponseDto>
                {
                    Success = true,
                    Message = "Xác nhận hợp đồng thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming contract for order {OrderId}: {Error}", orderId, ex.Message);
                return new ServiceResponse<PaymentQrResponseDto>
                {
                    Success = false,
                    Message = "Lỗi server khi xác nhận hợp đồng"
                };
            }
        }

        public async Task<ServiceResponse<RentalResponseDto>> StaffConfirmRentalAsync(int orderId, StaffConfirmationDto request)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ServiceResponse<RentalResponseDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn thuê"
                    };
                }

                var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(orderId);
                if (contract == null)
                {
                    return new ServiceResponse<RentalResponseDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy hợp đồng"
                    };
                }

                // Kiểm tra trạng thái Order hợp lệ cho staff confirmation
                if (order.Status != "Confirmed" && order.Status != "Active")
                {
                    return new ServiceResponse<RentalResponseDto>
                    {
                        Success = false,
                        Message = $"Không thể thực hiện xác nhận. Trạng thái hiện tại: {order.Status}"
                    };
                }

                if (request.IsConfirmed)
                {
                    // Staff xác nhận đúng người
                    if (request.Action == "Handover")
                    {
                        // Chỉ cho phép bàn giao xe khi trạng thái là "Confirmed"
                        if (order.Status != "Confirmed")
                        {
                            return new ServiceResponse<RentalResponseDto>
                            {
                                Success = false,
                                Message = "Chỉ có thể bàn giao xe khi đơn thuê đã được xác nhận"
                            };
                        }

                        // Bàn giao xe
                        order.Status = "Active";
                        
                        // Cập nhật trạng thái xe thành "Rented"
                        var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetByOrderIdAsync(orderId);
                        foreach (var orderLicensePlate in orderLicensePlates)
                        {
                            var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(orderLicensePlate.LicensePlateId);
                            if (licensePlate != null)
                            {
                                licensePlate.Status = "Rented";
                                _unitOfWork.LicensePlates.Update(licensePlate);
                            }
                        }
                    }
                    else if (request.Action == "Return")
                    {
                        // Chỉ cho phép trả xe khi trạng thái là "Active"
                        if (order.Status != "Active")
                        {
                            return new ServiceResponse<RentalResponseDto>
                            {
                                Success = false,
                                Message = "Chỉ có thể trả xe khi xe đang được thuê"
                            };
                        }

                        // Trả xe
                        order.Status = "Completed";
                        
                        // Cập nhật trạng thái xe thành "Available"
                        var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetByOrderIdAsync(orderId);
                        foreach (var orderLicensePlate in orderLicensePlates)
                        {
                            var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(orderLicensePlate.LicensePlateId);
                            if (licensePlate != null)
                            {
                                licensePlate.Status = "Available";
                                _unitOfWork.LicensePlates.Update(licensePlate);
                            }
                        }
                    }
                    else
                    {
                        return new ServiceResponse<RentalResponseDto>
                        {
                            Success = false,
                            Message = "Action không hợp lệ. Chỉ chấp nhận 'Handover' hoặc 'Return'"
                        };
                    }
                }
                else
                {
                    // Staff từ chối xác nhận
                    order.Status = "Rejected";
                    
                    // Cập nhật trạng thái xe thành "Available" khi bị từ chối
                    var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetByOrderIdAsync(orderId);
                    foreach (var orderLicensePlate in orderLicensePlates)
                    {
                        var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(orderLicensePlate.LicensePlateId);
                        if (licensePlate != null)
                        {
                            licensePlate.Status = "Available";
                            _unitOfWork.LicensePlates.Update(licensePlate);
                        }
                    }
                }

                _unitOfWork.Orders.Update(order);
                // Trạng thái hợp đồng bám theo Order, không cập nhật trực tiếp Contract.Status anymore
                await _unitOfWork.SaveChangesAsync();

                // Reload order with full navigation properties and create response
                var updatedOrder = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (updatedOrder == null)
                {
                    return new ServiceResponse<RentalResponseDto>
                    {
                        Success = false,
                        Message = "Lỗi khi tải lại thông tin đơn thuê"
                    };
                }

                var rentalDto = await PopulateRentalDtoAsync(updatedOrder);

                return new ServiceResponse<RentalResponseDto>
                {
                    Success = true,
                    Message = request.IsConfirmed ? "Xác nhận thành công" : "Từ chối xác nhận",
                    Data = new RentalResponseDto
                    {
                        Success = true,
                        Message = request.IsConfirmed ? "Xác nhận thành công" : "Từ chối xác nhận",
                        Data = rentalDto,
                        OrderId = rentalDto.OrderId,
                        ContractId = rentalDto.ContractId
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in staff confirmation for order {OrderId}: {Error}", orderId, ex.Message);
                return new ServiceResponse<RentalResponseDto>
                {
                    Success = false,
                    Message = "Lỗi server khi xác nhận đơn thuê"
                };
            }
        }
    }
}
