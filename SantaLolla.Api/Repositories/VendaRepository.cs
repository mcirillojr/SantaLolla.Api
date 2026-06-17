using SantaLolla.Api.Data;
using SantaLolla.Api.Models.Vendas;
using SantaLolla.Api.Repositories.Interfaces;
using Dapper;

namespace SantaLolla.Api.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public VendaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<VendaResponse>> ListarAsync(VendaFiltroRequest filtro)
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

            var notaFiscal = PrepararFiltroLike(filtro.NotaFiscal);
            var obs = PrepararFiltroLike(filtro.Obs);

            const string sql = @"
                SELECT
                    REDE AS Rede,
                    CODIGO_EMPRESA AS CodigoLoja,
                    CNPJ AS Cnpj,
                    ALIAS_ID AS AliasId,
                    APELIDO AS Apelido,
                    NOME AS Nome,
                    CODIGO_VENDA AS CodigoVenda,
                    DATA_VENDA AS DataVenda,
                    NOTA_FISCAL AS NotaFiscal,
                    EMISSAONF AS EmissaoNf,
                    LASTUPDATE_ORIGEM AS DataAtualizacao,
                    CODCLIENTE AS CodigoCliente,
                    CLIENTE AS Cliente,
                    CODVENDEDOR AS CodigoVendedor,
                    VENDEDOR AS Vendedor,
                    CONDICOES AS Condicoes,
                    QTDE_ITENS AS QtdeItens,
                    AVISTA AS AVista,
                    APRAZO AS APrazo,
                    TOTAL AS Total,
                    FRETE AS Frete,
                    CUSTO AS Custo,
                    VENDA_IMPORTADA AS VendaImportada,
                    STATUS AS Status,
                    OBS AS Obs
                FROM dbo.ALTERVISION_VENDAS_DETALHE
                WHERE
                    (@DataInicio IS NULL OR DATA_VENDA >= @DataInicio)
                    AND (@DataFim IS NULL OR DATA_VENDA <= @DataFim)
                    AND (@LastUpdateInicio IS NULL OR LASTUPDATE_ORIGEM >= @LastUpdateInicio)
                    AND (@LastUpdateFim IS NULL OR LASTUPDATE_ORIGEM <= @LastUpdateFim)
                    AND (@Rede IS NULL OR REDE = @Rede)
                    AND (@CodigoLoja IS NULL OR CODIGO_EMPRESA = @CodigoLoja)
                    AND (@NotaFiscal IS NULL OR NOTA_FISCAL LIKE @NotaFiscal)
                    AND (@Obs IS NULL OR OBS LIKE @Obs)
                ORDER BY
                    DATA_VENDA DESC,
                    LASTUPDATE_ORIGEM DESC,
                    REDE,
                    CODIGO_EMPRESA,
                    CODIGO_VENDA
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<VendaResponse>(
                sql,
                new
                {
                    filtro.DataInicio,
                    filtro.DataFim,
                    filtro.LastUpdateInicio,
                    filtro.LastUpdateFim,
                    filtro.Rede,
                    filtro.CodigoLoja,
                    NotaFiscal = notaFiscal,
                    Obs = obs,
                    Offset = offset,
                    filtro.TamanhoPagina
                }
            );
        }

        private static string? PrepararFiltroLike(string? valor)
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