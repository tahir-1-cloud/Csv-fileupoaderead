using System.Collections.Generic;
using System.Data.SqlClient;
using VacationPark.DBConnection;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly DatabaseContext _context;

        public FacilityRepository(DatabaseContext context)
        {
            _context = context;
        }

        //add facility function
        public void AddFacility(Facility facility)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if the FacilityID already exists
                        var checkQuery = "SELECT COUNT(1) FROM Facilities WHERE FacilityID = @FacilityID";
                        using (var checkCommand = new SqlCommand(checkQuery, connection, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@FacilityID", facility.FacilityID);
                            var exists = (int)checkCommand.ExecuteScalar() > 0;

                            if (exists)
                            {
                                // Handle duplicate key scenario
                                // Option 1: Update the existing record
                                var updateQuery = "UPDATE Facilities SET Description = @Description WHERE FacilityID = @FacilityID";
                                using (var updateCommand = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    updateCommand.Parameters.AddWithValue("@FacilityID", facility.FacilityID);
                                    updateCommand.Parameters.AddWithValue("@Description", facility.Description);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Option 2: Insert a new record
                                var insertQuery = "INSERT INTO Facilities (FacilityID, Description) VALUES (@FacilityID, @Description)";
                                using (var insertCommand = new SqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@FacilityID", facility.FacilityID);
                                    insertCommand.Parameters.AddWithValue("@Description", facility.Description);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error occurred while adding facility: " + ex.Message, ex);
                    }
                }
            }
        }


        public IEnumerable<Facility> GetAllFacilities()
        {
            var facilities = new List<Facility>();

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Facilities";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            facilities.Add(new Facility
                            {
                                FacilityID = reader.GetInt32(0),
                                Description = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return facilities;
        }
    }
}
