using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.ClientesVarejo
{
    /// <summary>
    /// Informações detalhadas de um Cliente Varejo.
    /// </summary>
    /// <remarks>
    /// DTO de resposta que contém dados completos de um cliente varejo incluindo identificação,
    /// documentação, contato, endereço, dados pessoais/comerciais, e informações financeiras.
    /// Retornado por endpoints de consulta individual ou listagem paginada.
    /// </remarks>
    public class ClienteVarejoResponse
    {
        /// <summary>
        /// Rede/schema de origem do cliente.
        /// </summary>
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        /// <summary>
        /// Código único do cliente na origem.
        /// </summary>
        [JsonPropertyName("codigoCliente")]
        public string? CodigoCliente { get; set; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        /// <summary>
        /// Apelido ou nome comercial do cliente.
        /// </summary>
        [JsonPropertyName("apelido")]
        public string? Apelido { get; set; }

        /// <summary>
        /// Tipo de pessoa (1 = Física, 2 = Jurídica).
        /// </summary>
        /// <remarks>
        /// Define se o cliente é uma pessoa física ou jurídica, impactando validações de documentos.
        /// </remarks>
        [JsonPropertyName("pessoa")]
        public int? Pessoa { get; set; }

        /// <summary>
        /// CPF (pessoa física) ou CNPJ (pessoa jurídica).
        /// </summary>
        [JsonPropertyName("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        /// <summary>
        /// RG (pessoa física) ou Inscrição Estadual (pessoa jurídica).
        /// </summary>
        [JsonPropertyName("rgIe")]
        public string? RgIe { get; set; }

        /// <summary>
        /// Documento auxiliar ou complementar.
        /// </summary>
        [JsonPropertyName("docAuxiliar")]
        public string? DocAuxiliar { get; set; }

        /// <summary>
        /// Endereço de email do cliente.
        /// </summary>
        /// <remarks>
        /// Pode conter múltiplos emails separados por ponto-e-vírgula ou vírgula conforme padrão da origem.
        /// </remarks>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Telefone de contato principal.
        /// </summary>
        [JsonPropertyName("telefone1")]
        public string? Telefone1 { get; set; }

        /// <summary>
        /// Telefone de contato secundário.
        /// </summary>
        [JsonPropertyName("telefone2")]
        public string? Telefone2 { get; set; }

        /// <summary>
        /// Terceiro número de telefone.
        /// </summary>
        [JsonPropertyName("telefone3")]
        public string? Telefone3 { get; set; }

        /// <summary>
        /// Quarto número de telefone.
        /// </summary>
        [JsonPropertyName("telefone4")]
        public string? Telefone4 { get; set; }

        /// <summary>
        /// Código de endereçamento postal.
        /// </summary>
        /// <remarks>
        /// Código postal ou ZIP code. Geralmente 8 dígitos para Brasil.
        /// </remarks>
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        /// <summary>
        /// Logradouro e número do endereço.
        /// </summary>
        [JsonPropertyName("endereco")]
        public string? Endereco { get; set; }

        /// <summary>
        /// Bairro do endereço.
        /// </summary>
        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        /// <summary>
        /// Cidade/município do endereço.
        /// </summary>
        [JsonPropertyName("cidade")]
        public string? Cidade { get; set; }

        /// <summary>
        /// Estado/Unidade Federativa (UF).
        /// </summary>
        /// <remarks>
        /// Dois caracteres (ex: SP, RJ, MG).
        /// </remarks>
        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        /// <summary>
        /// Complemento do endereço (apto, bloco, etc).
        /// </summary>
        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        /// <summary>
        /// Endereço completo formatado.
        /// </summary>
        /// <remarks>
        /// Concatenação de logradouro, número, complemento, bairro, cidade, UF.
        /// </remarks>
        [JsonPropertyName("enderecoCompleto")]
        public string? EnderecoCompleto { get; set; }

        /// <summary>
        /// Naturalidade (cidade/UF de nascimento).
        /// </summary>
        [JsonPropertyName("naturalidade")]
        public string? Naturalidade { get; set; }

        /// <summary>
        /// Origem ou fonte do cadastro.
        /// </summary>
        [JsonPropertyName("origem")]
        public string? Origem { get; set; }

        /// <summary>
        /// Estado civil do cliente (se pessoa física).
        /// </summary>
        [JsonPropertyName("estadoCivil")]
        public string? EstadoCivil { get; set; }

        /// <summary>
        /// Sexo do cliente (M, F, etc).
        /// </summary>
        [JsonPropertyName("sexo")]
        public string? Sexo { get; set; }

        /// <summary>
        /// Data de nascimento do cliente.
        /// </summary>
        [JsonPropertyName("nascimento")]
        public DateTime? Nascimento { get; set; }

        /// <summary>
        /// Aniversário formatado (DD/MM).
        /// </summary>
        /// <remarks>
        /// Formato DD/MM extraído da data de nascimento para campanhas sazonais.
        /// </remarks>
        [JsonPropertyName("aniversario")]
        public string? Aniversario { get; set; }

        /// <summary>
        /// Status do cliente (Ativo, Inativo, Bloqueado, etc).
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Grupo ou categoria de cliente.
        /// </summary>
        /// <remarks>
        /// Classificação comercial para segmentação (ex: VIP, Regular, Premium).
        /// </remarks>
        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        /// <summary>
        /// Código da atividade/ramo do cliente.
        /// </summary>
        [JsonPropertyName("atividade")]
        public string? Atividade { get; set; }

        /// <summary>
        /// Descrição da atividade/ramo de negócio.
        /// </summary>
        [JsonPropertyName("descricaoAtividade")]
        public string? DescricaoAtividade { get; set; }

        /// <summary>
        /// Código do responsável pelo cliente.
        /// </summary>
        [JsonPropertyName("responsavel")]
        public string? Responsavel { get; set; }

        /// <summary>
        /// Nome do responsável pelo cliente.
        /// </summary>
        [JsonPropertyName("nomeResponsavel")]
        public string? NomeResponsavel { get; set; }

        /// <summary>
        /// Empresa ou filial do cliente.
        /// </summary>
        [JsonPropertyName("empresa")]
        public string? Empresa { get; set; }

        /// <summary>
        /// Indica se o cliente é um comprador/cliente ativo.
        /// </summary>
        [JsonPropertyName("cliente")]
        public bool? Cliente { get; set; }

        /// <summary>
        /// Indica se o cliente é um fornecedor.
        /// </summary>
        [JsonPropertyName("fornecedor")]
        public bool? Fornecedor { get; set; }

        /// <summary>
        /// Indica se o cliente é um funcionário.
        /// </summary>
        [JsonPropertyName("funcionario")]
        public bool? Funcionario { get; set; }

        /// <summary>
        /// Indica se o cliente é uma transportadora.
        /// </summary>
        [JsonPropertyName("transportadora")]
        public bool? Transportadora { get; set; }

        /// <summary>
        /// Indica se o cliente é um conveniado/parceiro.
        /// </summary>
        [JsonPropertyName("conveniado")]
        public string? Conveniado { get; set; }

        /// <summary>
        /// Limite de crédito disponível para o cliente.
        /// </summary>
        /// <remarks>
        /// Valor em moeda corrente. Zero indica sem crédito.
        /// </remarks>
        [JsonPropertyName("credito")]
        public decimal? Credito { get; set; }

        /// <summary>
        /// Valor de bloqueio de crédito do cliente.
        /// </summary>
        /// <remarks>
        /// Quando atingido ou ultrapassado, bloqueia novas transações a crédito.
        /// </remarks>
        [JsonPropertyName("bloqueia")]
        public decimal? Bloqueia { get; set; }

        /// <summary>
        /// Data de cadastro do cliente.
        /// </summary>
        [JsonPropertyName("cadastro")]
        public DateTime? Cadastro { get; set; }

        /// <summary>
        /// Data da última atualização dos dados.
        /// </summary>
        [JsonPropertyName("atualizado")]
        public DateTime? Atualizado { get; set; }

        /// <summary>
        /// Observações ou notas adicionais sobre o cliente.
        /// </summary>
        /// <remarks>
        /// Campo livre para anotações, restrições especiais, ou informações adicionais.
        /// </remarks>
        [JsonPropertyName("obs")]
        public string? Obs { get; set; }
    }
}