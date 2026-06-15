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

        public async Task<IEnumerable<VendedorResponse>> ListarAsync(VendedorFiltroRequest filtro)
        {
            if (filtro.Pagina <= 0)
            {
                filtro.Pagina = 1;
            }

            if (filtro.TamanhoPagina <= 0)
            {
                filtro.TamanhoPagina = 500;
            }

            if (filtro.TamanhoPagina > 5000)
            {
                filtro.TamanhoPagina = 5000;
            }

            var offset = (filtro.Pagina - 1) * filtro.TamanhoPagina;

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

                    NULLIF(CPFCNPJ_VENDEDOR, '') AS Cpf,

                    DESCRICAO_ATIVIDADE AS Cargo,

                    VENDEDOR AS Apelido,

                    ADMISSAO AS DataAdmissao,

                    DEMISSAO AS DataDemissao,

                    LASTUPDATE_ORIGEM AS DataAtualizacao,

                    CASE 
                        WHEN ISNULL(ATIVO, 0) = 0 THEN 'Inativo'
                        WHEN DEMISSAO IS NOT NULL AND DEMISSAO <= CAST(GETDATE() AS DATE) THEN 'Inativo'
                        ELSE 'Ativo'
                    END AS Status

                FROM dbo.ALTERVISION_VENDEDORES
                WHERE ISNULL(ATIVO, 1) = 1
                  AND (DEMISSAO IS NULL OR DEMISSAO > CAST(GETDATE() AS DATE))
                  AND (@LastUpdateInicio IS NULL OR LASTUPDATE_ORIGEM >= @LastUpdateInicio)
                  AND (@LastUpdateFim IS NULL OR LASTUPDATE_ORIGEM <= @LastUpdateFim)
                  AND (@Rede IS NULL OR REDE = @Rede)
                  AND (@CodigoLoja IS NULL OR ISNULL(NULLIF(EMPRESA_ACESSO, ''), EMPRESA) = @CodigoLoja)
                  AND (@CodigoVendedor IS NULL OR CODVENDEDOR = @CodigoVendedor)
                ORDER BY REDE,
                         ISNULL(NULLIF(EMPRESA_ACESSO, ''), EMPRESA),
                         CODVENDEDOR
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<VendedorResponse>(
                sql,
                new
                {
                    filtro.LastUpdateInicio,
                    filtro.LastUpdateFim,
                    filtro.Rede,
                    filtro.CodigoLoja,
                    filtro.CodigoVendedor,
                    Offset = offset,
                    filtro.TamanhoPagina
                }
            );
        }
    }
}