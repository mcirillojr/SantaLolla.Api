using Microsoft.Data.SqlClient;
using System.Data;

namespace AlterVision.Api.Data
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
            var connectionString = _configuration.GetConnectionString("AlterVisionDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionString 'AlterVisionDb' não configurada.");
            }

            return new SqlConnection(connectionString);
        }
    }
}