using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.VendasProdutos
{
    public class VendaProdutoResponse
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
        /// <remarks>Exemplo: 00247577</remarks>
        [JsonPropertyName("codigoVenda")]
        public string? CodigoVenda { get; set; }

        /// <summary>
        /// Código do cliente.
        /// </summary>
        /// <remarks>Exemplo: 00012345</remarks>
        [JsonPropertyName("codCliente")]
        public string? CodCliente { get; set; }

        /// <summary>
        /// Código do vendedor.
        /// </summary>
        /// <remarks>Exemplo: 00084745</remarks>
        [JsonPropertyName("codVendedor")]
        public string? CodVendedor { get; set; }

        /// <summary>
        /// Código da empresa/loja.
        /// </summary>
        /// <remarks>Exemplo: 00000001</remarks>
        [JsonPropertyName("codigoEmpresa")]
        public string? CodigoEmpresa { get; set; }

        /// <summary>
        /// CNPJ da empresa/loja.
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        /// <summary>
        /// Apelido/nome fantasia da loja.
        /// </summary>
        [JsonPropertyName("apelido")]
        public string? Apelido { get; set; }

        /// <summary>
        /// Nome da empresa/loja.
        /// </summary>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// Data da venda.
        /// </summary>
        [JsonPropertyName("dataVenda")]
        public DateTime? DataVenda { get; set; }

        /// <summary>
        /// Data/hora da última atualização na origem.
        /// </summary>
        [JsonPropertyName("lastUpdateOrigem")]
        public DateTime? LastUpdateOrigem { get; set; }

        /// <summary>
        /// Nome do cliente da venda.
        /// </summary>
        [JsonPropertyName("cliente")]
        public string? Cliente { get; set; }

        /// <summary>
        /// Nome/apelido do vendedor.
        /// </summary>
        [JsonPropertyName("vendedor")]
        public string? Vendedor { get; set; }

        /// <summary>
        /// Condição de pagamento da venda.
        /// </summary>
        [JsonPropertyName("condicoes")]
        public string? Condicoes { get; set; }

        /// <summary>
        /// Indica se a venda foi importada.
        /// </summary>
        [JsonPropertyName("vendaImportada")]
        public string? VendaImportada { get; set; }

        /// <summary>
        /// Status da venda.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Produtos/itens da venda.
        /// </summary>
        [JsonPropertyName("produtos")]
        public List<VendaProdutoItemResponse> Produtos { get; set; } = new();
    }

    public class VendaProdutoItemResponse
    {
        /// <summary>
        /// Referência do produto.
        /// </summary>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Código de barras do produto.
        /// </summary>
        [JsonPropertyName("barras")]
        public string? Barras { get; set; }

        /// <summary>
        /// Código do produto.
        /// </summary>
        [JsonPropertyName("codigoProduto")]
        public string? CodigoProduto { get; set; }

        /// <summary>
        /// Tamanho do produto.
        /// </summary>
        [JsonPropertyName("tamanho")]
        public string? Tamanho { get; set; }

        /// <summary>
        /// Quantidade vendida.
        /// </summary>
        [JsonPropertyName("quantidade")]
        public decimal? Quantidade { get; set; }

        /// <summary>
        /// Valor unitário do item.
        /// </summary>
        [JsonPropertyName("unitario")]
        public decimal? Unitario { get; set; }

        /// <summary>
        /// Valor de desconto do item.
        /// </summary>
        [JsonPropertyName("desconto")]
        public decimal? Desconto { get; set; }

        /// <summary>
        /// Valor total do item.
        /// </summary>
        [JsonPropertyName("total")]
        public decimal? Total { get; set; }

        /// <summary>
        /// Valor do frete do item.
        /// </summary>
        [JsonPropertyName("frete")]
        public decimal? Frete { get; set; }

        /// <summary>
        /// Valor total do item somado ao frete.
        /// </summary>
        [JsonPropertyName("totalFrete")]
        public decimal? TotalFrete { get; set; }

        /// <summary>
        /// Custo do item.
        /// </summary>
        [JsonPropertyName("custo")]
        public decimal? Custo { get; set; }

        /// <summary>
        /// Coleção do produto.
        /// </summary>
        [JsonPropertyName("colecao")]
        public string? Colecao { get; set; }

        /// <summary>
        /// CPF/CNPJ do fornecedor do produto.
        /// </summary>
        [JsonPropertyName("fornecedor")]
        public string? Fornecedor { get; set; }
    }
}