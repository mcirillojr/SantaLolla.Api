using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.Lojas
{
    /// <summary>
    /// Retorno do cadastro de lojas.
    /// </summary>
    public class LojaResponse
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
        /// Nome fantasia ou apelido da loja.
        /// </summary>
        /// <remarks>Exemplo: SL - OSCAR FREIRE</remarks>
        
        [JsonPropertyName("nomeFantasia")]
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da loja.
        /// </summary>
        /// <remarks>Exemplo: 28.803.454/0001-10</remarks>
        
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        /// <summary>
        /// CEP da loja.
        /// </summary>
        /// <remarks>Exemplo: 01.426-003</remarks>
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        /// <summary>
        /// Data/hora da última atualização do cadastro na origem.
        /// </summary>
        /// <remarks>Exemplo: 2026-06-11T07:44:02</remarks>
        [JsonPropertyName("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }
    }
}
