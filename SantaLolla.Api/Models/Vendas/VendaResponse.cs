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
        /// Código da venda.
        /// </summary>
        /// <remarks>Exemplo: 123456</remarks>
       
        [JsonPropertyName("codigoVenda")]
        public string CodigoVenda { get; set; } = string.Empty;

        /// <summary>
        /// Data da venda.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11</remarks>
        
        [JsonPropertyName("dataVenda")]
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Hora da venda no formato numérico da origem.
        /// </summary>
        /// <remarks>Exemplo: 1530</remarks>
        [JsonPropertyName("hora")]
        public int? Hora { get; set; }

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
        /// Quantidade total de itens da venda.
        /// </summary>
        /// <remarks>Exemplo: 2</remarks>
        
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
        /// Status da venda.
        /// </summary>
        /// <remarks>Exemplo: Válida</remarks>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
