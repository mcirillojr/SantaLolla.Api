using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlterVision.Api.Models.Estoques
{
    /// <summary>
    /// Retorno detalhado do estoque atual.
    /// </summary>
    public class EstoqueResponse
    {
        /// <summary>
        /// Código da rede de origem.
        /// </summary>
        /// <remarks>Exemplo: rede000001</remarks>
        
        [JsonPropertyName("rede")]
        public string Rede { get; set; } = string.Empty;

        /// <summary>
        /// Código da loja/empresa.
        /// </summary>
        /// <remarks>Exemplo: 00000001</remarks>
        
        [JsonPropertyName("codigoLoja")]
        public string CodigoLoja { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da loja.
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        /// <summary>
        /// Nome fantasia ou apelido da loja.
        /// </summary>
        /// <remarks>Exemplo: SL - OSCAR FREIRE</remarks>
        [JsonPropertyName("nomeLoja")]
        public string? NomeLoja { get; set; }

        /// <summary>
        /// Código base do produto.
        /// </summary>
        /// <remarks>Exemplo: 123456</remarks>
        
        [JsonPropertyName("codigoProduto")]
        public string CodigoProduto { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        [JsonPropertyName("descricaoProduto")]
        public string? DescricaoProduto { get; set; }

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
        /// Grade do produto.
        /// </summary>
        [JsonPropertyName("grade")]
        public string? Grade { get; set; }

        /// <summary>
        /// Referência comercial do produto.
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
        /// Quantidade atual em estoque.
        /// </summary>
        /// <remarks>Exemplo: 10</remarks>
        
        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

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
        /// Preço alternativo 1.
        /// </summary>
        [JsonPropertyName("preco1")]
        public decimal? Preco1 { get; set; }

        /// <summary>
        /// Preço alternativo 2.
        /// </summary>
        [JsonPropertyName("preco2")]
        public decimal? Preco2 { get; set; }

        /// <summary>
        /// Data de referência do estoque.
        /// </summary>
        [JsonPropertyName("dataEstoque")]
        public DateTime? DataEstoque { get; set; }

        /// <summary>
        /// Data/hora da última atualização do registro na API.
        /// </summary>
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }
    }
}