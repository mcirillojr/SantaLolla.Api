using SantaLolla.Api.Data;
using SantaLolla.Api.Models.Estoques;
using SantaLolla.Api.Repositories.Interfaces;
using Dapper;

namespace SantaLolla.Api.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EstoqueRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<EstoqueResponse>> ListarAsync(EstoqueFiltroRequest filtro)
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
                    CODIGO_EMPRESA AS CodigoLoja,
                    CNPJ AS Cnpj,
                    APELIDO_EMPRESA AS NomeLoja,
                    CODIGO_PRODUTO AS CodigoProduto,
                    DESCRICAO_PRODUTO AS DescricaoProduto,
                    TAMANHO AS Tamanho,
                    COR AS Cor,
                    GRADE AS Grade,
                    REFERENCIA AS Referencia,
                    MARCA AS Marca,
                    GRUPO AS Grupo,
                    QUANTIDADE AS Quantidade,
                    CUSTO AS Custo,
                    PRECO AS Preco,
                    PRECO1 AS Preco1,
                    PRECO2 AS Preco2,
                    DATA_ESTOQUE AS DataEstoque,
                    DATA_ATUALIZACAO AS DataAtualizacao
                FROM dbo.ALTERVISION_ESTOQUE_ATUAL
                WHERE
                    (@Rede IS NULL OR REDE = @Rede)
                    AND (@CodigoLoja IS NULL OR CODIGO_EMPRESA = @CodigoLoja)
                    AND (@CodigoProduto IS NULL OR CODIGO_PRODUTO = @CodigoProduto)
                    AND (@Referencia IS NULL OR REFERENCIA = @Referencia)
                    AND (@Tamanho IS NULL OR TAMANHO = @Tamanho)
                    AND (@Cor IS NULL OR COR = @Cor)
                    AND (@DataAtualizacaoInicio IS NULL OR DATA_ATUALIZACAO >= @DataAtualizacaoInicio)
                    AND (@DataAtualizacaoFim IS NULL OR DATA_ATUALIZACAO <= @DataAtualizacaoFim)
                ORDER BY
                    REDE,
                    CODIGO_EMPRESA,
                    CODIGO_PRODUTO,
                    TAMANHO
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<EstoqueResponse>(
                sql,
                new
                {
                    filtro.Rede,
                    filtro.CodigoLoja,
                    filtro.CodigoProduto,
                    filtro.Referencia,
                    filtro.Tamanho,
                    filtro.Cor,
                    filtro.DataAtualizacaoInicio,
                    filtro.DataAtualizacaoFim,
                    Offset = offset,
                    filtro.TamanhoPagina
                }
            );
        }

        public async Task<IEnumerable<EstoqueTotalAgrupadoResponse>> ListarTotalAgrupadoAsync(
            EstoqueTotalAgrupadoFiltroRequest filtro
        )
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

            var nomeLoja = string.IsNullOrWhiteSpace(filtro.NomeLoja)
                ? null
                : filtro.NomeLoja.Trim();

            var referencia = string.IsNullOrWhiteSpace(filtro.Referencia)
                ? null
                : filtro.Referencia.Trim();

            const string sql = @"
                SELECT
                    APELIDO_EMPRESA AS NomeLoja,
                    REFERENCIA AS Referencia,
                    DESCRICAO_PRODUTO AS DescricaoProduto,
                    CODIGO_PRODUTO AS CodigoProduto,
                    TAMANHO AS Tamanho,
                    MARCA AS Marca,
                    SUM(QUANTIDADE) AS QuantidadeTotal
                FROM dbo.ALTERVISION_ESTOQUE_ATUAL
                WHERE
                    (@NomeLoja IS NULL OR APELIDO_EMPRESA LIKE @NomeLoja)
                    AND (@Referencia IS NULL OR REFERENCIA LIKE @Referencia)
                GROUP BY
                    APELIDO_EMPRESA,
                    REFERENCIA,
                    DESCRICAO_PRODUTO,
                    CODIGO_PRODUTO,
                    TAMANHO,
                    MARCA
                HAVING SUM(QUANTIDADE) <> 0
                ORDER BY
                    APELIDO_EMPRESA,
                    REFERENCIA,
                    CODIGO_PRODUTO,
                    TAMANHO
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<EstoqueTotalAgrupadoResponse>(
                sql,
                new
                {
                    NomeLoja = nomeLoja,
                    Referencia = referencia,
                    Offset = offset,
                    filtro.TamanhoPagina
                }
            );
        }
    }
}
