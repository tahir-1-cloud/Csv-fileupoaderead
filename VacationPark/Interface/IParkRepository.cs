using VacationPark.Models;

namespace VacationPark.Interface
{
    public interface IParkRepository
    {
        void AddPark(Park park);
        IEnumerable<Park> GetAllParks();
    }
}
