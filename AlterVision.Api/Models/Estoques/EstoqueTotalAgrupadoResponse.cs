using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlterVision.Api.Models.Estoques
{
    /// <summary>
    /// Estoque total agrupado por loja, referência, produto, tamanho e marca.
    /// </summary>
    public class EstoqueTotalAgrupadoResponse
    {
        /// <summary>
        /// Nome fantasia da loja.
        /// </summary>
        /// <remarks>Exemplo: SL - OSCAR FREIRE</remarks>
        [JsonPropertyName("nomeLoja")]
        public string? NomeLoja { get; set; }

        /// <summary>
        /// Referência comercial do produto.
        /// </summary>
        /// <remarks>Exemplo: 07FC.49BA.0345.0001</remarks>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        /// <remarks>Exemplo: SAPATILHA VERNIZ OLD CROMO</remarks>
        [JsonPropertyName("descricaoProduto")]
        public string? DescricaoProduto { get; set; }

        /// <summary>
        /// Código base do produto.
        /// </summary>
        /// <remarks>Exemplo: 123456</remarks>
        [JsonPropertyName("codigoProduto")]
        public string CodigoProduto { get; set; } = string.Empty;

        /// <summary>
        /// Tamanho do produto.
        /// </summary>
        /// <remarks>Exemplo: 35</remarks>
        [JsonPropertyName("tamanho")]
        public string? Tamanho { get; set; }

        /// <summary>
        /// Marca do produto.
        /// </summary>
        /// <remarks>Exemplo: SANTA LOLLA</remarks>
        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        /// <summary>
        /// Quantidade total disponível em estoque para o agrupamento.
        /// </summary>
        /// <remarks>Exemplo: 8</remarks>
        [JsonPropertyName("quantidadeTotal")]
        public int QuantidadeTotal { get; set; }
    }
}