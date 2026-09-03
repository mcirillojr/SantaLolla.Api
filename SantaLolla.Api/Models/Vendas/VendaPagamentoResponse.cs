using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Vendas
{
    /// <summary>
    /// Retorno dos pagamentos vinculados à venda.
    /// </summary>
    public class VendaPagamentoResponse
    {
        /// <summary>
        /// Código da venda vinculada ao pagamento.
        /// </summary>
        [JsonPropertyName("codigoVenda")]
        public string? CodigoVenda { get; set; }

        /// <summary>
        /// Código do título financeiro.
        /// </summary>
        [JsonPropertyName("codigoTitulo")]
        public string? CodigoTitulo { get; set; }

        /// <summary>
        /// Status do título.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Data de lançamento.
        /// </summary>
        [JsonPropertyName("lancamento")]
        public DateTime? Lancamento { get; set; }

        /// <summary>
        /// Data de vencimento.
        /// </summary>
        [JsonPropertyName("vencimento")]
        public DateTime? Vencimento { get; set; }

        /// <summary>
        /// Data de pagamento.
        /// </summary>
        [JsonPropertyName("pagamento")]
        public DateTime? Pagamento { get; set; }

        /// <summary>
        /// Valor do título.
        /// </summary>
        [JsonPropertyName("valor")]
        public decimal? Valor { get; set; }

        /// <summary>
        /// Valor pago.
        /// </summary>
        [JsonPropertyName("valorPago")]
        public decimal? ValorPago { get; set; }

        /// <summary>
        /// Número da parcela.
        /// </summary>
        [JsonPropertyName("numeroParcela")]
        public string? NumeroParcela { get; set; }

        /// <summary>
        /// Total de parcelas.
        /// </summary>
        [JsonPropertyName("totalParcelas")]
        public string? TotalParcelas { get; set; }

        /// <summary>
        /// Identificação se o pagamento foi POS ou TEF.
        /// </summary>
        [JsonPropertyName("posTef")]
        public string? PosTef { get; set; }

        /// <summary>
        /// Código da forma de pagamento.
        /// </summary>
        [JsonPropertyName("forma")]
        public string? Forma { get; set; }

        /// <summary>
        /// Código da instituição financeira.
        /// </summary>
        [JsonPropertyName("instituicao")]
        public string? Instituicao { get; set; }

        /// <summary>
        /// Nome da instituição financeira.
        /// </summary>
        [JsonPropertyName("nomeInstituicao")]
        public string? NomeInstituicao { get; set; }

        /// <summary>
        /// Operadora.
        /// </summary>
        [JsonPropertyName("operadora")]
        public string? Operadora { get; set; }

        /// <summary>
        /// Bandeira do cartão.
        /// </summary>
        [JsonPropertyName("bandeira")]
        public string? Bandeira { get; set; }

        /// <summary>
        /// Nome da operadora do cartão.
        /// </summary>
        [JsonPropertyName("nomeOperadora")]
        public string? NomeOperadora { get; set; }

        /// <summary>
        /// Código de autorização.
        /// </summary>
        [JsonPropertyName("autorizacao")]
        public string? Autorizacao { get; set; }

        /// <summary>
        /// NSU da transação.
        /// </summary>
        [JsonPropertyName("nsuHost")]
        public string? NsuHost { get; set; }
    }
}