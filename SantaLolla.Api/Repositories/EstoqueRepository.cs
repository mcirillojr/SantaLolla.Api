using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.Estoques;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EstoqueRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResponse<EstoqueResponse>> ListarAsync(
            EstoqueFiltroRequest filtro)
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

            var descricaoColecao =
                PrepararFiltroLike(filtro.DescricaoColecao);

            var descricaoLinha =
                PrepararFiltroLike(filtro.DescricaoLinha);

            const string sql = @"
                SELECT
                    COUNT(1)
                FROM dbo.SETA_ESTOQUE_ATUAL
                WHERE
                    (@Rede IS NULL OR REDE = @Rede)
                    AND (
                        @CodigoLoja IS NULL
                        OR CODIGO_EMPRESA = @CodigoLoja
                    )
                    AND (
                        @CodigoProduto IS NULL
                        OR CODIGO_PRODUTO = @CodigoProduto
                    )
                    AND (
                        @Referencia IS NULL
                        OR REFERENCIA = @Referencia
                    )
                    AND (
                        @Tamanho IS NULL
                        OR TAMANHO = @Tamanho
                    )
                    AND (
                        @Cor IS NULL
                        OR COR = @Cor
                    )
                    AND (
                        @DescricaoColecao IS NULL
                        OR DESCRICAO_COLECAO LIKE @DescricaoColecao
                    )
                    AND (
                        @DescricaoLinha IS NULL
                        OR DESCRICAO_LINHA LIKE @DescricaoLinha
                    )
                    AND (
                        @DataAtualizacaoInicio IS NULL
                        OR DATA_ATUALIZACAO >= @DataAtualizacaoInicio
                    )
                    AND (
                        @DataAtualizacaoFim IS NULL
                        OR DATA_ATUALIZACAO <= @DataAtualizacaoFim
                    );

                SELECT
                    REDE AS Rede,
                    CODIGO_EMPRESA AS CodigoEmpresa,
                    CNPJ AS Cnpj,
                    APELIDO_EMPRESA AS ApelidoEmpresa,
                    NOME_EMPRESA AS NomeEmpresa,
                    CODIGO_PRODUTO AS CodigoProduto,
                    DESCRICAO_PRODUTO AS DescricaoProduto,
                    TAMANHO AS Tamanho,
                    COR AS Cor,
                    GRADE AS Grade,
                    REFERENCIA AS Referencia,
                    MARCA AS Marca,
                    GRUPO AS Grupo,
                    DESCRICAO_COLECAO AS DescricaoColecao,
                    DESCRICAO_LINHA AS DescricaoLinha,
                    QUANTIDADE AS Quantidade,
                    CUSTO AS Custo,
                    PRECO AS Preco,
                    PRECO1 AS Preco1,
                    PRECO2 AS Preco2,
                    DATA_ESTOQUE AS DataEstoque,
                    DATA_CRIACAO AS DataCriacao,
                    DATA_ATUALIZACAO AS DataAtualizacao
                FROM dbo.SETA_ESTOQUE_ATUAL
                WHERE
                    (@Rede IS NULL OR REDE = @Rede)
                    AND (
                        @CodigoLoja IS NULL
                        OR CODIGO_EMPRESA = @CodigoLoja
                    )
                    AND (
                        @CodigoProduto IS NULL
                        OR CODIGO_PRODUTO = @CodigoProduto
                    )
                    AND (
                        @Referencia IS NULL
                        OR REFERENCIA = @Referencia
                    )
                    AND (
                        @Tamanho IS NULL
                        OR TAMANHO = @Tamanho
                    )
                    AND (
                        @Cor IS NULL
                        OR COR = @Cor
                    )
                    AND (
                        @DescricaoColecao IS NULL
                        OR DESCRICAO_COLECAO LIKE @DescricaoColecao
                    )
                    AND (
                        @DescricaoLinha IS NULL
                        OR DESCRICAO_LINHA LIKE @DescricaoLinha
                    )
                    AND (
                        @DataAtualizacaoInicio IS NULL
                        OR DATA_ATUALIZACAO >= @DataAtualizacaoInicio
                    )
                    AND (
                        @DataAtualizacaoFim IS NULL
                        OR DATA_ATUALIZACAO <= @DataAtualizacaoFim
                    )
                ORDER BY
                    REDE,
                    CODIGO_EMPRESA,
                    CODIGO_PRODUTO,
                    TAMANHO
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            var parametros = new
            {
                Rede = NormalizarTexto(filtro.Rede),
                CodigoLoja =
                    NormalizarTexto(filtro.CodigoLoja),
                CodigoProduto =
                    NormalizarTexto(filtro.CodigoProduto),
                Referencia =
                    NormalizarTexto(filtro.Referencia),
                Tamanho =
                    NormalizarTexto(filtro.Tamanho),
                Cor =
                    NormalizarTexto(filtro.Cor),
                DescricaoColecao = descricaoColecao,
                DescricaoLinha = descricaoLinha,
                filtro.DataAtualizacaoInicio,
                filtro.DataAtualizacaoFim,
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

            var estoques = (
                await resultado.ReadAsync<EstoqueResponse>()
            ).ToList();

            return PagedResponse<EstoqueResponse>.Create(
                estoques,
                total,
                filtro.Pagina,
                filtro.TamanhoPagina
            );
        }

        public async Task<
            PagedResponse<EstoqueTotalAgrupadoResponse>>
            ListarTotalAgrupadoAsync(
                EstoqueTotalAgrupadoFiltroRequest filtro)
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

            var nomeLoja =
                PrepararFiltroLike(filtro.NomeLoja);

            var referencia =
                PrepararFiltroLike(filtro.Referencia);

            var descricaoColecao =
                PrepararFiltroLike(filtro.DescricaoColecao);

            var descricaoLinha =
                PrepararFiltroLike(filtro.DescricaoLinha);

            const string sql = @"
                SELECT
                    COUNT(1)
                FROM
                (
                    SELECT
                        REDE,
                        CODIGO_PRODUTO,
                        DESCRICAO_PRODUTO,
                        REFERENCIA,
                        MARCA,
                        GRUPO,
                        DESCRICAO_COLECAO,
                        DESCRICAO_LINHA
                    FROM dbo.SETA_ESTOQUE_ATUAL
                    WHERE
                        (
                            @NomeLoja IS NULL
                            OR APELIDO_EMPRESA LIKE @NomeLoja
                        )
                        AND (
                            @Referencia IS NULL
                            OR REFERENCIA LIKE @Referencia
                        )
                        AND (
                            @DescricaoColecao IS NULL
                            OR DESCRICAO_COLECAO LIKE @DescricaoColecao
                        )
                        AND (
                            @DescricaoLinha IS NULL
                            OR DESCRICAO_LINHA LIKE @DescricaoLinha
                        )
                    GROUP BY
                        REDE,
                        CODIGO_PRODUTO,
                        DESCRICAO_PRODUTO,
                        REFERENCIA,
                        MARCA,
                        GRUPO,
                        DESCRICAO_COLECAO,
                        DESCRICAO_LINHA
                    HAVING SUM(QUANTIDADE) <> 0
                ) AS TOTAL_AGRUPADO;

                SELECT
                    REDE AS Rede,
                    CODIGO_PRODUTO AS CodigoProduto,
                    DESCRICAO_PRODUTO AS DescricaoProduto,
                    REFERENCIA AS Referencia,
                    MARCA AS Marca,
                    GRUPO AS Grupo,
                    DESCRICAO_COLECAO AS DescricaoColecao,
                    DESCRICAO_LINHA AS DescricaoLinha,
                    SUM(QUANTIDADE) AS QuantidadeTotal,
                    AVG(CUSTO) AS Custo,
                    AVG(PRECO) AS Preco,
                    AVG(PRECO1) AS Preco1,
                    AVG(PRECO2) AS Preco2
                FROM dbo.SETA_ESTOQUE_ATUAL
                WHERE
                    (
                        @NomeLoja IS NULL
                        OR APELIDO_EMPRESA LIKE @NomeLoja
                    )
                    AND (
                        @Referencia IS NULL
                        OR REFERENCIA LIKE @Referencia
                    )
                    AND (
                        @DescricaoColecao IS NULL
                        OR DESCRICAO_COLECAO LIKE @DescricaoColecao
                    )
                    AND (
                        @DescricaoLinha IS NULL
                        OR DESCRICAO_LINHA LIKE @DescricaoLinha
                    )
                GROUP BY
                    REDE,
                    CODIGO_PRODUTO,
                    DESCRICAO_PRODUTO,
                    REFERENCIA,
                    MARCA,
                    GRUPO,
                    DESCRICAO_COLECAO,
                    DESCRICAO_LINHA
                HAVING SUM(QUANTIDADE) <> 0
                ORDER BY
                    REDE,
                    REFERENCIA,
                    CODIGO_PRODUTO
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            var parametros = new
            {
                NomeLoja = nomeLoja,
                Referencia = referencia,
                DescricaoColecao = descricaoColecao,
                DescricaoLinha = descricaoLinha,
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

            var estoques = (
                await resultado
                    .ReadAsync<EstoqueTotalAgrupadoResponse>()
            ).ToList();

            return PagedResponse<
                EstoqueTotalAgrupadoResponse>.Create(
                    estoques,
                    total,
                    filtro.Pagina,
                    filtro.TamanhoPagina
                );
        }

        private static string? NormalizarTexto(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static string? PrepararFiltroLike(
            string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            valor = valor.Trim();

            if (valor.Contains('%'))
            {
                return valor;
            }

            return $"%{valor}%";
        }
    }
}