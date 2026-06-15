using System.Text.Json.Serialization;

namespace AlterVision.Api.Models.Vendedores
{
    /// <summary>
    /// Filtros para consulta de vendedores.
    /// </summary>
    public class VendedorFiltroRequest
    {
        /// <summary>
        /// Data/hora inicial da última atualização.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11T00:00:00</remarks>
        [JsonPropertyName("lastUpdateInicio")]
        public DateTime? LastUpdateInicio { get; set; }

        /// <summary>
        /// Data/hora final da última atualização.
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
        /// Código do vendedor.
        /// </summary>
        /// <remarks>Exemplo: 00084745</remarks>
        [JsonPropertyName("codigoVendedor")]
        public string? CodigoVendedor { get; set; }

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