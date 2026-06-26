using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Estoques
{
    public class EstoqueResponse
    {
        /// <summary>
        /// Rede/schema de origem.
        /// </summary>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código da empresa/loja.
        /// </summary>
        [JsonPropertyName("codigoEmpresa")]
        public string? CodigoEmpresa { get; set; }

        /// <summary>
        /// CNPJ da empresa/loja.
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        /// <summary>
        /// Apelido/nome fantasia da empresa.
        /// </summary>
        [JsonPropertyName("apelidoEmpresa")]
        public string? ApelidoEmpresa { get; set; }

        /// <summary>
        /// Nome/razão social da empresa.
        /// </summary>
        [JsonPropertyName("nomeEmpresa")]
        public string? NomeEmpresa { get; set; }

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
        /// Tamanho do produto.
        /// </summary>
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
        /// Quantidade atual em estoque.
        /// </summary>
        [JsonPropertyName("quantidade")]
        public int? Quantidade { get; set; }

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

        /// <summary>
        /// Data base do estoque.
        /// </summary>
        [JsonPropertyName("dataEstoque")]
        public DateTime? DataEstoque { get; set; }

        /// <summary>
        /// Data de criação do registro na base de integração.
        /// </summary>
        [JsonPropertyName("dataCriacao")]
        public DateTime? DataCriacao { get; set; }

        /// <summary>
        /// Data da última atualização do registro na base de integração.
        /// </summary>
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }
    }
}