using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Vendas;
using SantaLolla.Api.Repositories.Interfaces;
using System.Text.Json;

namespace SantaLolla.Api.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public VendaRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResponse<VendaResponse>> ListarAsync(
            VendaFiltroRequest filtro)
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

            var codigoVenda =
                string.IsNullOrWhiteSpace(filtro.CodigoVenda)
                    ? null
                    : filtro.CodigoVenda.Trim();

            var notaFiscal =
                PrepararFiltroLike(filtro.NotaFiscal);

            var obs =
                PrepararFiltroLike(filtro.Obs);

            const string sql = @"
                ------------------------------------------------------------
                -- TOTAL DE REGISTROS
                ------------------------------------------------------------
                SELECT
                    COUNT(1)
                FROM dbo.SETA_VENDAS_DETALHE V WITH (NOLOCK)
                WHERE
                    (
                        @DataInicio IS NULL
                        OR V.DATA_VENDA >= @DataInicio
                    )
                    AND (
                        @DataFim IS NULL
                        OR V.DATA_VENDA <= @DataFim
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
                        OR V.CODIGO_EMPRESA = @CodigoLoja
                    )
                    AND (
                        @CodigoVenda IS NULL
                        OR V.CODIGO_VENDA = @CodigoVenda
                    )
                    AND (
                        @NotaFiscal IS NULL
                        OR V.NOTA_FISCAL LIKE @NotaFiscal
                    )
                    AND (
                        @Obs IS NULL
                        OR V.OBS LIKE @Obs
                    );

                ------------------------------------------------------------
                -- IDENTIFICA AS VENDAS DA PÁGINA
                ------------------------------------------------------------
                SELECT
                    V.ID_VENDA_DETALHE
                INTO #VENDAS_PAGINA
                FROM dbo.SETA_VENDAS_DETALHE V WITH (NOLOCK)
                WHERE
                    (
                        @DataInicio IS NULL
                        OR V.DATA_VENDA >= @DataInicio
                    )
                    AND (
                        @DataFim IS NULL
                        OR V.DATA_VENDA <= @DataFim
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
                        OR V.CODIGO_EMPRESA = @CodigoLoja
                    )
                    AND (
                        @CodigoVenda IS NULL
                        OR V.CODIGO_VENDA = @CodigoVenda
                    )
                    AND (
                        @NotaFiscal IS NULL
                        OR V.NOTA_FISCAL LIKE @NotaFiscal
                    )
                    AND (
                        @Obs IS NULL
                        OR V.OBS LIKE @Obs
                    )

                ORDER BY
                    V.DATA_VENDA DESC,
                    V.LASTUPDATE_ORIGEM DESC,
                    V.REDE,
                    V.CODIGO_EMPRESA,
                    V.CODIGO_VENDA

                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;

                ------------------------------------------------------------
                -- DADOS DAS VENDAS
                ------------------------------------------------------------
                SELECT
                    V.REDE AS Rede,
                    V.CODIGO_EMPRESA AS CodigoLoja,

                    LOJA.MARCA AS Marca,

                    V.CNPJ AS Cnpj,
                    V.ALIAS_ID AS AliasId,
                    V.APELIDO AS Apelido,
                    V.NOME AS Nome,

                    V.CODIGO_VENDA AS CodigoVenda,
                    V.DATA_VENDA AS DataVenda,
                    V.HORA_VENDA AS HoraVenda,
                    V.NOTA_FISCAL AS NotaFiscal,
                    V.SERIE AS Serie,
                    V.EMISSAONF AS EmissaoNf,
                    V.LASTUPDATE_ORIGEM AS DataAtualizacao,

                    V.CODCLIENTE AS CodigoCliente,
                    V.CLIENTE AS Cliente,
                    V.CPFCNPJ_CLIENTE AS CpfCnpjCliente,

                    V.VENDA_VINCULADA AS VendaVinculada,
                    V.TIPO_OPERACAO AS TipoOperacao,

                    V.CODVENDEDOR AS CodigoVendedor,
                    V.VENDEDOR AS Vendedor,

                    V.CONDICOES AS Condicoes,
                    V.PARCELAS AS ParcelasJson,
                    V.QTDE_PARCELAS AS QtdeParcelas,
                    V.VALOR_TITULOS AS ValorTitulos,

                    V.QTDE_ITENS AS QtdeItens,
                    V.AVISTA AS AVista,
                    V.APRAZO AS APrazo,
                    V.TOTAL AS Total,
                    V.FRETE AS Frete,
                    V.CUSTO AS Custo,

                    V.VENDA_IMPORTADA AS VendaImportada,
                    V.STATUS AS Status,
                    V.OBS AS Obs

                FROM dbo.SETA_VENDAS_DETALHE V WITH (NOLOCK)

                INNER JOIN #VENDAS_PAGINA VP
                    ON VP.ID_VENDA_DETALHE = V.ID_VENDA_DETALHE

                OUTER APPLY
                (
                    SELECT TOP 1
                        L.MARCA
                    FROM dbo.SETA_LOJAS L WITH (NOLOCK)
                    WHERE L.REDE = V.REDE
                      AND L.CODIGO_EMPRESA = V.CODIGO_EMPRESA
                      AND L.ATIVO = 1
                    ORDER BY
                        L.ID_LOJA DESC
                ) LOJA

                ORDER BY
                    V.DATA_VENDA DESC,
                    V.LASTUPDATE_ORIGEM DESC,
                    V.REDE,
                    V.CODIGO_EMPRESA,
                    V.CODIGO_VENDA;

                ------------------------------------------------------------
                -- PAGAMENTOS DAS VENDAS DA PÁGINA
                ------------------------------------------------------------
                SELECT
                    P.REDE AS Rede,
                    P.CODIGO_EMPRESA AS CodigoLoja,
                    P.CODIGO_VENDA AS CodigoVenda,

                    P.CODIGO_TITULO AS CodigoTitulo,
                    P.STATUS AS Status,

                    P.LANCAMENTO AS Lancamento,
                    P.VENCIMENTO AS Vencimento,
                    P.PAGAMENTO AS Pagamento,

                    P.VALOR AS Valor,
                    
                    P.NUMERO_PARCELA AS NumeroParcela,
                    P.TOTAL_PARCELAS AS TotalParcelas,

                    P.POS_TEF AS PosTef,
                    P.FORMA AS Forma,
                    P.INSTITUICAO AS Instituicao,
                    P.NOME_INSTITUICAO AS NomeInstituicao,

                    P.OPERADORA AS Operadora,
                    P.BANDEIRA AS Bandeira,
                    P.NOME_OPERADORA AS NomeOperadora,

                    P.AUTORIZACAO AS Autorizacao,
                    P.NSU_HOST AS NsuHost

                FROM dbo.SETA_VENDAS_PAGAMENTOS P WITH (NOLOCK)

                INNER JOIN
                (
                    SELECT DISTINCT
                        V.REDE,
                        V.CODIGO_EMPRESA,
                        V.CODIGO_VENDA

                    FROM dbo.SETA_VENDAS_DETALHE V WITH (NOLOCK)

                    INNER JOIN #VENDAS_PAGINA VP
                        ON VP.ID_VENDA_DETALHE = V.ID_VENDA_DETALHE
                ) VENDAS
                    ON VENDAS.REDE = P.REDE
                    AND VENDAS.CODIGO_EMPRESA = P.CODIGO_EMPRESA
                    AND VENDAS.CODIGO_VENDA = P.CODIGO_VENDA

                ORDER BY
                    P.REDE,
                    P.CODIGO_EMPRESA,
                    P.CODIGO_VENDA,
                    P.NUMERO_PARCELA,
                    P.CODIGO_TITULO;

                DROP TABLE #VENDAS_PAGINA;
            ";

            var parametros = new
            {
                filtro.DataInicio,
                filtro.DataFim,
                filtro.LastUpdateInicio,
                filtro.LastUpdateFim,
                filtro.Rede,
                filtro.CodigoLoja,

                CodigoVenda = codigoVenda,
                NotaFiscal = notaFiscal,
                Obs = obs,

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

            // Total de vendas
            var total =
                await resultado.ReadSingleAsync<int>();

            // Vendas da página
            var vendas = (
                await resultado.ReadAsync<VendaResponse>()
            ).ToList();

            // Pagamentos das vendas da página
            var pagamentos = (
                await resultado.ReadAsync<VendaPagamentoDbResult>()
            ).ToList();

            // Converte JSON de parcelas
            var jsonOptions =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            foreach (var venda in vendas)
            {
                if (string.IsNullOrWhiteSpace(venda.ParcelasJson))
                {
                    venda.Parcelas =
                        new List<VendaParcelaResponse>();
                }
                else
                {
                    try
                    {
                        venda.Parcelas =
                            JsonSerializer.Deserialize<List<VendaParcelaResponse>>(
                                venda.ParcelasJson,
                                jsonOptions
                            ) ?? new List<VendaParcelaResponse>();
                    }
                    catch (JsonException)
                    {
                        venda.Parcelas =
                            new List<VendaParcelaResponse>();
                    }
                }
            }

            // Agrupa pagamentos por venda
            var pagamentosPorVenda =
                pagamentos
                    .GroupBy(p => CriarChaveVenda(
                        p.Rede,
                        p.CodigoLoja,
                        p.CodigoVenda
                    ))
                    .ToDictionary(
                        g => g.Key,
                        g => g
                            .Select(p => new VendaPagamentoResponse
                            {
                                CodigoVenda = p.CodigoVenda,
                                CodigoTitulo = p.CodigoTitulo,
                                Status = p.Status,
                                Lancamento = p.Lancamento,
                                Vencimento = p.Vencimento,
                                Pagamento = p.Pagamento,
                                Valor = p.Valor,
                                ValorPago = p.ValorPago,
                                NumeroParcela = p.NumeroParcela,
                                TotalParcelas = p.TotalParcelas,
                                PosTef = p.PosTef,
                                Forma = p.Forma,
                                Instituicao = p.Instituicao,
                                NomeInstituicao = p.NomeInstituicao,
                                Operadora = p.Operadora,
                                Bandeira = p.Bandeira,
                                NomeOperadora = p.NomeOperadora,
                                Autorizacao = p.Autorizacao,
                                NsuHost = p.NsuHost
                            })
                            .ToList()
                    );

            // Vincula os pagamentos às vendas
            foreach (var venda in vendas)
            {
                var chave =
                    CriarChaveVenda(
                        venda.Rede,
                        venda.CodigoLoja,
                        venda.CodigoVenda
                    );

                if (pagamentosPorVenda.TryGetValue(
                    chave,
                    out var pagamentosVenda))
                {
                    venda.Pagamentos =
                        pagamentosVenda;
                }
                else
                {
                    venda.Pagamentos =
                        new List<VendaPagamentoResponse>();
                }
            }

            return PagedResponse<VendaResponse>.Create(
                vendas,
                total,
                filtro.Pagina,
                filtro.TamanhoPagina
            );
        }

        private static string CriarChaveVenda(
            string? rede,
            string? codigoLoja,
            string? codigoVenda)
        {
            return string.Join(
                "|",
                rede?.Trim() ?? string.Empty,
                codigoLoja?.Trim() ?? string.Empty,
                codigoVenda?.Trim() ?? string.Empty
            );
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

        /// <summary>
        /// Estrutura interna utilizada somente para mapear
        /// os pagamentos retornados pelo banco.
        /// </summary>
        private sealed class VendaPagamentoDbResult
        {
            public string Rede { get; set; } = string.Empty;

            public string CodigoLoja { get; set; } = string.Empty;

            public string CodigoVenda { get; set; } = string.Empty;

            public string? CodigoTitulo { get; set; }

            public string? Status { get; set; }

            public DateTime? Lancamento { get; set; }

            public DateTime? Vencimento { get; set; }

            public DateTime? Pagamento { get; set; }

            public decimal? Valor { get; set; }

            public decimal? ValorPago { get; set; }

            public string? NumeroParcela { get; set; }

            public string? TotalParcelas { get; set; }

            public string? PosTef { get; set; }

            public string? Forma { get; set; }

            public string? Instituicao { get; set; }

            public string? NomeInstituicao { get; set; }

            public string? Operadora { get; set; }

            public string? Bandeira { get; set; }

            public string? NomeOperadora { get; set; }

            public string? Autorizacao { get; set; }

            public string? NsuHost { get; set; }
        }
    }
}