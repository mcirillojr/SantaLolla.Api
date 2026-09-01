using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Vendas
{
    /// <summary>
    /// Retorno das parcelas/títulos da venda.
    /// </summary>
    public class VendaParcelaResponse
    {
        /// <summary>
        /// Número da parcela.
        /// </summary>
        /// <remarks>Exemplo: 01</remarks>
        [JsonPropertyName("parcela")]
        public string Parcela { get; set; } = string.Empty;

        /// <summary>
        /// Data de vencimento da parcela.
        /// </summary>
        /// <remarks>Exemplo: 2026-08-24</remarks>
        [JsonPropertyName("vencimento")]
        public DateOnly? Vencimento { get; set; }

        /// <summary>
        /// Valor da parcela.
        /// </summary>
        /// <remarks>Exemplo: 119.40</remarks>
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}