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
                FROM dbo.SETA_API_TERCEIROS (NOLOCK)
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
                UPDATE dbo.SETA_API_TERCEIROS
                   SET DATA_ULTIMO_ACESSO = GETDATE()
                 WHERE ID_TERCEIRO = @IdTerceiro;
            ";

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                sql,
                new { IdTerceiro = idTerceiro }
            );
        }
        public async Task<bool> TerceiroTemPermissaoAsync(
            long idTerceiro,
            string endpoint,
            string metodoHttp
)
        {
            const string sql = @"
        SELECT TOP 1 1
        FROM dbo.SETA_API_TERCEIROS_PERMISSOES
        WHERE ID_TERCEIRO = @IdTerceiro
          AND PERMITIDO = 1
          AND (
                (ENDPOINT = '*' AND METODO_HTTP = '*')
             OR (ENDPOINT = '*' AND METODO_HTTP = @MetodoHttp)
             OR (ENDPOINT = @Endpoint AND METODO_HTTP = '*')
             OR (ENDPOINT = @Endpoint AND METODO_HTTP = @MetodoHttp)
          );
    ";

            using var connection = _connectionFactory.CreateConnection();

            var permitido = await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new
                {
                    IdTerceiro = idTerceiro,
                    Endpoint = endpoint,
                    MetodoHttp = metodoHttp.ToUpper()
                }
            );

            return permitido.HasValue;
        }
        public async Task<TerceiroAcessoRede?> ObterAcessoRedesAsync(
        long idTerceiro)
        {
            const string sql = @"
        SELECT
            ID_TERCEIRO AS IdTerceiro,
            ACESSO_TODAS_REDES AS AcessoTodasRedes
        FROM dbo.SETA_API_TERCEIROS WITH (NOLOCK)
        WHERE ID_TERCEIRO = @IdTerceiro
          AND ATIVO = 1;

        SELECT
            REDE
        FROM dbo.SETA_API_TERCEIROS_REDES WITH (NOLOCK)
        WHERE ID_TERCEIRO = @IdTerceiro
          AND ATIVO = 1
        ORDER BY REDE;
    ";

            using var connection =
                _connectionFactory.CreateConnection();

            using var resultado =
                await connection.QueryMultipleAsync(
                    sql,
                    new
                    {
                        IdTerceiro = idTerceiro
                    }
                );

            var terceiro =
                await resultado.ReadFirstOrDefaultAsync<TerceiroAcessoRedeDbResult>();

            if (terceiro == null)
            {
                return null;
            }

            var redes =
                (await resultado.ReadAsync<string>())
                .ToList();

            return new TerceiroAcessoRede
            {
                AcessoTodasRedes =
                    terceiro.AcessoTodasRedes,

                RedesPermitidas =
                    redes
            };
        }
        private sealed class TerceiroAcessoRedeDbResult
        {
            public long IdTerceiro { get; set; }

            public bool AcessoTodasRedes { get; set; }
        }
    }

 }

