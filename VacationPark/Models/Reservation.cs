namespace VacationPark.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerID { get; set; }
        public int HouseID { get; set; }
    }
}
