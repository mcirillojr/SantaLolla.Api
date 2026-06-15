using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlterVision.Api.Models.Vendedores
{
    /// <summary>
    /// Retorno do cadastro de vendedores.
    /// </summary>
    public class VendedorResponse
    {
        /// <summary>
        /// Código da rede de origem.
        /// </summary>
        /// <remarks>Exemplo: rede000001</remarks>
        
        [JsonPropertyName("rede")]
        public string Rede { get; set; } = string.Empty;

        /// <summary>
        /// Código da loja/empresa vinculada ao vendedor.
        /// </summary>
        /// <remarks>Exemplo: 00000001</remarks>
        [JsonPropertyName("codigoLoja")]
        public string? CodigoLoja { get; set; }

        /// <summary>
        /// Nome fantasia ou apelido da loja vinculada ao vendedor.
        /// </summary>
        /// <remarks>Exemplo: SL - OSCAR FREIRE</remarks>
        [JsonPropertyName("nomeLoja")]
        public string? NomeLoja { get; set; }

        /// <summary>
        /// Código do vendedor.
        /// </summary>
        /// <remarks>Exemplo: 00084745</remarks>
        
        [JsonPropertyName("codigoVendedor")]
        public string CodigoVendedor { get; set; } = string.Empty;

        /// <summary>
        /// Nome completo do vendedor.
        /// </summary>
        /// <remarks>Exemplo: THAIS MARTINS PEREIRA GODOIS</remarks>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// CPF do vendedor, quando disponível.
        /// </summary>
        /// <remarks>Exemplo: 453.580.568-74</remarks>
        [JsonPropertyName("cpf")]
        public string? Cpf { get; set; }

        /// <summary>
        /// Cargo ou atividade do vendedor.
        /// </summary>
        /// <remarks>Exemplo: VENDEDORA</remarks>
        [JsonPropertyName("cargo")]
        public string? Cargo { get; set; }

        /// <summary>
        /// Apelido ou nome reduzido do vendedor.
        /// </summary>
        /// <remarks>Exemplo: THAIS MARTINS</remarks>
        [JsonPropertyName("apelido")]
        public string? Apelido { get; set; }

        /// <summary>
        /// Data de admissão do vendedor.
        /// </summary>
        /// <remarks>Exemplo: 2019-02-01</remarks>
        [JsonPropertyName("dataAdmissao")]
        public DateTime? DataAdmissao { get; set; }

        /// <summary>
        /// Data de demissão do vendedor, quando houver.
        /// </summary>
        /// <remarks>Exemplo: 2026-05-31</remarks>
        [JsonPropertyName("dataDemissao")]
        public DateTime? DataDemissao { get; set; }

        /// <summary>
        /// Data/hora da última atualização do cadastro na origem.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11T10:09:32</remarks>
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        /// <summary>
        /// Status atual do vendedor.
        /// </summary>
        /// <remarks>Exemplo: Ativo</remarks>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}