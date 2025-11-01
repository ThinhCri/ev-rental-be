using AutoMapper;
using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class ComplaintService : BaseService, IComplaintService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ComplaintService(IUnitOfWork unitOfWork, ApplicationDbContext context, IMapper mapper, ILogger<ComplaintService> logger) : base(mapper, logger)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ComplaintResponseDto> CreateComplaintAsync(CreateComplaintDto createDto, int userId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(createDto.OrderId);
                if (order == null)
                {
                    return new ComplaintResponseDto
                    {
                        Success = false,
                        Message = "Order not found"
                    };
                }

                if (order.UserId != userId)
                {
                    return new ComplaintResponseDto
                    {
                        Success = false,
                        Message = "You don't have permission to create complaint for this order"
                    };
                }

                var complaint = new Complaint
                {
                    Description = createDto.Description,
                    Status = "Pending",
                    UserId = userId,
                    OrderId = createDto.OrderId,
                    ComplaintDate = DateTime.Now
                };

                await _unitOfWork.Complaints.AddAsync(complaint);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Complaint {ComplaintId} created for order {OrderId} by user {UserId}", 
                    complaint.ComplaintId, createDto.OrderId, userId);

                var createdComplaint = await _unitOfWork.Complaints.GetByIdAsync(complaint.ComplaintId);
                if (createdComplaint != null)
                {
                    await _context.Entry(createdComplaint).Reference(c => c.User).LoadAsync();
                    await _context.Entry(createdComplaint).Reference(c => c.Order).LoadAsync();
                    await _context.Entry(createdComplaint.Order).Collection(o => o.OrderLicensePlates).LoadAsync();
                }

                var vehicles = new List<ComplaintVehicleDto>();
                if (createdComplaint?.Order?.OrderLicensePlates != null)
                {
                    foreach (var orderLicensePlate in createdComplaint.Order.OrderLicensePlates)
                    {
                        await _context.Entry(orderLicensePlate).Reference(olp => olp.LicensePlate).LoadAsync();
                        if (orderLicensePlate.LicensePlate != null)
                        {
                            await _context.Entry(orderLicensePlate.LicensePlate).Reference(lp => lp.Vehicle).LoadAsync();
                            if (orderLicensePlate.LicensePlate.Vehicle != null)
                            {
                                await _context.Entry(orderLicensePlate.LicensePlate.Vehicle).Reference(v => v.Brand).LoadAsync();
                                
                                vehicles.Add(new ComplaintVehicleDto
                                {
                                    VehicleId = orderLicensePlate.LicensePlate.Vehicle.VehicleId,
                                    Model = orderLicensePlate.LicensePlate.Vehicle.Model,
                                    BrandName = orderLicensePlate.LicensePlate.Vehicle.Brand?.BrandName,
                                    VehicleImage = orderLicensePlate.LicensePlate.Vehicle.VehicleImage,
                                    PlateNumber = orderLicensePlate.LicensePlate.PlateNumber,
                                    LicensePlateId = orderLicensePlate.LicensePlate.LicensePlateId
                                });
                            }
                        }
                    }
                }

                var complaintDto = new ComplaintDto
                {
                    ComplaintId = complaint.ComplaintId,
                    ComplaintDate = complaint.ComplaintDate,
                    Description = complaint.Description,
                    Status = complaint.Status,
                    UserId = complaint.UserId,
                    OrderId = complaint.OrderId,
                    UserName = createdComplaint?.User?.FullName,
                    UserEmail = createdComplaint?.User?.Email,
                    OrderStartTime = createdComplaint?.Order?.StartTime,
                    OrderEndTime = createdComplaint?.Order?.EndTime,
                    Vehicles = vehicles
                };

                return new ComplaintResponseDto
                {
                    Success = true,
                    Message = "Complaint created successfully",
                    Data = complaintDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating complaint: {Error}", ex.Message);
                return new ComplaintResponseDto
                {
                    Success = false,
                    Message = $"Error creating complaint: {ex.Message}"
                };
            }
        }

        public async Task<ComplaintListResponseDto> GetAllComplaintsAsync()
        {
            try
            {
                var complaints = await _unitOfWork.Complaints.GetAllAsync();

                var complaintsList = complaints.ToList();
                foreach (var complaint in complaintsList)
                {
                    await _context.Entry(complaint).Reference(c => c.User).LoadAsync();
                    await _context.Entry(complaint).Reference(c => c.Order).LoadAsync();
                    await _context.Entry(complaint.Order).Collection(o => o.OrderLicensePlates).LoadAsync();
                }

                var complaintDtos = new List<ComplaintDto>();
                foreach (var c in complaintsList)
                {
                    var vehicles = new List<ComplaintVehicleDto>();
                    if (c.Order?.OrderLicensePlates != null)
                    {
                        foreach (var orderLicensePlate in c.Order.OrderLicensePlates)
                        {
                            await _context.Entry(orderLicensePlate).Reference(olp => olp.LicensePlate).LoadAsync();
                            if (orderLicensePlate.LicensePlate != null)
                            {
                                await _context.Entry(orderLicensePlate.LicensePlate).Reference(lp => lp.Vehicle).LoadAsync();
                                if (orderLicensePlate.LicensePlate.Vehicle != null)
                                {
                                    await _context.Entry(orderLicensePlate.LicensePlate.Vehicle).Reference(v => v.Brand).LoadAsync();
                                    
                                    vehicles.Add(new ComplaintVehicleDto
                                    {
                                        VehicleId = orderLicensePlate.LicensePlate.Vehicle.VehicleId,
                                        Model = orderLicensePlate.LicensePlate.Vehicle.Model,
                                        BrandName = orderLicensePlate.LicensePlate.Vehicle.Brand?.BrandName,
                                        VehicleImage = orderLicensePlate.LicensePlate.Vehicle.VehicleImage,
                                        PlateNumber = orderLicensePlate.LicensePlate.PlateNumber,
                                        LicensePlateId = orderLicensePlate.LicensePlate.LicensePlateId
                                    });
                                }
                            }
                        }
                    }

                    complaintDtos.Add(new ComplaintDto
                    {
                        ComplaintId = c.ComplaintId,
                        ComplaintDate = c.ComplaintDate,
                        Description = c.Description,
                        Status = c.Status,
                        UserId = c.UserId,
                        OrderId = c.OrderId,
                        UserName = c.User?.FullName,
                        UserEmail = c.User?.Email,
                        OrderStartTime = c.Order?.StartTime,
                        OrderEndTime = c.Order?.EndTime,
                        Vehicles = vehicles
                    });
                }

                return new ComplaintListResponseDto
                {
                    Success = true,
                    Message = "Complaints retrieved successfully",
                    Data = complaintDtos,
                    TotalCount = complaintDtos.Count,
                    PageNumber = 1,
                    PageSize = complaintDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all complaints: {Error}", ex.Message);
                return new ComplaintListResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving complaints: {ex.Message}",
                    Data = new List<ComplaintDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<ComplaintResponseDto> UpdateComplaintStatusAsync(int complaintId, UpdateComplaintStatusDto updateDto)
        {
            try
            {
                var complaint = await _unitOfWork.Complaints.GetByIdAsync(complaintId);
                if (complaint == null)
                {
                    return new ComplaintResponseDto
                    {
                        Success = false,
                        Message = "Complaint not found"
                    };
                }

                complaint.Status = updateDto.Status;
                _unitOfWork.Complaints.Update(complaint);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Complaint {ComplaintId} status updated to {Status}", 
                    complaintId, updateDto.Status);

                await _context.Entry(complaint).Reference(c => c.User).LoadAsync();
                await _context.Entry(complaint).Reference(c => c.Order).LoadAsync();
                await _context.Entry(complaint.Order).Collection(o => o.OrderLicensePlates).LoadAsync();

                var vehicles = new List<ComplaintVehicleDto>();
                if (complaint.Order?.OrderLicensePlates != null)
                {
                    foreach (var orderLicensePlate in complaint.Order.OrderLicensePlates)
                    {
                        await _context.Entry(orderLicensePlate).Reference(olp => olp.LicensePlate).LoadAsync();
                        if (orderLicensePlate.LicensePlate != null)
                        {
                            await _context.Entry(orderLicensePlate.LicensePlate).Reference(lp => lp.Vehicle).LoadAsync();
                            if (orderLicensePlate.LicensePlate.Vehicle != null)
                            {
                                await _context.Entry(orderLicensePlate.LicensePlate.Vehicle).Reference(v => v.Brand).LoadAsync();
                                
                                vehicles.Add(new ComplaintVehicleDto
                                {
                                    VehicleId = orderLicensePlate.LicensePlate.Vehicle.VehicleId,
                                    Model = orderLicensePlate.LicensePlate.Vehicle.Model,
                                    BrandName = orderLicensePlate.LicensePlate.Vehicle.Brand?.BrandName,
                                    VehicleImage = orderLicensePlate.LicensePlate.Vehicle.VehicleImage,
                                    PlateNumber = orderLicensePlate.LicensePlate.PlateNumber,
                                    LicensePlateId = orderLicensePlate.LicensePlate.LicensePlateId
                                });
                            }
                        }
                    }
                }

                var complaintDto = new ComplaintDto
                {
                    ComplaintId = complaint.ComplaintId,
                    ComplaintDate = complaint.ComplaintDate,
                    Description = complaint.Description,
                    Status = complaint.Status,
                    UserId = complaint.UserId,
                    OrderId = complaint.OrderId,
                    UserName = complaint.User?.FullName,
                    UserEmail = complaint.User?.Email,
                    OrderStartTime = complaint.Order?.StartTime,
                    OrderEndTime = complaint.Order?.EndTime,
                    Vehicles = vehicles
                };

                return new ComplaintResponseDto
                {
                    Success = true,
                    Message = "Complaint status updated successfully",
                    Data = complaintDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complaint status {ComplaintId}: {Error}", 
                    complaintId, ex.Message);
                return new ComplaintResponseDto
                {
                    Success = false,
                    Message = $"Error updating complaint status: {ex.Message}"
                };
            }
        }

        public async Task<ComplaintListResponseDto> GetUserComplaintsAsync(int userId)
        {
            try
            {
                var complaints = await _unitOfWork.Complaints.GetComplaintsByUserIdAsync(userId);

                var complaintsList = complaints.ToList();
                foreach (var complaint in complaintsList)
                {
                    await _context.Entry(complaint).Reference(c => c.User).LoadAsync();
                    await _context.Entry(complaint).Reference(c => c.Order).LoadAsync();
                    await _context.Entry(complaint.Order).Collection(o => o.OrderLicensePlates).LoadAsync();
                }

                var complaintDtos = new List<ComplaintDto>();
                foreach (var c in complaintsList)
                {
                    var vehicles = new List<ComplaintVehicleDto>();
                    if (c.Order?.OrderLicensePlates != null)
                    {
                        foreach (var orderLicensePlate in c.Order.OrderLicensePlates)
                        {
                            await _context.Entry(orderLicensePlate).Reference(olp => olp.LicensePlate).LoadAsync();
                            if (orderLicensePlate.LicensePlate != null)
                            {
                                await _context.Entry(orderLicensePlate.LicensePlate).Reference(lp => lp.Vehicle).LoadAsync();
                                if (orderLicensePlate.LicensePlate.Vehicle != null)
                                {
                                    await _context.Entry(orderLicensePlate.LicensePlate.Vehicle).Reference(v => v.Brand).LoadAsync();
                                    
                                    vehicles.Add(new ComplaintVehicleDto
                                    {
                                        VehicleId = orderLicensePlate.LicensePlate.Vehicle.VehicleId,
                                        Model = orderLicensePlate.LicensePlate.Vehicle.Model,
                                        BrandName = orderLicensePlate.LicensePlate.Vehicle.Brand?.BrandName,
                                        VehicleImage = orderLicensePlate.LicensePlate.Vehicle.VehicleImage,
                                        PlateNumber = orderLicensePlate.LicensePlate.PlateNumber,
                                        LicensePlateId = orderLicensePlate.LicensePlate.LicensePlateId
                                    });
                                }
                            }
                        }
                    }

                    complaintDtos.Add(new ComplaintDto
                    {
                        ComplaintId = c.ComplaintId,
                        ComplaintDate = c.ComplaintDate,
                        Description = c.Description,
                        Status = c.Status,
                        UserId = c.UserId,
                        OrderId = c.OrderId,
                        UserName = c.User?.FullName,
                        UserEmail = c.User?.Email,
                        OrderStartTime = c.Order?.StartTime,
                        OrderEndTime = c.Order?.EndTime,
                        Vehicles = vehicles
                    });
                }

                return new ComplaintListResponseDto
                {
                    Success = true,
                    Message = "User complaints retrieved successfully",
                    Data = complaintDtos,
                    TotalCount = complaintDtos.Count,
                    PageNumber = 1,
                    PageSize = complaintDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user complaints for user {UserId}: {Error}", userId, ex.Message);
                return new ComplaintListResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving user complaints: {ex.Message}",
                    Data = new List<ComplaintDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<ComplaintListResponseDto> GetComplaintsByStationAsync(int stationId)
        {
            try
            {
                var allComplaints = await _unitOfWork.Complaints.GetAllAsync();

                var complaintsList = allComplaints.ToList();
                foreach (var complaint in complaintsList)
                {
                    await _context.Entry(complaint).Reference(c => c.User).LoadAsync();
                    await _context.Entry(complaint).Reference(c => c.Order).LoadAsync();
                    await _context.Entry(complaint.Order).Collection(o => o.OrderLicensePlates).LoadAsync();
                }

                var stationComplaints = new List<Complaint>();
                foreach (var c in complaintsList)
                {
                    if (c.Order?.OrderLicensePlates != null)
                    {
                        foreach (var orderLicensePlate in c.Order.OrderLicensePlates)
                        {
                            await _context.Entry(orderLicensePlate).Reference(olp => olp.LicensePlate).LoadAsync();
                            if (orderLicensePlate.LicensePlate != null)
                            {
                                await _context.Entry(orderLicensePlate.LicensePlate).Reference(lp => lp.Station).LoadAsync();
                                if (orderLicensePlate.LicensePlate.Station?.StationId == stationId)
                                {
                                    stationComplaints.Add(c);
                                    break;
                                }
                            }
                        }
                    }
                }

                var complaintDtos = new List<ComplaintDto>();
                foreach (var c in stationComplaints)
                {
                    var vehicles = new List<ComplaintVehicleDto>();
                    if (c.Order?.OrderLicensePlates != null)
                    {
                        foreach (var orderLicensePlate in c.Order.OrderLicensePlates)
                        {
                            await _context.Entry(orderLicensePlate).Reference(olp => olp.LicensePlate).LoadAsync();
                            if (orderLicensePlate.LicensePlate != null)
                            {
                                await _context.Entry(orderLicensePlate.LicensePlate).Reference(lp => lp.Vehicle).LoadAsync();
                                if (orderLicensePlate.LicensePlate.Vehicle != null)
                                {
                                    await _context.Entry(orderLicensePlate.LicensePlate.Vehicle).Reference(v => v.Brand).LoadAsync();
                                    
                                    vehicles.Add(new ComplaintVehicleDto
                                    {
                                        VehicleId = orderLicensePlate.LicensePlate.Vehicle.VehicleId,
                                        Model = orderLicensePlate.LicensePlate.Vehicle.Model,
                                        BrandName = orderLicensePlate.LicensePlate.Vehicle.Brand?.BrandName,
                                        VehicleImage = orderLicensePlate.LicensePlate.Vehicle.VehicleImage,
                                        PlateNumber = orderLicensePlate.LicensePlate.PlateNumber,
                                        LicensePlateId = orderLicensePlate.LicensePlate.LicensePlateId
                                    });
                                }
                            }
                        }
                    }

                    complaintDtos.Add(new ComplaintDto
                    {
                        ComplaintId = c.ComplaintId,
                        ComplaintDate = c.ComplaintDate,
                        Description = c.Description,
                        Status = c.Status,
                        UserId = c.UserId,
                        OrderId = c.OrderId,
                        UserName = c.User?.FullName,
                        UserEmail = c.User?.Email,
                        OrderStartTime = c.Order?.StartTime,
                        OrderEndTime = c.Order?.EndTime,
                        Vehicles = vehicles
                    });
                }

                return new ComplaintListResponseDto
                {
                    Success = true,
                    Message = "Station complaints retrieved successfully",
                    Data = complaintDtos,
                    TotalCount = complaintDtos.Count,
                    PageNumber = 1,
                    PageSize = complaintDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints for station {StationId}: {Error}", stationId, ex.Message);
                return new ComplaintListResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving station complaints: {ex.Message}",
                    Data = new List<ComplaintDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }
    }
}

