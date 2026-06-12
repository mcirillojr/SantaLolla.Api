using AlterVision.Api.Data;
using AlterVision.Api.Models.Vendedores;
using AlterVision.Api.Repositories.Interfaces;
using Dapper;

namespace AlterVision.Api.Repositories
{
    public class VendedorRepository : IVendedorRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public VendedorRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<VendedorResponse>> ListarAsync()
        {
            const string sql = @"
                SELECT
                    REDE AS Rede,

                    ISNULL(NULLIF(EMPRESA_ACESSO, ''), EMPRESA) AS CodigoLoja,

                    ISNULL(
                        NULLIF(APELIDO_EMPRESA_ACESSO, ''),
                        APELIDO_EMPRESA
                    ) AS NomeLoja,

                    CODVENDEDOR AS CodigoVendedor,

                    ISNULL(
                        NULLIF(NOME_VENDEDOR, ''),
                        VENDEDOR
                    ) AS Nome,

                    CPFCNPJ_VENDEDOR AS Cpf,

                    DESCRICAO_ATIVIDADE AS Cargo,

                    VENDEDOR AS Apelido,

                    ADMISSAO AS DataAdmissao,

                    DEMISSAO AS DataDemissao,

                    LASTUPDATE_ORIGEM AS DataAtualizacao,

                    CASE 
                        WHEN ATIVO = 1 THEN 'Ativo'
                        ELSE 'Inativo'
                    END AS Status

                FROM dbo.ALTERVISION_VENDEDORES
                WHERE ISNULL(ATIVO, 1) = 1
                ORDER BY REDE,
                         ISNULL(NULLIF(EMPRESA_ACESSO, ''), EMPRESA),
                         CODVENDEDOR;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<VendedorResponse>(sql);
        }
    }
}