namespace AlterVision.Api.Models.Vendedores
{
    public class VendedorFiltroRequest
    {
        public DateTime? LastUpdateInicio { get; set; }

        public DateTime? LastUpdateFim { get; set; }

        public string? Rede { get; set; }

        public string? CodigoLoja { get; set; }

        public string? CodigoVendedor { get; set; }

        public int Pagina { get; set; } = 1;

        public int TamanhoPagina { get; set; } = 500;
    }
}