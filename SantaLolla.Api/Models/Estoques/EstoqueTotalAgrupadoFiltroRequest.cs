using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Estoques
{
    /// <summary>
    /// Filtros para consulta do estoque total agrupado.
    /// </summary>
    public class EstoqueTotalAgrupadoFiltroRequest
    {
        /// <summary>
        /// Pesquisa pelo nome da loja usando LIKE.
        /// </summary>
        /// <remarks>Exemplo: %oscar%</remarks>
        [JsonPropertyName("nomeLoja")]
        public string? NomeLoja { get; set; }

        /// <summary>
        /// Pesquisa pela referência do produto usando LIKE.
        /// </summary>
        /// <remarks>Exemplo: %07FC%</remarks>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

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
