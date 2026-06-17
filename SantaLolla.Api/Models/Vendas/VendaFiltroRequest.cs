using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Vendas
{
    /// <summary>
    /// Filtros para consulta de vendas.
    /// </summary>
    public class VendaFiltroRequest
    {
        /// <summary>
        /// Data inicial da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-01</remarks>
        [JsonPropertyName("dataInicio")]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data final da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-15</remarks>
        [JsonPropertyName("dataFim")]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Data/hora inicial da última atualização da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11T00:00:00</remarks>
        [JsonPropertyName("lastUpdateInicio")]
        public DateTime? LastUpdateInicio { get; set; }

        /// <summary>
        /// Data/hora final da última atualização da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-15T23:59:59</remarks>
        [JsonPropertyName("lastUpdateFim")]
        public DateTime? LastUpdateFim { get; set; }

        /// <summary>
        /// Código da rede.
        /// </summary>
        /// <remarks>Exemplo: rede000001</remarks>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código da loja/empresa.
        /// </summary>
        /// <remarks>Exemplo: 00000001</remarks>
        [JsonPropertyName("codigoLoja")]
        public string? CodigoLoja { get; set; }

        /// <summary>
        /// Número da nota fiscal.
        /// Pesquisa usando LIKE.
        /// Pode informar somente parte do número ou usar percentual.
        /// </summary>
        /// <remarks>Exemplo: 123 ou %123%</remarks>
        [JsonPropertyName("notaFiscal")]
        public string? NotaFiscal { get; set; }

        /// <summary>
        /// Observação da venda.
        /// Pesquisa usando LIKE.
        /// Pode informar somente parte da observação ou usar percentual.
        /// </summary>
        /// <remarks>Exemplo: importada ou %VENDA IMPORTADA%</remarks>
        [JsonPropertyName("obs")]
        public string? Obs { get; set; }

        /// <summary>
        /// Número da página da consulta.
        /// </summary>
        /// <remarks>Valor padrão: 1</remarks>
        [JsonPropertyName("pagina")]
        public int Pagina { get; set; } = 1;

        /// <summary>
        /// Quantidade de registros por página.
        /// </summary>
        /// <remarks>Valor padrão: 500. Limite máximo aplicado pela API: 5000.</remarks>
        [JsonPropertyName("tamanhoPagina")]
        public int TamanhoPagina { get; set; } = 500;
    }
}