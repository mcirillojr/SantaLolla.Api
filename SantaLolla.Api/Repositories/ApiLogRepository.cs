using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.Logs;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class ApiLogRepository : IApiLogRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ApiLogRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task GravarAsync(ApiLogAcesso log)
        {
            const string sql = @"
                INSERT INTO dbo.SETA_API_LOG_ACESSOS (
                    ID_TERCEIRO,
                    CLIENT_ID,
                    NOME_TERCEIRO,
                    METODO_HTTP,
                    ENDPOINT,
                    QUERY_STRING,
                    STATUS_CODE,
                    TEMPO_RESPOSTA_MS,
                    IP_ORIGEM,
                    USER_AGENT,
                    MENSAGEM_ERRO,
                    DATA_REQUISICAO
                )
                VALUES (
                    @IdTerceiro,
                    @ClientId,
                    @NomeTerceiro,
                    @MetodoHttp,
                    @Endpoint,
                    @QueryString,
                    @StatusCode,
                    @TempoRespostaMs,
                    @IpOrigem,
                    @UserAgent,
                    @MensagemErro,
                    GETDATE()
                );
            ";

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                log.IdTerceiro,
                ClientId = Limitar(log.ClientId, 100),
                NomeTerceiro = Limitar(log.NomeTerceiro, 100),
                MetodoHttp = Limitar(log.MetodoHttp, 10) ?? string.Empty,
                Endpoint = Limitar(log.Endpoint, 300) ?? string.Empty,
                log.QueryString,
                log.StatusCode,
                log.TempoRespostaMs,
                IpOrigem = Limitar(log.IpOrigem, 100),
                UserAgent = Limitar(log.UserAgent, 500),
                log.MensagemErro
            });
        }

        private static string? Limitar(string? valor, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(valor))
            {
                return valor;
            }

            return valor.Length <= tamanhoMaximo
                ? valor
                : valor.Substring(0, tamanhoMaximo);
        }
    }
}