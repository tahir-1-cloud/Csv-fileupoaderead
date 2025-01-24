using VacationPark.Models;

namespace VacationPark.Interface
{
    //Facilityinterface
    public interface IFacilityRepository
    {
        //in interface we just define function and implementaion in class 
        void AddFacility(Facility facility);
        IEnumerable<Facility> GetAllFacilities();
    }
}
