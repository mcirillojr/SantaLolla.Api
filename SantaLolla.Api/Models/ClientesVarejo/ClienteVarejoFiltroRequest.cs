using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.ClientesVarejo
{
    /// <summary>
    /// Filtros de busca para consultar Clientes Varejo.
    /// </summary>
    /// <remarks>
    /// DTO utilizado em requisições GET para filtrar e buscar clientes varejo por critérios múltiplos.
    /// Suporta paginação e LIKE patterns para campos de texto.
    /// Todos os filtros são opcionais e se omitidos não serão aplicados à busca.
    /// </remarks>
    public class ClienteVarejoFiltroRequest
    {
        /// <summary>
        /// Rede/schema de origem do cliente.
        /// </summary>
        /// <remarks>
        /// Identifica o contexto ou sistema de origem do cliente (ex: "LOJA1", "CENTRAL").
        /// Usado para filtrar clientes de uma rede específica.
        /// </remarks>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código único do cliente na origem.
        /// </summary>
        /// <remarks>
        /// Identificador único dentro da rede/schema. Usado para buscar cliente específico.
        /// Suporta valores numéricos ou alfanuméricos dependendo da origem.
        /// </remarks>
        [JsonPropertyName("codigoCliente")]
        public string? CodigoCliente { get; set; }

        /// <summary>
        /// Nome do cliente com suporte a filtros LIKE.
        /// </summary>
        /// <remarks>
        /// Permite busca parcial do nome. Use % para wildcard (ex: "%Marcio%" busca nomes contendo "Marcio").
        /// Sem especificar %, busca nomes que iniciam com o valor fornecido.
        /// Case-insensitive.
        /// </remarks>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// CPF ou CNPJ do cliente.
        /// </summary>
        /// <remarks>
        /// Aceita tanto CPF quanto CNPJ. Pode ser informado com ou sem formatação (com/sem pontos e hífens).
        /// Recomenda-se enviar sem formatação para melhor compatibilidade (ex: "12345678901234").
        /// </remarks>
        [JsonPropertyName("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        /// <summary>
        /// Data inicial do intervalo de atualização (inclusive).
        /// </summary>
        /// <remarks>
        /// Filtra clientes com campo ATUALIZADO &gt;= AtualizadoInicio.
        /// Formato: ISO 8601 (ex: 2024-01-15T10:30:00).
        /// Se não informado, não aplica limite inferior.
        /// </remarks>
        [JsonPropertyName("atualizadoInicio")]
        public DateTime? AtualizadoInicio { get; set; }

        /// <summary>
        /// Data final do intervalo de atualização (inclusive).
        /// </summary>
        /// <remarks>
        /// Filtra clientes com campo ATUALIZADO &lt;= AtualizadoFim.
        /// Formato: ISO 8601 (ex: 2024-01-31T23:59:59).
        /// Se não informado, não aplica limite superior.
        /// </remarks>
        [JsonPropertyName("atualizadoFim")]
        public DateTime? AtualizadoFim { get; set; }

        /// <summary>
        /// Número da página para paginação (baseado em 1).
        /// </summary>
        /// <remarks>
        /// Começa em 1. Padrão é 1 se não informado.
        /// Combinado com TamanhoPagina para calcular OFFSET da query.
        /// </remarks>
        /// <example>1</example>
        [JsonPropertyName("pagina")]
        public int Pagina { get; set; } = 1;

        /// <summary>
        /// Quantidade de registros retornados por página.
        /// </summary>
        /// <remarks>
        /// Padrão: 500 registros.
        /// Máximo: 5000 registros por página.
        /// Limite reduzido para evitar timeouts em buscas muito grandes.
        /// </remarks>
        /// <example>500</example>
        [JsonPropertyName("tamanhoPagina")]
        public int TamanhoPagina { get; set; } = 500;
    }
}