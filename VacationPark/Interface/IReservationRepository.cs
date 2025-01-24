using VacationPark.Models;

namespace VacationPark.Interface
{
    public interface IReservationRepository
    {
        //in interface we just define function and implementaion in class 

        IEnumerable<Reservation> GetReservationsByCriteria(int? parkId, string customerName, DateTime? startDate, DateTime? endDate);
        void AddReservation(Reservation reservation);
        IEnumerable<Reservation> GetAllReservations();
    }
}
