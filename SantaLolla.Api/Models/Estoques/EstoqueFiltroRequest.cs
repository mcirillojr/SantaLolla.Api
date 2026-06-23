using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Estoques
{
    /// <summary>
    /// Filtros para consulta detalhada do estoque atual.
    /// </summary>
    public class EstoqueFiltroRequest
    {
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
        /// Código base do produto.
        /// </summary>
        /// <remarks>Exemplo: 123456</remarks>
        [JsonPropertyName("codigoProduto")]
        public string? CodigoProduto { get; set; }

        /// <summary>
        /// Referência comercial do produto.
        /// </summary>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Tamanho do produto.
        /// </summary>
        /// <remarks>Exemplo: 35</remarks>
        [JsonPropertyName("tamanho")]
        public string? Tamanho { get; set; }

        /// <summary>
        /// Cor do produto.
        /// </summary>
        [JsonPropertyName("cor")]
        public string? Cor { get; set; }

        /// <summary>
        /// Data/hora inicial da atualização do estoque.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-15T00:00:00</remarks>
        [JsonPropertyName("dataAtualizacaoInicio")]
        public DateTime? DataAtualizacaoInicio { get; set; }

        /// <summary>
        /// Data/hora final da atualização do estoque.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-15T23:59:59</remarks>
        [JsonPropertyName("dataAtualizacaoFim")]
        public DateTime? DataAtualizacaoFim { get; set; }

        /// <summary>
        /// Número da página da consulta.
        /// </summary>
        /// <example>1</example>
        /// <remarks>Valor padrão: 1</remarks>
        [JsonPropertyName("pagina")]
        public int Pagina { get; set; } = 1;

        /// <summary>
        /// Quantidade de registros por página.
        /// </summary>
        /// <example>500</example>
        /// <remarks>Valor padrão: 500. Limite máximo aplicado pela API: 5000.</remarks>
        [JsonPropertyName("tamanhoPagina")]
        public int TamanhoPagina { get; set; } = 500;
    }
}
