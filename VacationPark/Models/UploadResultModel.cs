namespace VacationPark.Models
{
    public class UploadResultModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Facility> Facilities { get; set; }
        public IEnumerable<House> Houses { get; set; }
        public IEnumerable<Park> Parks { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }
    }
}
