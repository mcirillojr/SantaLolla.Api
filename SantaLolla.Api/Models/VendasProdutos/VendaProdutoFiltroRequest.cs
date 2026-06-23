using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.VendasProdutos
{
    public class VendaProdutoFiltroRequest
    {
        /// <summary>
        /// Rede/schema de origem.
        /// </summary>
        /// <remarks>Exemplo: rede000001</remarks>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código da venda.
        /// </summary>
        /// <remarks>Exemplo: 00012345</remarks>
        [JsonPropertyName("venda")]
        public string? Venda { get; set; }

        /// <summary>
        /// Código do vendedor.
        /// </summary>
        /// <remarks>Exemplo: 00000010</remarks>
        [JsonPropertyName("codVendedor")]
        public string? CodVendedor { get; set; }

        /// <summary>
        /// Referência do produto.
        /// </summary>
        /// <remarks>
        /// Aceita pesquisa com LIKE.
        /// Exemplo: %ABC123%
        /// </remarks>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Apelido/nome fantasia da loja.
        /// </summary>
        /// <remarks>
        /// Aceita pesquisa com LIKE.
        /// Exemplo: %OSCAR%
        /// </remarks>
        [JsonPropertyName("apelido")]
        public string? Apelido { get; set; }

        /// <summary>
        /// Nome do cliente da venda.
        /// </summary>
        /// <remarks>
        /// Aceita pesquisa com LIKE.
        /// Exemplo: %MARCIO%
        /// </remarks>
        [JsonPropertyName("cliente")]
        public string? Cliente { get; set; }

        /// <summary>
        /// Data inicial da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-01</remarks>
        [JsonPropertyName("dataVendaInicio")]
        public DateTime? DataVendaInicio { get; set; }

        /// <summary>
        /// Data final da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-23</remarks>
        [JsonPropertyName("dataVendaFim")]
        public DateTime? DataVendaFim { get; set; }

        /// <summary>
        /// Data/hora inicial da última atualização.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11T00:00:00</remarks>
        [JsonPropertyName("lastUpdateInicio")]
        public DateTime? LastUpdateInicio { get; set; }

        /// <summary>
        /// Data/hora final da última atualização.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-23T23:59:59</remarks>
        [JsonPropertyName("lastUpdateFim")]
        public DateTime? LastUpdateFim { get; set; }

        /// <summary>
        /// Página da consulta.
        /// </summary>
        /// <example>1</example>
        /// <remarks>Valor padrão: 1</remarks>
        [JsonPropertyName("pagina")]
        public int Pagina { get; set; } = 1;

        /// <summary>
        /// Quantidade de registros por página.
        /// </summary>
        /// <example>500</example>
        /// <remarks>
        /// Valor padrão: 500.
        /// Limite máximo: 5000.
        /// </remarks>
        [JsonPropertyName("tamanhoPagina")]
        public int TamanhoPagina { get; set; } = 500;
    }
}