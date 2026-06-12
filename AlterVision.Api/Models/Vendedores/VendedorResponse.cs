namespace AlterVision.Api.Models.Vendedores
{
    public class VendedorResponse
    {
        public string Rede { get; set; } = string.Empty;

        public string? CodigoLoja { get; set; }

        public string? NomeLoja { get; set; }

        public string CodigoVendedor { get; set; } = string.Empty;

        public string? Nome { get; set; }

        public string? Cpf { get; set; }

        public string? Cargo { get; set; }

        public string? Apelido { get; set; }

        public DateTime? DataAdmissao { get; set; }

        public DateTime? DataDemissao { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public string? Status { get; set; }
    }
}