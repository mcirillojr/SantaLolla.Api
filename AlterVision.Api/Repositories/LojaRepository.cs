using AlterVision.Api.Data;
using AlterVision.Api.Models.Lojas;
using AlterVision.Api.Repositories.Interfaces;
using Dapper;

namespace AlterVision.Api.Repositories
{
    public class LojaRepository : ILojaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public LojaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<LojaResponse>> ListarAsync()
        {
            const string sql = @"
      
               SELECT
                   REDE AS Rede,
                   CODIGO_EMPRESA AS CodigoLoja,
                   APELIDO AS NomeFantasia,
                   CNPJ AS Cnpj,
                   CEP AS Cep,
                   LASTUPDATE_ORIGEM AS DataAtualizacao
               FROM dbo.ALTERVISION_LOJAS
               WHERE ISNULL(ATIVO, 1) = 1
               ORDER BY REDE, CODIGO_EMPRESA
                    ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<LojaResponse>(sql);
        }
    }
}