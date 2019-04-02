using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Hisab.Dapper
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private string _connectionString;



        public DbConnectionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            try
            {
                var sqlConnection = new SqlConnection(_connectionString);

              
                await sqlConnection.OpenAsync();
                return sqlConnection;

            }
            catch
            {
                throw;
            }
        }

       

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
