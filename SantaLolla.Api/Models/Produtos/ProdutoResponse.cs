using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Produtos
{
    /// <summary>
    /// Retorno dos produtos.
    /// </summary>
    public class ProdutoResponse
    {
        /// <summary>
        /// Rede/origem do produto.
        /// </summary>
        /// <remarks>Exemplo: public</remarks>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código interno do produto.
        /// </summary>
        [JsonPropertyName("codigoProduto")]
        public string? CodigoProduto { get; set; }

        /// <summary>
        /// Código Linx do produto.
        /// </summary>
        [JsonPropertyName("codigoLinx")]
        public string? CodigoLinx { get; set; }

        /// <summary>
        /// Código pai do produto.
        /// </summary>
        [JsonPropertyName("codigoPai")]
        public string? CodigoPai { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        [JsonPropertyName("descricaoProduto")]
        public string? DescricaoProduto { get; set; }

        /// <summary>
        /// Cor do produto.
        /// </summary>
        [JsonPropertyName("cor")]
        public string? Cor { get; set; }

        /// <summary>
        /// Referência do produto.
        /// </summary>
        [JsonPropertyName("referencia")]
        public string? Referencia { get; set; }

        /// <summary>
        /// Código da marca.
        /// </summary>
        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        /// <summary>
        /// Descrição da marca.
        /// </summary>
        /// <remarks>Exemplo: SantaLolla ou Degalls</remarks>
        [JsonPropertyName("descricaoMarca")]
        public string? DescricaoMarca { get; set; }

        /// <summary>
        /// URL pública da primeira imagem.
        /// </summary>
        [JsonPropertyName("urlFoto1")]
        public string? UrlFoto1 { get; set; }

        /// <summary>
        /// URL pública da segunda imagem.
        /// </summary>
        [JsonPropertyName("urlFoto2")]
        public string? UrlFoto2 { get; set; }

        /// <summary>
        /// URL pública da terceira imagem.
        /// </summary>
        [JsonPropertyName("urlFoto3")]
        public string? UrlFoto3 { get; set; }

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
        /// Descrição do departamento.
        /// </summary>
        [JsonPropertyName("descricaoDepartamento")]
        public string? DescricaoDepartamento { get; set; }

        /// <summary>
        /// Código do grupo.
        /// </summary>
        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        /// <summary>
        /// Descrição do grupo.
        /// </summary>
        [JsonPropertyName("descricaoGrupo")]
        public string? DescricaoGrupo { get; set; }

        /// <summary>
        /// Grade do produto.
        /// </summary>
        [JsonPropertyName("grade")]
        public string? Grade { get; set; }

        /// <summary>
        /// Descrição da grade do produto.
        /// </summary>
        /// <remarks>Exemplo: FEMININO 34/39</remarks>
        [JsonPropertyName("descricaoGrade")]
        public string? DescricaoGrade { get; set; }

        /// <summary>
        /// Código da coleção.
        /// </summary>
        [JsonPropertyName("codigoColecao")]
        public string? CodigoColecao { get; set; }

        /// <summary>
        /// Descrição da coleção.
        /// </summary>
        [JsonPropertyName("descricaoColecao")]
        public string? DescricaoColecao { get; set; }

        /// <summary>
        /// Código da linha.
        /// </summary>
        [JsonPropertyName("codigoLinha")]
        public string? CodigoLinha { get; set; }

        /// <summary>
        /// Descrição da linha.
        /// </summary>
        [JsonPropertyName("descricaoLinha")]
        public string? DescricaoLinha { get; set; }

        /// <summary>
        /// Código do subgrupo.
        /// </summary>
        [JsonPropertyName("subgrupo")]
        public string? Subgrupo { get; set; }

        /// <summary>
        /// Data de cadastro do produto na origem.
        /// </summary>
        [JsonPropertyName("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Última atualização do produto na origem.
        /// </summary>
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Indica se o produto está desativado.
        /// </summary>
        [JsonPropertyName("desativar")]
        public bool? Desativar { get; set; }

        /// <summary>
        /// Custo do produto.
        /// </summary>
        [JsonPropertyName("custo")]
        public decimal? Custo { get; set; }

        /// <summary>
        /// Custo de aquisição.
        /// </summary>
        [JsonPropertyName("custoAquisicao")]
        public decimal? CustoAquisicao { get; set; }

        /// <summary>
        /// Código de produto vinculado.
        /// </summary>
        [JsonPropertyName("vinculado")]
        public string? Vinculado { get; set; }

        /// <summary>
        /// Preço principal.
        /// </summary>
        [JsonPropertyName("preco")]
        public decimal? Preco { get; set; }

        /// <summary>
        /// Preço da tabela 1.
        /// </summary>
        [JsonPropertyName("preco1")]
        public decimal? Preco1 { get; set; }

        /// <summary>
        /// Preço da tabela 2.
        /// </summary>
        [JsonPropertyName("preco2")]
        public decimal? Preco2 { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        [JsonPropertyName("quantidade")]
        public decimal? Quantidade { get; set; }

        /// <summary>
        /// Informação de promoção.
        /// </summary>
        [JsonPropertyName("promocoes")]
        public string? Promocoes { get; set; }

        /// <summary>
        /// Indica se o produto está habilitado para e-commerce.
        /// </summary>
        [JsonPropertyName("ecommerce")]
        public bool? Ecommerce { get; set; }

        /// <summary>
        /// Data da última compra.
        /// </summary>
        [JsonPropertyName("ultimaCompra")]
        public DateTime? UltimaCompra { get; set; }
    }
}