using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Produtos;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ProdutoRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResponse<ProdutoResponse>> ListarAsync(
            ProdutoFiltroRequest filtro)
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

            var referencia =
                PrepararFiltroLike(filtro.Referencia);

            var descricaoProduto =
                PrepararFiltroLike(filtro.DescricaoProduto);

            var descricaoMarca =
                PrepararFiltroLike(filtro.DescricaoMarca);

            const string sql = @"
                ------------------------------------------------------------
                -- TOTAL DE REGISTROS
                ------------------------------------------------------------
                SELECT
                    COUNT(1)
                FROM dbo.SETA_PRODUTOS P WITH (NOLOCK)
                WHERE
                    (
                        @Rede IS NULL
                        OR P.REDE = @Rede
                    )
                    AND (
                        @CodigoProduto IS NULL
                        OR P.CODIGO_PRODUTO = @CodigoProduto
                    )
                    AND (
                        @CodigoLinx IS NULL
                        OR P.CODIGO_LINX = @CodigoLinx
                    )
                    AND (
                        @CodigoPai IS NULL
                        OR P.CODIGO_PAI = @CodigoPai
                    )
                    AND (
                        @Referencia IS NULL
                        OR P.REFERENCIA LIKE @Referencia
                    )
                    AND (
                        @DescricaoProduto IS NULL
                        OR P.DESCRICAO_PRODUTO LIKE @DescricaoProduto
                    )
                    AND (
                        @Marca IS NULL
                        OR P.MARCA = @Marca
                    )
                    AND (
                        @DescricaoMarca IS NULL
                        OR P.DESCRICAO_MARCA LIKE @DescricaoMarca
                    )
                    AND (
                        @Fornecedor IS NULL
                        OR P.FORNECEDOR = @Fornecedor
                    )
                    AND (
                        @Departamento IS NULL
                        OR P.DEPARTAMENTO = @Departamento
                    )
                    AND (
                        @Grupo IS NULL
                        OR P.GRUPO = @Grupo
                    )
                    AND (
                        @CodigoColecao IS NULL
                        OR P.CODIGO_COLECAO = @CodigoColecao
                    )
                    AND (
                        @CodigoLinha IS NULL
                        OR P.CODIGO_LINHA = @CodigoLinha
                    )
                    AND (
                        @Desativar IS NULL
                        OR P.DESATIVAR = @Desativar
                    )
                    AND (
                        @Ecommerce IS NULL
                        OR P.ECOMMERCE = @Ecommerce
                    )
                    AND (
                        @LastUpdateInicio IS NULL
                        OR P.LASTUPDATE_ORIGEM >= @LastUpdateInicio
                    )
                    AND (
                        @LastUpdateFim IS NULL
                        OR P.LASTUPDATE_ORIGEM <= @LastUpdateFim
                    );

                ------------------------------------------------------------
                -- DADOS DOS PRODUTOS
                ------------------------------------------------------------
                SELECT
                    P.REDE AS Rede,
                    P.CODIGO_PRODUTO AS CodigoProduto,
                    P.CODIGO_LINX AS CodigoLinx,

                    P.CODIGO_PAI AS CodigoPai,

                    P.DESCRICAO_PRODUTO AS DescricaoProduto,
                    P.COR AS Cor,
                    P.REFERENCIA AS Referencia,

                    P.MARCA AS Marca,
                    P.DESCRICAO_MARCA AS DescricaoMarca,

                    P.URL_FOTO_1 AS UrlFoto1,
                    P.URL_FOTO_2 AS UrlFoto2,
                    P.URL_FOTO_3 AS UrlFoto3,

                    P.FORNECEDOR AS Fornecedor,

                    P.DEPARTAMENTO AS Departamento,
                    P.DESCRICAO_DEPARTAMENTO AS DescricaoDepartamento,

                    P.GRUPO AS Grupo,
                    P.DESCRICAO_GRUPO AS DescricaoGrupo,

                    P.GRADE AS Grade,
                    P.DESCRICAO_GRADE AS DescricaoGrade,

                    P.CODIGO_COLECAO AS CodigoColecao,
                    P.DESCRICAO_COLECAO AS DescricaoColecao,

                    P.CODIGO_LINHA AS CodigoLinha,
                    P.DESCRICAO_LINHA AS DescricaoLinha,

                    P.SUBGRUPO AS Subgrupo,

                    P.DATA_CADASTRO_ORIGEM AS DataCadastro,
                    P.LASTUPDATE_ORIGEM AS DataAtualizacao,

                    P.DESATIVAR AS Desativar,

                    P.CUSTO AS Custo,
                    P.CUSTO_AQUISICAO AS CustoAquisicao,

                    P.VINCULADO AS Vinculado,

                    P.PRECO AS Preco,
                    P.PRECO1 AS Preco1,
                    P.PRECO2 AS Preco2,

                    P.QUANTIDADE AS Quantidade,

                    P.PROMOCOES AS Promocoes,
                    P.ECOMMERCE AS Ecommerce,

                    P.ULTIMA_COMPRA AS UltimaCompra

                FROM dbo.SETA_PRODUTOS P WITH (NOLOCK)

                WHERE
                    (
                        @Rede IS NULL
                        OR P.REDE = @Rede
                    )
                    AND (
                        @CodigoProduto IS NULL
                        OR P.CODIGO_PRODUTO = @CodigoProduto
                    )
                    AND (
                        @CodigoLinx IS NULL
                        OR P.CODIGO_LINX = @CodigoLinx
                    )
                    AND (
                        @CodigoPai IS NULL
                        OR P.CODIGO_PAI = @CodigoPai
                    )
                    AND (
                        @Referencia IS NULL
                        OR P.REFERENCIA LIKE @Referencia
                    )
                    AND (
                        @DescricaoProduto IS NULL
                        OR P.DESCRICAO_PRODUTO LIKE @DescricaoProduto
                    )
                    AND (
                        @Marca IS NULL
                        OR P.MARCA = @Marca
                    )
                    AND (
                        @DescricaoMarca IS NULL
                        OR P.DESCRICAO_MARCA LIKE @DescricaoMarca
                    )
                    AND (
                        @Fornecedor IS NULL
                        OR P.FORNECEDOR = @Fornecedor
                    )
                    AND (
                        @Departamento IS NULL
                        OR P.DEPARTAMENTO = @Departamento
                    )
                    AND (
                        @Grupo IS NULL
                        OR P.GRUPO = @Grupo
                    )
                    AND (
                        @CodigoColecao IS NULL
                        OR P.CODIGO_COLECAO = @CodigoColecao
                    )
                    AND (
                        @CodigoLinha IS NULL
                        OR P.CODIGO_LINHA = @CodigoLinha
                    )
                    AND (
                        @Desativar IS NULL
                        OR P.DESATIVAR = @Desativar
                    )
                    AND (
                        @Ecommerce IS NULL
                        OR P.ECOMMERCE = @Ecommerce
                    )
                    AND (
                        @LastUpdateInicio IS NULL
                        OR P.LASTUPDATE_ORIGEM >= @LastUpdateInicio
                    )
                    AND (
                        @LastUpdateFim IS NULL
                        OR P.LASTUPDATE_ORIGEM <= @LastUpdateFim
                    )

                ORDER BY
                    P.LASTUPDATE_ORIGEM DESC,
                    P.REDE,
                    P.CODIGO_PRODUTO

                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            var parametros = new
            {
                Rede =
                    NormalizarTexto(filtro.Rede),

                CodigoProduto =
                    NormalizarTexto(filtro.CodigoProduto),

                CodigoLinx =
                    NormalizarTexto(filtro.CodigoLinx),

                CodigoPai =
                    NormalizarTexto(filtro.CodigoPai),

                Referencia = referencia,
                DescricaoProduto = descricaoProduto,

                Marca =
                    NormalizarTexto(filtro.Marca),

                DescricaoMarca = descricaoMarca,

                Fornecedor =
                    NormalizarTexto(filtro.Fornecedor),

                Departamento =
                    NormalizarTexto(filtro.Departamento),

                Grupo =
                    NormalizarTexto(filtro.Grupo),

                CodigoColecao =
                    NormalizarTexto(filtro.CodigoColecao),

                CodigoLinha =
                    NormalizarTexto(filtro.CodigoLinha),

                filtro.Desativar,
                filtro.Ecommerce,

                filtro.LastUpdateInicio,
                filtro.LastUpdateFim,

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

            var produtos = (
                await resultado.ReadAsync<ProdutoResponse>()
            ).ToList();

            return PagedResponse<ProdutoResponse>.Create(
                produtos,
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