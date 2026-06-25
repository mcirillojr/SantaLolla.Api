using System.Text.Json.Serialization;

namespace SantaLolla.Api.Models.ClientesVarejo
{
    /// <summary>
    /// Retorno detalhado de Clientes varejo.
    /// </summary>
    public class ClienteVarejoResponse
    {
        [JsonPropertyName("rede")]
        public string? Rede { get; set; }

        [JsonPropertyName("codigoCliente")]
        public string? CodigoCliente { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("apelido")]
        public string? Apelido { get; set; }

        [JsonPropertyName("pessoa")]
        public int? Pessoa { get; set; }

        [JsonPropertyName("cpfCnpj")]
        public string? CpfCnpj { get; set; }

        [JsonPropertyName("rgIe")]
        public string? RgIe { get; set; }

        [JsonPropertyName("docAuxiliar")]
        public string? DocAuxiliar { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("telefone1")]
        public string? Telefone1 { get; set; }

        [JsonPropertyName("telefone2")]
        public string? Telefone2 { get; set; }

        [JsonPropertyName("telefone3")]
        public string? Telefone3 { get; set; }

        [JsonPropertyName("telefone4")]
        public string? Telefone4 { get; set; }

        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("endereco")]
        public string? Endereco { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("cidade")]
        public string? Cidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("enderecoCompleto")]
        public string? EnderecoCompleto { get; set; }

        [JsonPropertyName("naturalidade")]
        public string? Naturalidade { get; set; }

        [JsonPropertyName("origem")]
        public string? Origem { get; set; }

        [JsonPropertyName("estadoCivil")]
        public string? EstadoCivil { get; set; }

        [JsonPropertyName("sexo")]
        public string? Sexo { get; set; }

        [JsonPropertyName("nascimento")]
        public DateTime? Nascimento { get; set; }

        [JsonPropertyName("aniversario")]
        public string? Aniversario { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }

        [JsonPropertyName("atividade")]
        public string? Atividade { get; set; }

        [JsonPropertyName("descricaoAtividade")]
        public string? DescricaoAtividade { get; set; }

        [JsonPropertyName("responsavel")]
        public string? Responsavel { get; set; }

        [JsonPropertyName("nomeResponsavel")]
        public string? NomeResponsavel { get; set; }

        [JsonPropertyName("empresa")]
        public string? Empresa { get; set; }

        [JsonPropertyName("cliente")]
        public bool? Cliente { get; set; }

        [JsonPropertyName("fornecedor")]
        public bool? Fornecedor { get; set; }

        [JsonPropertyName("funcionario")]
        public bool? Funcionario { get; set; }

        [JsonPropertyName("transportadora")]
        public bool? Transportadora { get; set; }

        [JsonPropertyName("conveniado")]
        public string? Conveniado { get; set; }

        [JsonPropertyName("credito")]
        public decimal? Credito { get; set; }

        [JsonPropertyName("bloqueia")]
        public decimal? Bloqueia { get; set; }

        [JsonPropertyName("cadastro")]
        public DateTime? Cadastro { get; set; }

        [JsonPropertyName("atualizado")]
        public DateTime? Atualizado { get; set; }

        [JsonPropertyName("obs")]
        public string? Obs { get; set; }
    }
}