using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RentalService> _logger;

        public RentalService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RentalService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RentalListResponseDto> GetAllRentalsAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var rentalDtos = await MapOrdersToRentalDtos(orders.ToList());

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
                _logger.LogError(ex, "Error getting all rentals");
                return new RentalListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy danh sách đơn thuê",
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

                var rentalDto = await MapOrderToRentalDto(order);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Lấy thông tin đơn thuê thành công",
                    Data = rentalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rental by id: {OrderId}", orderId);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy thông tin đơn thuê"
                };
            }
        }

        public async Task<RentalListResponseDto> GetUserRentalsAsync(int userId)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var userOrders = orders.Where(o => o.UserId == userId).ToList();
                var rentalDtos = await MapOrdersToRentalDtos(userOrders);

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
                _logger.LogError(ex, "Error getting user rentals for user: {UserId}", userId);
                return new RentalListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy lịch sử thuê xe",
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
            try
            {
                // Kiểm tra xe có sẵn không
                foreach (var vehicleId in createDto.VehicleIds)
                {
                    if (!await IsVehicleAvailableAsync(vehicleId, createDto.StartTime, createDto.EndTime))
                    {
                        return new RentalResponseDto
                        {
                            Success = false,
                            Message = $"Xe ID {vehicleId} không có sẵn trong khoảng thời gian đã chọn"
                        };
                    }
                }

                // Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    StartTime = createDto.StartTime,
                    EndTime = createDto.EndTime,
                    Status = "Pending",
                    TotalAmount = 0 // Sẽ tính sau
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Tạo Order_LicensePlate cho mỗi xe
                var totalAmount = 0m;
                foreach (var vehicleId in createDto.VehicleIds)
                {
                    var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(vehicleId);
                    if (vehicle?.PricePerDay != null)
                    {
                        var days = (int)Math.Ceiling((createDto.EndTime - createDto.StartTime).TotalDays);
                        var vehicleCost = vehicle.PricePerDay.Value * days;
                        totalAmount += vehicleCost;

                        // Lấy biển số có sẵn của xe
                        var licensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
                        var availableLicensePlate = licensePlates
                            .FirstOrDefault(lp => lp.VehicleId == vehicleId && lp.Status == "Available");

                        if (availableLicensePlate != null)
                        {
                            var orderLicensePlate = new Order_LicensePlate
                            {
                                OrderId = order.OrderId,
                                LicensePlateId = availableLicensePlate.LicensePlateId
                            };
                            await _unitOfWork.OrderLicensePlates.AddAsync(orderLicensePlate);
                        }
                    }
                }

                // Cập nhật tổng tiền
                order.TotalAmount = totalAmount;
                _unitOfWork.Orders.Update(order);

                // Tạo Contract
                var contract = new Contract
                {
                    OrderId = order.OrderId,
                    CreatedDate = DateTime.Now,
                    Status = "Pending",
                    Deposit = createDto.DepositAmount ?? (totalAmount * 0.2m), // 20% cọc
                    RentalFee = totalAmount,
                    ExtraFee = 0
                };

                await _unitOfWork.Contracts.AddAsync(contract);
                await _unitOfWork.SaveChangesAsync();

                var rentalDto = await MapOrderToRentalDto(order);

                _logger.LogInformation("Created rental {OrderId} for user {UserId}", order.OrderId, userId);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Tạo đơn thuê thành công",
                    Data = rentalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental for user {UserId}", userId);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi tạo đơn thuê"
                };
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

                // Kiểm tra trạng thái có thể cập nhật không
                if (order.Status == "Completed" || order.Status == "Cancelled")
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Không thể cập nhật đơn thuê đã hoàn thành hoặc đã hủy"
                    };
                }

                // Cập nhật thông tin
                if (updateDto.StartTime.HasValue) order.StartTime = updateDto.StartTime.Value;
                if (updateDto.EndTime.HasValue) order.EndTime = updateDto.EndTime.Value;
                if (!string.IsNullOrEmpty(updateDto.Status)) order.Status = updateDto.Status;

                // Nếu cập nhật xe, kiểm tra xe có sẵn
                if (updateDto.VehicleIds != null && updateDto.VehicleIds.Any())
                {
                    foreach (var vehicleId in updateDto.VehicleIds)
                    {
                        if (!await IsVehicleAvailableAsync(vehicleId, order.StartTime ?? DateTime.Now, order.EndTime ?? DateTime.Now.AddDays(1)))
                        {
                            return new RentalResponseDto
                            {
                                Success = false,
                                Message = $"Xe ID {vehicleId} không có sẵn trong khoảng thời gian đã chọn"
                            };
                        }
                    }
                }

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                var rentalDto = await MapOrderToRentalDto(order);

                _logger.LogInformation("Updated rental {OrderId}", orderId);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Cập nhật đơn thuê thành công",
                    Data = rentalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rental {OrderId}", orderId);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi cập nhật đơn thuê"
                };
            }
        }

        public async Task<RentalResponseDto> CancelRentalAsync(int orderId)
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

                if (order.Status == "Completed")
                {
                    return new RentalResponseDto
                    {
                        Success = false,
                        Message = "Không thể hủy đơn thuê đã hoàn thành"
                    };
                }

                order.Status = "Cancelled";
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                var rentalDto = await MapOrderToRentalDto(order);

                _logger.LogInformation("Cancelled rental {OrderId}", orderId);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Hủy đơn thuê thành công",
                    Data = rentalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling rental {OrderId}", orderId);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi hủy đơn thuê"
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

                var rentalDto = await MapOrderToRentalDto(order);

                _logger.LogInformation("Completed rental {OrderId}", orderId);

                return new RentalResponseDto
                {
                    Success = true,
                    Message = "Hoàn thành đơn thuê thành công",
                    Data = rentalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing rental {OrderId}", orderId);
                return new RentalResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi hoàn thành đơn thuê"
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
                    // Kiểm tra xe có sẵn không
                    if (!await IsVehicleAvailableAsync(vehicle.VehicleId, searchDto.StartTime, searchDto.EndTime))
                        continue;

                    // Apply filters
                    if (searchDto.StationId.HasValue && vehicle.StationId != searchDto.StationId.Value)
                        continue;

                    if (searchDto.BrandId.HasValue && vehicle.BrandId != searchDto.BrandId.Value)
                        continue;

                    var availableVehicle = new AvailableVehicleDto
                    {
                        VehicleId = vehicle.VehicleId,
                        Model = vehicle.Model,
                        BrandName = vehicle.Brand?.BrandName ?? "",
                        PricePerDay = vehicle.PricePerDay,
                        SeatNumber = vehicle.SeatNumber,
                        VehicleImage = vehicle.VehicleImage,
                        StationName = vehicle.Station?.StationName,
                        StationId = vehicle.StationId,
                        Battery = vehicle.Battery,
                        RangeKm = vehicle.RangeKm,
                        Status = vehicle.Status,
                        EstimatedTotalCost = vehicle.PricePerDay * (int)Math.Ceiling((searchDto.EndTime - searchDto.StartTime).TotalDays)
                    };

                    // Lấy biển số có sẵn
                    var licensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
                    var vehicleLicensePlates = licensePlates
                        .Where(lp => lp.VehicleId == vehicle.VehicleId && lp.Status == "Available")
                        .Select(lp => new AvailableLicensePlateDto
                        {
                            LicensePlateId = lp.LicensePlateId,
                            LicensePlateNumber = lp.LicensePlateId, // LicensePlateId is the number
                            Status = lp.Status
                        })
                        .ToList();

                    availableVehicle.AvailableLicensePlates = vehicleLicensePlates;
                    availableVehicles.Add(availableVehicle);
                }

                // Apply pagination
                var totalCount = availableVehicles.Count;
                var pagedVehicles = availableVehicles
                    .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

                return new AvailableVehicleListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách xe có sẵn thành công",
                    Data = pagedVehicles,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available vehicles");
                return new AvailableVehicleListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy danh sách xe có sẵn",
                    Data = new List<AvailableVehicleDto>(),
                    TotalCount = 0,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = 0
                };
            }
        }

        public async Task<RentalListResponseDto> SearchRentalsAsync(RentalSearchDto searchDto)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var query = orders.AsQueryable();

                // Apply filters
                if (searchDto.UserId.HasValue)
                    query = query.Where(o => o.UserId == searchDto.UserId.Value);

                if (!string.IsNullOrEmpty(searchDto.Status))
                    query = query.Where(o => o.Status == searchDto.Status);

                if (searchDto.StartDate.HasValue)
                    query = query.Where(o => o.OrderDate >= searchDto.StartDate.Value);

                if (searchDto.EndDate.HasValue)
                    query = query.Where(o => o.OrderDate <= searchDto.EndDate.Value);

                // Apply sorting
                query = searchDto.SortBy?.ToLower() switch
                {
                    "totalamount" => searchDto.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(o => o.TotalAmount)
                        : query.OrderBy(o => o.TotalAmount),
                    "status" => searchDto.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(o => o.Status)
                        : query.OrderBy(o => o.Status),
                    _ => searchDto.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(o => o.OrderDate)
                        : query.OrderBy(o => o.OrderDate)
                };

                var totalCount = query.Count();
                var pagedOrders = query
                    .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToList();

                var rentalDtos = await MapOrdersToRentalDtos(pagedOrders);
                var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

                return new RentalListResponseDto
                {
                    Success = true,
                    Message = "Tìm kiếm đơn thuê thành công",
                    Data = rentalDtos,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching rentals");
                return new RentalListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi tìm kiếm đơn thuê",
                    Data = new List<RentalDto>(),
                    TotalCount = 0,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
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
                _logger.LogError(ex, "Error calculating rental cost for vehicle {VehicleId}", vehicleId);
                return 0;
            }
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var conflictingOrders = orders.Where(o => 
                    o.Status != "Cancelled" && 
                    o.Status != "Completed" &&
                    o.StartTime.HasValue && 
                    o.EndTime.HasValue &&
                    !(endTime <= o.StartTime.Value || startTime >= o.EndTime.Value))
                    .ToList();

                // Kiểm tra xem xe có trong đơn thuê xung đột không
                foreach (var order in conflictingOrders)
                {
                    var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetAllAsync();
                    var vehicleLicensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
                    
                    var hasVehicleInOrder = orderLicensePlates
                        .Where(olp => olp.OrderId == order.OrderId)
                        .Join(vehicleLicensePlates, 
                            olp => olp.LicensePlateId, 
                            vlp => vlp.LicensePlateId, 
                            (olp, vlp) => vlp.VehicleId)
                        .Any(vid => vid == vehicleId);

                    if (hasVehicleInOrder)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking vehicle availability for vehicle {VehicleId}", vehicleId);
                return false;
            }
        }

        private async Task<List<RentalDto>> MapOrdersToRentalDtos(List<Order> orders)
        {
            var rentalDtos = new List<RentalDto>();
            foreach (var order in orders)
            {
                var rentalDto = await MapOrderToRentalDto(order);
                rentalDtos.Add(rentalDto);
            }
            return rentalDtos;
        }

        private async Task<RentalDto> MapOrderToRentalDto(Order order)
        {
            var rentalDto = new RentalDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                StartTime = order.StartTime,
                EndTime = order.EndTime,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                UserId = order.UserId,
                UserName = order.User?.FullName,
                UserEmail = order.User?.Email,
                TotalDays = order.StartTime.HasValue && order.EndTime.HasValue 
                    ? (int)Math.Ceiling((order.EndTime.Value - order.StartTime.Value).TotalDays)
                    : 0
            };

            // Lấy thông tin xe
            var orderLicensePlates = await _unitOfWork.OrderLicensePlates.GetAllAsync();
            var licensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync();

            var vehicleIds = orderLicensePlates
                .Where(olp => olp.OrderId == order.OrderId)
                .Join(licensePlates, 
                    olp => olp.LicensePlateId, 
                    lp => lp.LicensePlateId, 
                    (olp, lp) => lp.VehicleId)
                .Distinct()
                .ToList();

            foreach (var vehicleId in vehicleIds)
            {
                var vehicle = vehicles.FirstOrDefault(v => v.VehicleId == vehicleId);
                if (vehicle != null)
                {
                    var rentalVehicle = new RentalVehicleDto
                    {
                        VehicleId = vehicle.VehicleId,
                        Model = vehicle.Model,
                        BrandName = vehicle.Brand?.BrandName ?? "",
                        PricePerDay = vehicle.PricePerDay,
                        SeatNumber = vehicle.SeatNumber,
                        VehicleImage = vehicle.VehicleImage,
                        StationName = vehicle.Station?.StationName
                    };

                    // Lấy biển số của xe trong đơn thuê
                    var vehicleLicensePlates = orderLicensePlates
                        .Where(olp => olp.OrderId == order.OrderId)
                        .Join(licensePlates, 
                            olp => olp.LicensePlateId, 
                            lp => lp.LicensePlateId, 
                            (olp, lp) => lp)
                        .Where(lp => lp.VehicleId == vehicleId)
                        .Select(lp => new RentalLicensePlateDto
                        {
                            LicensePlateId = lp.LicensePlateId,
                            LicensePlateNumber = lp.LicensePlateId,
                            Status = lp.Status
                        })
                        .ToList();

                    rentalVehicle.LicensePlates = vehicleLicensePlates;
                    rentalDto.Vehicles.Add(rentalVehicle);
                }
            }

            // Lấy thông tin hợp đồng
            var contracts = await _unitOfWork.Contracts.GetAllAsync();
            var orderContracts = contracts.Where(c => c.OrderId == order.OrderId).ToList();

            foreach (var contract in orderContracts)
            {
                var rentalContract = new RentalContractDto
                {
                    ContractId = contract.ContractId,
                    CreatedDate = contract.CreatedDate,
                    Status = contract.Status,
                    Deposit = contract.Deposit,
                    RentalFee = contract.RentalFee,
                    ExtraFee = contract.ExtraFee
                };

                // Lấy thông tin thanh toán
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var contractPayments = payments.Where(p => p.ContractId == contract.ContractId).ToList();

                foreach (var payment in contractPayments)
                {
                    var transactions = await _unitOfWork.Transactions.GetAllAsync();
                    var paymentTransaction = transactions.FirstOrDefault(t => t.PaymentId == payment.PaymentId);

                    var rentalPayment = new RentalPaymentDto
                    {
                        PaymentId = payment.PaymentId,
                        PaymentDate = payment.PaymentDate,
                        Amount = payment.Amount ?? 0,
                        Status = payment.Status ?? "",
                        PaymentMethod = "VNPay",
                        TransactionId = paymentTransaction?.TransactionId.ToString() ?? ""
                    };

                    rentalContract.Payments.Add(rentalPayment);
                }

                rentalDto.Contracts.Add(rentalContract);
            }

            // Tính toán tổng phí
            if (rentalDto.Contracts.Any())
            {
                var contract = rentalDto.Contracts.First();
                rentalDto.DepositAmount = contract.Deposit;
                rentalDto.RentalFee = contract.RentalFee;
                rentalDto.ExtraFee = contract.ExtraFee;
                rentalDto.DailyRate = rentalDto.RentalFee / Math.Max(rentalDto.TotalDays, 1);
            }

            return rentalDto;
        }
    }
}

