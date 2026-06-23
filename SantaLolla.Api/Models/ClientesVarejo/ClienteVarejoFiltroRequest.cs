using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.ClientesVarejo
{
    public class ClienteVarejoFiltroRequest
    {
        /// <summary>
        /// Rede/schema de origem.
        /// </summary>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código do cliente na origem.
        /// </summary>
        [JsonPropertyName("codigoCliente")]
        public string? CodigoCliente { get; set; }

        /// <summary>
        /// Nome do cliente. Permite LIKE.
        /// Pode informar Marcio ou %Marcio%.
        /// </summary>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// CPF ou CNPJ do cliente.
        /// </summary>
        [JsonPropertyName("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        /// <summary>
        /// Data inicial de atualização.
        /// Filtra ATUALIZADO maior ou igual.
        /// </summary>
        [JsonPropertyName("atualizadoInicio")]
        public DateTime? AtualizadoInicio { get; set; }

        /// <summary>
        /// Data final de atualização.
        /// Filtra ATUALIZADO menor ou igual.
        /// </summary>
        [JsonPropertyName("atualizadoFim")]
        public DateTime? AtualizadoFim { get; set; }

        /// <summary>
        /// Página da consulta.
        /// </summary>
        /// <example>1</example>
        [JsonPropertyName("pagina")]
        public int Pagina { get; set; } = 1;

        /// <summary>
        /// Quantidade de registros por página.
        /// Padrão 500, máximo 5000.
        /// </summary>
        /// <example>500</example>
        [JsonPropertyName("tamanhoPagina")]
        public int TamanhoPagina { get; set; } = 500;
    }
}