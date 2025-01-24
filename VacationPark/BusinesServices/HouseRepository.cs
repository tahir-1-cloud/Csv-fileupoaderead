using System.Data.SqlClient;
using VacationPark.DBConnection;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.BusinesServices
{
    public class HouseRepository: IHouseRepository
    {
        private readonly DatabaseContext _context;

        public HouseRepository(DatabaseContext context)
        {
            _context = context;
        }

        public IEnumerable<House> GetHousesByPark(int parkId)
        {
            var houses = new List<House>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Houses WHERE ParkID = @ParkID", connection))
                {
                    command.Parameters.Add(new SqlParameter("@ParkID", parkId));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            houses.Add(new House
                            {
                                HouseID = reader.GetInt32(0),
                                Street = reader.GetString(1),
                                Number = reader.GetInt32(2),
                                IsActive = reader.GetBoolean(3),
                                Capacity = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return houses;
        }

        public IEnumerable<House> GetAvailableHouses(int parkId, int capacity)
        {
            var houses = new List<House>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = @"
            SELECT h.HouseID, h.Street, h.Number, h.IsActive, h.Capacity
            FROM Houses h
            INNER JOIN ParkHouses ph ON h.HouseID = ph.HouseID
            WHERE ph.ParkID = @ParkID AND h.IsActive = 1 AND h.Capacity >= @Capacity";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@ParkID", parkId));
                    command.Parameters.Add(new SqlParameter("@Capacity", capacity));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            houses.Add(new House
                            {
                                HouseID = reader.GetInt32(0),
                                Street = reader.GetString(1),
                                Number = reader.GetInt32(2),
                                IsActive = reader.GetBoolean(3),
                                Capacity = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return houses;
        }


        public void MarkHouseUnderMaintenance(int houseId)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE Houses SET IsActive = 0 WHERE HouseID = @HouseID", connection))
                {
                    command.Parameters.Add(new SqlParameter("@HouseID", houseId));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddHouse(House house)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "INSERT INTO Houses (HouseID, Street, Number, IsActive, Capacity) VALUES (@HouseID, @Street, @Number, @IsActive, @Capacity)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HouseID", house.HouseID);
                    command.Parameters.AddWithValue("@Street", house.Street);
                    command.Parameters.AddWithValue("@Number", house.Number);
                    command.Parameters.AddWithValue("@IsActive", house.IsActive);
                    command.Parameters.AddWithValue("@Capacity", house.Capacity);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<House> GetAllHouses()
        {
            var houses = new List<House>();

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Houses";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            houses.Add(new House
                            {
                                HouseID = reader.GetInt32(0),
                                Street = reader.GetString(1),
                                Number = reader.GetInt32(2),
                                IsActive = reader.GetBoolean(3),
                                Capacity = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }

            return houses;
        }
    }

}

