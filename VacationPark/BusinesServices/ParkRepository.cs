using System.Collections.Generic;
using System.Data.SqlClient;
using VacationPark.DBConnection;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.Repositories
{
    public class ParkRepository : IParkRepository
    {
        private readonly DatabaseContext _context;

        public ParkRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void AddPark(Park park)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "INSERT INTO Parks (ParkID, Name, Location) VALUES (@ParkID, @Name, @Location)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParkID", park.ParkID);
                    command.Parameters.AddWithValue("@Name", park.Name);
                    command.Parameters.AddWithValue("@Location", park.Location);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Park> GetAllParks()
        {
            var parks = new List<Park>();

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Parks";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            parks.Add(new Park
                            {
                                ParkID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Location = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return parks;
        }
    }
}
