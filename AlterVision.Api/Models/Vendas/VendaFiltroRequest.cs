namespace AlterVision.Api.Models.Vendas
{
    public class VendaFiltroRequest
    {
        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public DateTime? LastUpdateInicio { get; set; }

        public DateTime? LastUpdateFim { get; set; }

        public string? Rede { get; set; }

        public string? CodigoLoja { get; set; }

        public int Pagina { get; set; } = 1;

        public int TamanhoPagina { get; set; } = 500;
    }
}