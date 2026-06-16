using Microsoft.Data.SqlClient;
using System.Data;

namespace SantaLolla.Api.Data
{
    public class SqlConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("SantaLollaDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionString 'SantaLollaDb' não configurada.");
            }

            return new SqlConnection(connectionString);
        }
    }
}
