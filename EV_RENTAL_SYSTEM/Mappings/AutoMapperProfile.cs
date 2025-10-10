using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.DriverLicenseNumber, opt => opt.MapFrom(src => src.Licenses.FirstOrDefault() != null ? src.Licenses.FirstOrDefault()!.LicenseNumber : null))
                .ForMember(dest => dest.DriverLicenseImage, opt => opt.MapFrom(src => src.Licenses.FirstOrDefault() != null ? src.Licenses.FirstOrDefault()!.LicenseImageUrl : null));

            // User management mappings (Admin)
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Will be hashed manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.Licenses, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Complaints, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Password not updated in this operation
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt not updated
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.Licenses, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Complaints, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            // Staff management mappings (Admin)
            CreateMap<CreateStaffDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Will be hashed manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => 2)) // Station Staff
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.Licenses, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Complaints, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<UpdateStaffDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Password not updated in this operation
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt not updated
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => 2)) // Station Staff
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.Licenses, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Complaints, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            // Vehicle mappings
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.ModelYear, opt => opt.MapFrom(src => src.ModelYear))
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom(src => src.PricePerDay))
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
                .ForMember(dest => dest.VehicleImage, opt => opt.MapFrom(src => src.VehicleImage))
                .ForMember(dest => dest.Battery, opt => opt.MapFrom(src => src.Battery))
                .ForMember(dest => dest.RangeKm, opt => opt.MapFrom(src => src.RangeKm))
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Will be calculated from LicensePlates
                // Station is derived from LicensePlates' Station
                .ForMember(dest => dest.StationId, opt => opt.Ignore())
                .ForMember(dest => dest.StationName, opt => opt.Ignore())
                .ForMember(dest => dest.StationStreet, opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.AvailableLicensePlates, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.LicensePlateNumbers, opt => opt.Ignore()); // Will be set manually

            CreateMap<CreateVehicleDto, Vehicle>()
                .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.LicensePlates, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleImage, opt => opt.Ignore()); // Will be set manually after upload

            CreateMap<UpdateVehicleDto, Vehicle>()
                .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.LicensePlates, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleImage, opt => opt.Ignore()); // Will be set manually after upload

            // Station mappings
            CreateMap<Station, StationDto>()
                .ForMember(dest => dest.StationId, opt => opt.MapFrom(src => src.StationId))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.StationName))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.FullAddress, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.VehicleCount, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.AvailableVehicleCount, opt => opt.Ignore()); // Will be set manually

            CreateMap<CreateStationDto, Station>()
                .ForMember(dest => dest.StationId, opt => opt.Ignore())
                .ForMember(dest => dest.LicensePlates, opt => opt.Ignore());

            CreateMap<UpdateStationDto, Station>()
                .ForMember(dest => dest.StationId, opt => opt.Ignore())
                .ForMember(dest => dest.LicensePlates, opt => opt.Ignore());

            // Order mappings
            CreateMap<Order, RentalDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Vehicles, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.Contracts, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.TotalDays, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.DailyRate, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.DepositAmount, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.RentalFee, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.ExtraFee, opt => opt.Ignore()); // Will be set manually

            CreateMap<CreateRentalDto, Order>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()) // Will be calculated
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Will be set from token
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLicensePlates, opt => opt.Ignore())
                .ForMember(dest => dest.Complaints, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore());

            CreateMap<UpdateRentalDto, Order>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLicensePlates, opt => opt.Ignore())
                .ForMember(dest => dest.Complaints, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore());
        }
    }
}


