namespace AlterVision.Api.Models.Vendas
{
    public class VendaResponse
    {
        public string Rede { get; set; } = string.Empty;

        public string CodigoLoja { get; set; } = string.Empty;

        public string? Cnpj { get; set; }

        public string? AliasId { get; set; }

        public string CodigoVenda { get; set; } = string.Empty;

        public DateTime DataVenda { get; set; }

        public int? Hora { get; set; }

        public DateTime? EmissaoNf { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public string? CodigoCliente { get; set; }

        public string? Cliente { get; set; }

        public string CodigoVendedor { get; set; } = string.Empty;

        public string? Vendedor { get; set; }

        public string? Condicoes { get; set; }

        public decimal QtdeItens { get; set; }

        public decimal AVista { get; set; }

        public decimal APrazo { get; set; }

        public decimal Total { get; set; }

        public decimal Frete { get; set; }

        public decimal Custo { get; set; }

        public string? VendaImportada { get; set; }

        public string? Status { get; set; }
    }
}