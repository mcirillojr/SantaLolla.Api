using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.VendasProdutos;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class VendaProdutoRepository : IVendaProdutoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public VendaProdutoRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResponse<VendaProdutoResponse>> ListarAsync(
            VendaProdutoFiltroRequest filtro
        )
        {
            filtro.Pagina =
                filtro.Pagina <= 0
                    ? 1
                    : filtro.Pagina;

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

            var apelido =
                PrepararFiltroLike(filtro.Apelido);

            var cliente =
                PrepararFiltroLike(filtro.Cliente);

            var referencia =
                PrepararFiltroLike(filtro.Referencia);

            const string sql = @"
                SELECT
                    COUNT(1)
                FROM
                (
                    SELECT
                        REDE,
                        CODIGO_VENDA
                    FROM dbo.SETA_VENDAS_PRODUTOS
                    WHERE 1 = 1
                      AND (
                            @Rede IS NULL
                            OR REDE = @Rede
                          )
                      AND (
                            @Venda IS NULL
                            OR CODIGO_VENDA = @Venda
                          )
                      AND (
                            @CodVendedor IS NULL
                            OR CODVENDEDOR = @CodVendedor
                          )
                      AND (
                            @Referencia IS NULL
                            OR REFERENCIA LIKE @Referencia
                          )
                      AND (
                            @Apelido IS NULL
                            OR APELIDO LIKE @Apelido
                          )
                      AND (
                            @Cliente IS NULL
                            OR CLIENTE LIKE @Cliente
                          )
                      AND (
                            @DataVendaInicio IS NULL
                            OR DATA_VENDA >= @DataVendaInicio
                          )
                      AND (
                            @DataVendaFim IS NULL
                            OR DATA_VENDA <= @DataVendaFim
                          )
                      AND (
                            @LastUpdateInicio IS NULL
                            OR LASTUPDATE_ORIGEM >= @LastUpdateInicio
                          )
                      AND (
                            @LastUpdateFim IS NULL
                            OR LASTUPDATE_ORIGEM <= @LastUpdateFim
                          )
                    GROUP BY
                        REDE,
                        CODIGO_VENDA
                ) AS TOTAL_VENDAS;

                ;WITH VendasFiltradas AS
                (
                    SELECT
                        REDE,
                        CODIGO_VENDA,
                        MAX(DATA_VENDA) AS DATA_VENDA,
                        MAX(LASTUPDATE_ORIGEM) AS LASTUPDATE_ORIGEM
                    FROM dbo.SETA_VENDAS_PRODUTOS
                    WHERE 1 = 1
                      AND (
                            @Rede IS NULL
                            OR REDE = @Rede
                          )
                      AND (
                            @Venda IS NULL
                            OR CODIGO_VENDA = @Venda
                          )
                      AND (
                            @CodVendedor IS NULL
                            OR CODVENDEDOR = @CodVendedor
                          )
                      AND (
                            @Referencia IS NULL
                            OR REFERENCIA LIKE @Referencia
                          )
                      AND (
                            @Apelido IS NULL
                            OR APELIDO LIKE @Apelido
                          )
                      AND (
                            @Cliente IS NULL
                            OR CLIENTE LIKE @Cliente
                          )
                      AND (
                            @DataVendaInicio IS NULL
                            OR DATA_VENDA >= @DataVendaInicio
                          )
                      AND (
                            @DataVendaFim IS NULL
                            OR DATA_VENDA <= @DataVendaFim
                          )
                      AND (
                            @LastUpdateInicio IS NULL
                            OR LASTUPDATE_ORIGEM >= @LastUpdateInicio
                          )
                      AND (
                            @LastUpdateFim IS NULL
                            OR LASTUPDATE_ORIGEM <= @LastUpdateFim
                          )
                    GROUP BY
                        REDE,
                        CODIGO_VENDA
                    ORDER BY
                        MAX(DATA_VENDA) DESC,
                        MAX(LASTUPDATE_ORIGEM) DESC,
                        REDE,
                        CODIGO_VENDA
                    OFFSET @Offset ROWS
                    FETCH NEXT @TamanhoPagina ROWS ONLY
                )
                SELECT
                    P.REDE AS Rede,
                    P.CODIGO_VENDA AS CodigoVenda,
                    P.CODCLIENTE AS CodCliente,
                    P.CODVENDEDOR AS CodVendedor,

                    P.CODIGO_EMPRESA AS CodigoEmpresa,
                    P.CNPJ AS Cnpj,
                    P.APELIDO AS Apelido,
                    P.NOME AS Nome,

                    P.DATA_VENDA AS DataVenda,
                    P.LASTUPDATE_ORIGEM AS LastUpdateOrigem,

                    P.CLIENTE AS Cliente,
                    P.VENDEDOR AS Vendedor,
                    P.CONDICOES AS Condicoes,

                    P.VENDA_IMPORTADA AS VendaImportada,
                    P.STATUS AS Status,

                    P.REFERENCIA AS Referencia,
                    P.BARRAS AS Barras,
                    P.CODIGO_PRODUTO AS CodigoProduto,
                    P.TAMANHO AS Tamanho,
                    P.QUANTIDADE AS Quantidade,
                    P.UNITARIO AS Unitario,
                    P.DESCONTO AS Desconto,
                    P.TOTAL AS Total,
                    P.FRETE AS Frete,
                    P.TOTAL_FRETE AS TotalFrete,
                    P.CUSTO AS Custo,
                    P.COLECAO AS Colecao,
                    P.FORNECEDOR AS Fornecedor
                FROM dbo.SETA_VENDAS_PRODUTOS P

                INNER JOIN VendasFiltradas VF
                    ON VF.REDE = P.REDE
                   AND VF.CODIGO_VENDA = P.CODIGO_VENDA

                ORDER BY
                    VF.DATA_VENDA DESC,
                    VF.LASTUPDATE_ORIGEM DESC,
                    P.REDE,
                    P.CODIGO_VENDA,
                    P.REFERENCIA,
                    P.CODIGO_PRODUTO,
                    P.TAMANHO;
            ";

            var parametros = new
            {
                Rede =
                    NormalizarTexto(filtro.Rede),

                Venda =
                    NormalizarTexto(filtro.Venda),

                CodVendedor =
                    NormalizarTexto(filtro.CodVendedor),

                Referencia = referencia,
                Apelido = apelido,
                Cliente = cliente,

                filtro.DataVendaInicio,
                filtro.DataVendaFim,
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

            var linhas = (
                await resultado.ReadAsync<VendaProdutoLinha>()
            ).ToList();

            var vendasProdutos = linhas
                .GroupBy(x => new
                {
                    x.Rede,
                    x.CodigoVenda
                })
                .Select(g =>
                {
                    var venda = g.First();

                    return new VendaProdutoResponse
                    {
                        Rede = venda.Rede,
                        CodigoVenda = venda.CodigoVenda,
                        CodCliente = venda.CodCliente,
                        CodVendedor = venda.CodVendedor,

                        CodigoEmpresa = venda.CodigoEmpresa,
                        Cnpj = venda.Cnpj,
                        Apelido = venda.Apelido,
                        Nome = venda.Nome,

                        DataVenda = venda.DataVenda,
                        LastUpdateOrigem =
                            venda.LastUpdateOrigem,

                        Cliente = venda.Cliente,
                        Vendedor = venda.Vendedor,
                        Condicoes = venda.Condicoes,

                        VendaImportada =
                            venda.VendaImportada,

                        Status = venda.Status,

                        Produtos = g
                            .Select(p =>
                                new VendaProdutoItemResponse
                                {
                                    Referencia = p.Referencia,
                                    Barras = p.Barras,
                                    CodigoProduto =
                                        p.CodigoProduto,
                                    Tamanho = p.Tamanho,
                                    Quantidade = p.Quantidade,
                                    Unitario = p.Unitario,
                                    Desconto = p.Desconto,
                                    Total = p.Total,
                                    Frete = p.Frete,
                                    TotalFrete = p.TotalFrete,
                                    Custo = p.Custo,
                                    Colecao = p.Colecao,
                                    Fornecedor = p.Fornecedor
                                }
                            )
                            .ToList()
                    };
                })
                .ToList();

            return PagedResponse<VendaProdutoResponse>.Create(
                vendasProdutos,
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

        private class VendaProdutoLinha
        {
            public string? Rede { get; set; }
            public string? CodigoVenda { get; set; }
            public string? CodCliente { get; set; }
            public string? CodVendedor { get; set; }

            public string? CodigoEmpresa { get; set; }
            public string? Cnpj { get; set; }
            public string? Apelido { get; set; }
            public string? Nome { get; set; }

            public DateTime? DataVenda { get; set; }
            public DateTime? LastUpdateOrigem { get; set; }

            public string? Cliente { get; set; }
            public string? Vendedor { get; set; }
            public string? Condicoes { get; set; }

            public string? VendaImportada { get; set; }
            public string? Status { get; set; }

            public string? Referencia { get; set; }
            public string? Barras { get; set; }
            public string? CodigoProduto { get; set; }
            public string? Tamanho { get; set; }

            public decimal? Quantidade { get; set; }
            public decimal? Unitario { get; set; }
            public decimal? Desconto { get; set; }
            public decimal? Total { get; set; }
            public decimal? Frete { get; set; }
            public decimal? TotalFrete { get; set; }
            public decimal? Custo { get; set; }

            public string? Colecao { get; set; }
            public string? Fornecedor { get; set; }
        }
    }
}