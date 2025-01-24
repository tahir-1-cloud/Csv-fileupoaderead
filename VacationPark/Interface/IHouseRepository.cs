using VacationPark.Models;

namespace VacationPark.Interface
{
    public interface IHouseRepository
    {
        IEnumerable<House> GetHousesByPark(int parkId);
        IEnumerable<House> GetAvailableHouses(int parkId, int capacity);
        void MarkHouseUnderMaintenance(int houseId);

        void AddHouse(House house);
        IEnumerable<House> GetAllHouses();

    }
}
