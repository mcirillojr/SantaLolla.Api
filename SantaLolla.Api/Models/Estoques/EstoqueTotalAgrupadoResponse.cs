using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Estoques
{
    public class EstoqueTotalAgrupadoResponse
    {
        /// <summary>
        /// Rede/schema de origem.
        /// </summary>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código do produto.
        /// </summary>
        [JsonPropertyName("codigoProduto")]
        public string? CodigoProduto { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        [JsonPropertyName("descricaoProduto")]
        public string? DescricaoProduto { get; set; }

        /// <summary>
        /// Referência do produto.
        /// </summary>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Marca do produto.
        /// </summary>
        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        /// <summary>
        /// Grupo do produto.
        /// </summary>
        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        /// <summary>
        /// Descrição da coleção do produto.
        /// </summary>
        [JsonPropertyName("descricaoColecao")]
        public string? DescricaoColecao { get; set; }

        /// <summary>
        /// Descrição da linha do produto.
        /// </summary>
        [JsonPropertyName("descricaoLinha")]
        public string? DescricaoLinha { get; set; }


        /// <summary>
        /// Quantidade total agrupada em estoque.
        /// </summary>
        [JsonPropertyName("quantidadeTotal")]
        public int? QuantidadeTotal { get; set; }

        /// <summary>
        /// Custo do produto.
        /// </summary>
        [JsonPropertyName("custo")]
        public decimal? Custo { get; set; }

        /// <summary>
        /// Preço principal do produto.
        /// </summary>
        [JsonPropertyName("preco")]
        public decimal? Preco { get; set; }

        /// <summary>
        /// Preço 1 do produto.
        /// </summary>
        [JsonPropertyName("preco1")]
        public decimal? Preco1 { get; set; }

        /// <summary>
        /// Preço 2 do produto.
        /// </summary>
        [JsonPropertyName("preco2")]
        public decimal? Preco2 { get; set; }
    }
}