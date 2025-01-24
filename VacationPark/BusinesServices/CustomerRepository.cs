using System.Collections.Generic;
using System.Data.SqlClient;
using VacationPark.DBConnection;
using VacationPark.Interface;
using VacationPark.Models;

namespace VacationPark.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;

        public CustomerRepository(DatabaseContext context)
        {
            _context = context;
        }
        //Add customer services 
        public void AddCustomer(Customer customer)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if the CustomerID already exists
                        var checkQuery = "SELECT COUNT(1) FROM Customers WHERE CustomerID = @CustomerID";
                        using (var checkCommand = new SqlCommand(checkQuery, connection, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                            var exists = (int)checkCommand.ExecuteScalar() > 0;

                            if (exists)
                            {
                                // Update the existing record
                                var updateQuery = "UPDATE Customers SET Name = @Name, Address = @Address WHERE CustomerID = @CustomerID";
                                using (var updateCommand = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    updateCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                                    updateCommand.Parameters.AddWithValue("@Name", customer.Name);
                                    updateCommand.Parameters.AddWithValue("@Address", customer.Address);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert a new record
                                var insertQuery = "INSERT INTO Customers (CustomerID, Name, Address) VALUES (@CustomerID, @Name, @Address)";
                                using (var insertCommand = new SqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCommand.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                                    insertCommand.Parameters.AddWithValue("@Name", customer.Name);
                                    insertCommand.Parameters.AddWithValue("@Address", customer.Address);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        transaction.Rollback();
                        throw new Exception("Error occurred while adding customer: " + ex.Message, ex);
                    }
                }
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT * FROM Customers", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Address = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return customers;
        }

    }
}
