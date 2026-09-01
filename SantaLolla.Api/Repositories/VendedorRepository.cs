using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Vendedores;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class VendedorRepository : IVendedorRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public VendedorRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResponse<VendedorResponse>> ListarAsync(
            VendedorFiltroRequest filtro)
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

            var offset =
                (filtro.Pagina - 1) *
                filtro.TamanhoPagina;

            const string sql = @"
                SELECT
                    COUNT(1)
                FROM dbo.SETA_VENDEDORES V WITH (NOLOCK)
                WHERE ISNULL(V.ATIVO, 1) = 1
                  AND (
                        V.DEMISSAO IS NULL
                        OR V.DEMISSAO > CAST(GETDATE() AS DATE)
                      )
                  AND (
                        @LastUpdateInicio IS NULL
                        OR V.LASTUPDATE_ORIGEM >= @LastUpdateInicio
                      )
                  AND (
                        @LastUpdateFim IS NULL
                        OR V.LASTUPDATE_ORIGEM <= @LastUpdateFim
                      )
                  AND (
                        @Rede IS NULL
                        OR V.REDE = @Rede
                      )
                  AND (
                        @CodigoLoja IS NULL
                        OR ISNULL(
                            NULLIF(V.EMPRESA_ACESSO, ''),
                            V.EMPRESA
                        ) = @CodigoLoja
                      )
                  AND (
                        @CodigoVendedor IS NULL
                        OR V.CODVENDEDOR = @CodigoVendedor
                      );

                SELECT
                    V.REDE AS Rede,

                    ISNULL(
                        NULLIF(V.EMPRESA_ACESSO, ''),
                        V.EMPRESA
                    ) AS CodigoLoja,

                    ISNULL(
                        NULLIF(V.APELIDO_EMPRESA_ACESSO, ''),
                        V.APELIDO_EMPRESA
                    ) AS NomeLoja,

                    L.MARCA AS Marca,

                    V.CODVENDEDOR AS CodigoVendedor,

                    ISNULL(
                        NULLIF(V.NOME_VENDEDOR, ''),
                        V.VENDEDOR
                    ) AS Nome,

                    NULLIF(V.CPFCNPJ_VENDEDOR, '') AS Cpf,

                    V.DESCRICAO_ATIVIDADE AS Cargo,

                    V.VENDEDOR AS Apelido,

                    V.ADMISSAO AS DataAdmissao,

                    V.DEMISSAO AS DataDemissao,

                    V.LASTUPDATE_ORIGEM AS DataAtualizacao,

                    CASE
                        WHEN ISNULL(V.ATIVO, 0) = 0
                            THEN 'Inativo'

                        WHEN V.DEMISSAO IS NOT NULL
                         AND V.DEMISSAO <= CAST(GETDATE() AS DATE)
                            THEN 'Inativo'

                        ELSE 'Ativo'
                    END AS Status

                FROM dbo.SETA_VENDEDORES V WITH (NOLOCK)

                LEFT JOIN dbo.SETA_LOJAS L WITH (NOLOCK)
                    ON L.REDE = V.REDE
                   AND L.CODIGO_EMPRESA =
                       ISNULL(
                           NULLIF(V.EMPRESA_ACESSO, ''),
                           V.EMPRESA
                       )
                   AND L.ATIVO = 1

                WHERE ISNULL(V.ATIVO, 1) = 1
                  AND (
                        V.DEMISSAO IS NULL
                        OR V.DEMISSAO > CAST(GETDATE() AS DATE)
                      )
                  AND (
                        @LastUpdateInicio IS NULL
                        OR V.LASTUPDATE_ORIGEM >= @LastUpdateInicio
                      )
                  AND (
                        @LastUpdateFim IS NULL
                        OR V.LASTUPDATE_ORIGEM <= @LastUpdateFim
                      )
                  AND (
                        @Rede IS NULL
                        OR V.REDE = @Rede
                      )
                  AND (
                        @CodigoLoja IS NULL
                        OR ISNULL(
                            NULLIF(V.EMPRESA_ACESSO, ''),
                            V.EMPRESA
                        ) = @CodigoLoja
                      )
                  AND (
                        @CodigoVendedor IS NULL
                        OR V.CODVENDEDOR = @CodigoVendedor
                      )

                ORDER BY
                    V.REDE,
                    ISNULL(
                        NULLIF(V.EMPRESA_ACESSO, ''),
                        V.EMPRESA
                    ),
                    V.CODVENDEDOR

                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            var parametros = new
            {
                filtro.LastUpdateInicio,
                filtro.LastUpdateFim,
                filtro.Rede,
                filtro.CodigoLoja,
                filtro.CodigoVendedor,
                Offset = offset,
                filtro.TamanhoPagina
            };

            using var connection =
                _connectionFactory.CreateConnection();

            using var resultado =
                await connection.QueryMultipleAsync(
                    sql,
                    parametros
                );

            var total =
                await resultado.ReadSingleAsync<int>();

            var vendedores = (
                await resultado.ReadAsync<VendedorResponse>()
            ).ToList();

            return PagedResponse<VendedorResponse>.Create(
                vendedores,
                total,
                filtro.Pagina,
                filtro.TamanhoPagina
            );
        }
    }
}