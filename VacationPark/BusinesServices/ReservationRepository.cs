using System.Data.SqlClient;
using VacationPark.DBConnection;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.BusinesServices
{
    public class ReservationRepository: IReservationRepository
    {
        private readonly DatabaseContext _context;

        public ReservationRepository(DatabaseContext context)
        {
            _context = context;
        }

        //Get reservation according to parameter
        public IEnumerable<Reservation> GetReservationsByCriteria(int? parkId, string customerName, DateTime? startDate, DateTime? endDate)
        {
            var reservations = new List<Reservation>();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = @"
            SELECT r.ReservationID, r.StartDate, r.EndDate, r.CustomerID, r.HouseID
            FROM Reservations r
            INNER JOIN Houses h ON r.HouseID = h.HouseID
            INNER JOIN ParkHouses ph ON h.HouseID = ph.HouseID
            WHERE 1=1";

                if (parkId.HasValue)
                    query += " AND ph.ParkID = @ParkID";
                if (!string.IsNullOrEmpty(customerName))
                    query += " AND r.CustomerID IN (SELECT CustomerID FROM Customers WHERE Name LIKE @CustomerName)";
                if (startDate.HasValue)
                    query += " AND r.StartDate >= @StartDate";
                if (endDate.HasValue)
                    query += " AND r.EndDate <= @EndDate";

                using (var command = new SqlCommand(query, connection))
                {
                    if (parkId.HasValue)
                        command.Parameters.Add(new SqlParameter("@ParkID", parkId));
                    if (!string.IsNullOrEmpty(customerName))
                        command.Parameters.Add(new SqlParameter("@CustomerName", "%" + customerName + "%"));
                    if (startDate.HasValue)
                        command.Parameters.Add(new SqlParameter("@StartDate", startDate));
                    if (endDate.HasValue)
                        command.Parameters.Add(new SqlParameter("@EndDate", endDate));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservations.Add(new Reservation
                            {
                                ReservationID = reader.GetInt32(0),
                                StartDate = reader.GetDateTime(1),
                                EndDate = reader.GetDateTime(2),
                                CustomerID = reader.GetInt32(3),
                                HouseID = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return reservations;
        }


        public void AddReservation(Reservation reservation)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if the ReservationID already exists
                        var checkQuery = "SELECT COUNT(1) FROM Reservations WHERE ReservationID = @ReservationID";
                        using (var checkCommand = new SqlCommand(checkQuery, connection, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                            var exists = (int)checkCommand.ExecuteScalar() > 0;

                            if (exists)
                            {
                                // Handle duplicate key scenario (Option 1: Update the existing record)
                                var updateQuery = "UPDATE Reservations SET StartDate = @StartDate, EndDate = @EndDate, CustomerID = @CustomerID, HouseID = @HouseID WHERE ReservationID = @ReservationID";
                                using (var updateCommand = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    updateCommand.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                                    updateCommand.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                                    updateCommand.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                                    updateCommand.Parameters.AddWithValue("@CustomerID", reservation.CustomerID);
                                    updateCommand.Parameters.AddWithValue("@HouseID", reservation.HouseID);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Option 2: Insert a new record
                                var insertQuery = "INSERT INTO Reservations (ReservationID, StartDate, EndDate, CustomerID, HouseID) VALUES (@ReservationID, @StartDate, @EndDate, @CustomerID, @HouseID)";
                                using (var insertCommand = new SqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                                    insertCommand.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                                    insertCommand.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                                    insertCommand.Parameters.AddWithValue("@CustomerID", reservation.CustomerID);
                                    insertCommand.Parameters.AddWithValue("@HouseID", reservation.HouseID);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error occurred while adding or updating reservation: " + ex.Message, ex);
                    }
                }
            }
        }

        //GetAll reservation from db it is services
        public IEnumerable<Reservation> GetAllReservations()
        {
            var reservations = new List<Reservation>();

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Reservations";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservations.Add(new Reservation
                            {
                                ReservationID = reader.GetInt32(0),
                                StartDate = reader.GetDateTime(1),
                                EndDate = reader.GetDateTime(2),
                                CustomerID = reader.GetInt32(3),
                                HouseID = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }

            return reservations;
        }
    }
}

