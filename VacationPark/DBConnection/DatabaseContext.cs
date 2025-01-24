using System.Data.SqlClient;

namespace VacationPark.DBConnection
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
