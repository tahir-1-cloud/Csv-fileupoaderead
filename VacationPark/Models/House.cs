namespace VacationPark.Models
{
    public class House
    {
        public int HouseID { get; set; }
        public string? Street { get; set; }
        public int Number { get; set; }
        public bool IsActive { get; set; }
        public int Capacity { get; set; }
    }
}
