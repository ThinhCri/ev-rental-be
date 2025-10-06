using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IStationRepository : IGenericRepository<Station>
    {
        Task<IEnumerable<Station>> GetStationsByProvinceAsync(string province);
        Task<Station?> GetStationWithVehiclesAsync(int stationId);
    }
}




