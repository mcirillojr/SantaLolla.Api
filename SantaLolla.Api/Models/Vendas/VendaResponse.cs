using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Vendas
{
    /// <summary>
    /// Retorno das vendas.
    /// </summary>
    public class VendaResponse
    {
        /// <summary>
        /// Código da rede de origem.
        /// </summary>
        /// <remarks>Exemplo: rede000001</remarks>
        [JsonPropertyName("rede")]
        public string Rede { get; set; } = string.Empty;

        /// <summary>
        /// Código da loja/empresa da venda.
        /// </summary>
        /// <remarks>Exemplo: 00000001</remarks>
        [JsonPropertyName("codigoLoja")]
        public string CodigoLoja { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da loja da venda.
        /// </summary>
        /// <remarks>Exemplo: 28.803.454/0001-10</remarks>
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        /// <summary>
        /// Identificador auxiliar da loja ou origem.
        /// </summary>
        [JsonPropertyName("aliasId")]
        public string? AliasId { get; set; }

        /// <summary>
        /// Nome fantasia ou apelido da loja.
        /// </summary>
        /// <remarks>Exemplo: SL - OSCAR FREIRE</remarks>
        [JsonPropertyName("apelido")]
        public string? Apelido { get; set; }

        /// <summary>
        /// Razão social ou nome completo da loja.
        /// </summary>
        /// <remarks>Exemplo: SANTA LOLLA FRANQUIAS LTDA</remarks>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// Código da venda.
        /// </summary>
        /// <remarks>Exemplo: 123456</remarks>
        [Required]
        [JsonPropertyName("codigoVenda")]
        public string CodigoVenda { get; set; } = string.Empty;

        /// <summary>
        /// Data da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11</remarks>
        [JsonPropertyName("dataVenda")]
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Hora da venda no formato HH:MM.
        /// </summary>
        /// <remarks>Exemplo: 14:30</remarks>
        [JsonPropertyName("horaVenda")]
        public string? HoraVenda { get; set; }

        /// <summary>
        /// Número da nota fiscal autorizada relacionada à venda.
        /// </summary>
        /// <remarks>Exemplo: 00012345</remarks>
        [JsonPropertyName("notaFiscal")]
        public string? NotaFiscal { get; set; }

        /// <summary>
        /// Número da série da nota fiscal autorizada relacionada à venda.
        /// </summary>
        /// <remarks>Exemplo: 1</remarks>
        [JsonPropertyName("serie")]
        public string? Serie { get; set; }

        /// <summary>
        /// Data de emissão da nota fiscal, quando houver.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11</remarks>
        [JsonPropertyName("emissaoNf")]
        public DateTime? EmissaoNf { get; set; }

        /// <summary>
        /// Data/hora da última atualização da venda na origem.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11T09:30:00</remarks>
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Código do cliente da venda.
        /// </summary>
        [JsonPropertyName("codigoCliente")]
        public string? CodigoCliente { get; set; }

        /// <summary>
        /// Nome do cliente da venda.
        /// </summary>
        [JsonPropertyName("cliente")]
        public string? Cliente { get; set; }

        /// <summary>
        /// Código do vendedor da venda.
        /// </summary>
        /// <remarks>Exemplo: 00084745</remarks>
        [JsonPropertyName("codigoVendedor")]
        public string CodigoVendedor { get; set; } = string.Empty;

        /// <summary>
        /// Nome ou apelido do vendedor da venda.
        /// </summary>
        [JsonPropertyName("vendedor")]
        public string? Vendedor { get; set; }

        /// <summary>
        /// Condição ou forma de pagamento da venda.
        /// </summary>
        [JsonPropertyName("condicoes")]
        public string? Condicoes { get; set; }

        /// <summary>
        /// Quantidade de parcelas/títulos financeiros da venda.
        /// </summary>
        /// <remarks>Exemplo: 2</remarks>
        [JsonPropertyName("qtdeParcelas")]
        public int QtdeParcelas { get; set; }

        /// <summary>
        /// Parcelas/títulos financeiros da venda.
        /// </summary>
        [JsonPropertyName("parcelas")]
        public List<VendaParcelaResponse> Parcelas { get; set; } = new();

        /// <summary>
        /// Valor total dos títulos financeiros da venda.
        /// </summary>
        /// <remarks>Exemplo: 238.80</remarks>
        [JsonPropertyName("valorTitulos")]
        public decimal ValorTitulos { get; set; }

        /// <summary>
        /// Campo auxiliar utilizado internamente para receber o JSON das parcelas do banco.
        /// </summary>
        [JsonIgnore]
        public string? ParcelasJson { get; set; }

        /// <summary>
        /// Quantidade total de itens da venda.
        /// </summary>
        /// <remarks>Exemplo: 2.0000</remarks>
        [JsonPropertyName("qtdeItens")]
        public decimal QtdeItens { get; set; }

        /// <summary>
        /// Valor pago à vista.
        /// </summary>
        /// <remarks>Exemplo: 100.00</remarks>
        [JsonPropertyName("aVista")]
        public decimal AVista { get; set; }

        /// <summary>
        /// Valor pago a prazo.
        /// </summary>
        /// <remarks>Exemplo: 0.00</remarks>
        [JsonPropertyName("aPrazo")]
        public decimal APrazo { get; set; }

        /// <summary>
        /// Valor total da venda.
        /// </summary>
        /// <remarks>Exemplo: 199.90</remarks>
        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Valor do frete.
        /// </summary>
        [JsonPropertyName("frete")]
        public decimal Frete { get; set; }

        /// <summary>
        /// Custo total da venda.
        /// </summary>
        [JsonPropertyName("custo")]
        public decimal Custo { get; set; }

        /// <summary>
        /// Indica se a venda foi importada.
        /// </summary>
        /// <remarks>Exemplo: Sim ou Não</remarks>
        [JsonPropertyName("vendaImportada")]
        public string? VendaImportada { get; set; }

        /// <summary>
        /// Status da venda na origem.
        /// </summary>
        /// <remarks>Exemplo: S, O ou C</remarks>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Observação registrada na venda.
        /// </summary>
        /// <remarks>Exemplo: VENDA IMPORTADA EM 2026-06-17</remarks>
        [JsonPropertyName("obs")]
        public string? Obs { get; set; }
    }
}