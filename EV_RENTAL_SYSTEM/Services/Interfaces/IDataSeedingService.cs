using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IDataSeedingService
    {
        Task SeedDataAsync();
        Task SeedRolesAsync();
        Task SeedLicenseTypesAsync();
        Task SeedBrandsAsync();
        Task SeedStationsAsync();
        Task SeedProcessStepsAsync();
        Task SeedSampleVehiclesAsync();
        Task SeedSampleLicensePlatesAsync();
    }
}
