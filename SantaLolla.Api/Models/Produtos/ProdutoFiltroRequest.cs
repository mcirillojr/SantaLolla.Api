using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Produtos
{
    /// <summary>
    /// Filtros para consulta de produtos.
    /// </summary>
    public class ProdutoFiltroRequest
    {
        /// <summary>
        /// Rede/origem do produto.
        /// </summary>
        /// <remarks>Exemplo: public</remarks>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código do produto.
        /// </summary>
        /// <remarks>Exemplo: 170902</remarks>
        [JsonPropertyName("codigoProduto")]
        public string? CodigoProduto { get; set; }

        /// <summary>
        /// Código Linx do produto.
        /// </summary>
        /// <remarks>Exemplo: 1411211</remarks>
        [JsonPropertyName("codigoLinx")]
        public string? CodigoLinx { get; set; }

        /// <summary>
        /// Código pai do produto.
        /// </summary>
        [JsonPropertyName("codigoPai")]
        public string? CodigoPai { get; set; }

        /// <summary>
        /// Referência do produto.
        /// </summary>
        /// <remarks>
        /// Aceita pesquisa com LIKE.
        /// Exemplo: %0106.0D67%
        /// </remarks>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        /// <remarks>
        /// Aceita pesquisa com LIKE.
        /// Exemplo: %FLIP FLOP%
        /// </remarks>
        [JsonPropertyName("descricaoProduto")]
        public string? DescricaoProduto { get; set; }

        /// <summary>
        /// Código da marca.
        /// </summary>
        /// <remarks>Exemplo: 000009</remarks>
        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        /// <summary>
        /// Descrição da marca.
        /// </summary>
        /// <remarks>Exemplo: SantaLolla ou Degalls</remarks>
        [JsonPropertyName("descricaoMarca")]
        public string? DescricaoMarca { get; set; }

        /// <summary>
        /// Código do fornecedor.
        /// </summary>
        [JsonPropertyName("fornecedor")]
        public string? Fornecedor { get; set; }

        /// <summary>
        /// Código do departamento.
        /// </summary>
        [JsonPropertyName("departamento")]
        public string? Departamento { get; set; }

        /// <summary>
        /// Código do grupo.
        /// </summary>
        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        /// <summary>
        /// Código da coleção.
        /// </summary>
        [JsonPropertyName("codigoColecao")]
        public string? CodigoColecao { get; set; }

        /// <summary>
        /// Código da linha.
        /// </summary>
        [JsonPropertyName("codigoLinha")]
        public string? CodigoLinha { get; set; }

        /// <summary>
        /// Indica se o produto está desativado.
        /// </summary>
        [JsonPropertyName("desativar")]
        public bool? Desativar { get; set; }

        /// <summary>
        /// Indica se o produto está habilitado para e-commerce.
        /// </summary>
        [JsonPropertyName("ecommerce")]
        public bool? Ecommerce { get; set; }

        /// <summary>
        /// Data/hora inicial da última atualização do produto.
        /// </summary>
        /// <remarks>Exemplo: 2026-09-01T00:00:00</remarks>
        [JsonPropertyName("lastUpdateInicio")]
        public DateTime? LastUpdateInicio { get; set; }

        /// <summary>
        /// Data/hora final da última atualização do produto.
        /// </summary>
        /// <remarks>Exemplo: 2026-09-04T23:59:59</remarks>
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