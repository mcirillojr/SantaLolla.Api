using SantaLolla.Api.Data;
using SantaLolla.Api.Models.Auth;
using SantaLolla.Api.Repositories.Interfaces;
using Dapper;

namespace SantaLolla.Api.Repositories
{
    public class TerceiroRepository : ITerceiroRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public TerceiroRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TerceiroApi?> ObterPorClientIdAsync(string clientId)
        {
            const string sql = @"
                SELECT
                    ID_TERCEIRO AS IdTerceiro,
                    NOME AS Nome,
                    CLIENT_ID AS ClientId,
                    CLIENT_SECRET_HASH AS ClientSecretHash,
                    ATIVO AS Ativo
                FROM dbo.ALTERVISION_API_TERCEIROS
                WHERE CLIENT_ID = @ClientId;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<TerceiroApi>(
                sql,
                new { ClientId = clientId }
            );
        }

        public async Task AtualizarUltimoAcessoAsync(long idTerceiro)
        {
            const string sql = @"
                UPDATE dbo.ALTERVISION_API_TERCEIROS
                   SET DATA_ULTIMO_ACESSO = GETDATE()
                 WHERE ID_TERCEIRO = @IdTerceiro;
            ";

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                sql,
                new { IdTerceiro = idTerceiro }
            );
        }
    }
}
