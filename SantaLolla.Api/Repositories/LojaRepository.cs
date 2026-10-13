using Dapper;
using Microsoft.AspNetCore.Http;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.Lojas;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class LojaRepository : ILojaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LojaRepository(
            SqlConnectionFactory connectionFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _connectionFactory = connectionFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResponse<LojaResponse>> ListarAsync(
            LojaFiltroRequest filtro)
        {
            if (filtro.Pagina <= 0)
            {
                filtro.Pagina = 1;
            }

            if (filtro.TamanhoPagina <= 0)
            {
                filtro.TamanhoPagina = 50;
            }

            if (filtro.TamanhoPagina > 5000)
            {
                filtro.TamanhoPagina = 5000;
            }

            // -----------------------------------------------------
            // ACESSO ÀS REDES DO TERCEIRO
            // -----------------------------------------------------
            var httpContext =
                _httpContextAccessor.HttpContext;

            var acessoTodasRedes =
                httpContext?.Items["AcessoTodasRedes"] is bool acesso
                    && acesso;

            var redesPermitidas =
                httpContext?.Items["RedesPermitidas"] as List<string>
                ?? new List<string>();

            // Se o terceiro for restrito e não possuir
            // nenhuma rede cadastrada, não retorna lojas.
            if (!acessoTodasRedes &&
                redesPermitidas.Count == 0)
            {
                return PagedResponse<LojaResponse>.Create(
                    new List<LojaResponse>(),
                    0,
                    filtro.Pagina,
                    filtro.TamanhoPagina
                );
            }

            var offset =
                (filtro.Pagina - 1) *
                filtro.TamanhoPagina;

            const string sql = @"
                ------------------------------------------------------------
                -- TOTAL DE REGISTROS
                ------------------------------------------------------------
                SELECT
                    COUNT(1)
                FROM dbo.SETA_LOJAS WITH (NOLOCK)
                WHERE ISNULL(ATIVO, 1) = 1

                  AND (
                        @AcessoTodasRedes = 1
                        OR REDE IN @RedesPermitidas
                      )

                  AND (
                        @LastUpdateInicio IS NULL
                        OR LASTUPDATE_ORIGEM >= @LastUpdateInicio
                      )

                  AND (
                        @LastUpdateFim IS NULL
                        OR LASTUPDATE_ORIGEM <= @LastUpdateFim
                      )

                  AND (
                        @Rede IS NULL
                        OR REDE = @Rede
                      )

                  AND (
                        @CodigoLoja IS NULL
                        OR CODIGO_EMPRESA = @CodigoLoja
                      );


                ------------------------------------------------------------
                -- DADOS DAS LOJAS
                ------------------------------------------------------------
                SELECT
                    REDE AS Rede,
                    CODIGO_EMPRESA AS CodigoLoja,
                    APELIDO AS NomeFantasia,
                    CNPJ AS Cnpj,
                    CEP AS Cep,
                    MARCA AS Marca,
                    LASTUPDATE_ORIGEM AS DataAtualizacao

                FROM dbo.SETA_LOJAS WITH (NOLOCK)

                WHERE ISNULL(ATIVO, 1) = 1

                  AND (
                        @AcessoTodasRedes = 1
                        OR REDE IN @RedesPermitidas
                      )

                  AND (
                        @LastUpdateInicio IS NULL
                        OR LASTUPDATE_ORIGEM >= @LastUpdateInicio
                      )

                  AND (
                        @LastUpdateFim IS NULL
                        OR LASTUPDATE_ORIGEM <= @LastUpdateFim
                      )

                  AND (
                        @Rede IS NULL
                        OR REDE = @Rede
                      )

                  AND (
                        @CodigoLoja IS NULL
                        OR CODIGO_EMPRESA = @CodigoLoja
                      )

                ORDER BY
                    REDE,
                    CODIGO_EMPRESA

                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            using var connection =
                _connectionFactory.CreateConnection();

            using var resultado =
                await connection.QueryMultipleAsync(
                    sql,
                    new
                    {
                        AcessoTodasRedes =
                            acessoTodasRedes,

                        RedesPermitidas =
                            redesPermitidas,

                        filtro.LastUpdateInicio,
                        filtro.LastUpdateFim,
                        filtro.Rede,
                        filtro.CodigoLoja,

                        Offset =
                            offset,

                        filtro.TamanhoPagina
                    }
                );

            var total =
                await resultado.ReadSingleAsync<int>();

            var lojas = (
                await resultado.ReadAsync<LojaResponse>()
            ).ToList();

            return PagedResponse<LojaResponse>.Create(
                lojas,
                total,
                filtro.Pagina,
                filtro.TamanhoPagina
            );
        }
    }
}