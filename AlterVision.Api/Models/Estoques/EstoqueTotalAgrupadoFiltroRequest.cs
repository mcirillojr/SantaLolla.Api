namespace AlterVision.Api.Models.Estoques
{
    public class EstoqueTotalAgrupadoFiltroRequest
    {
        public string? NomeLoja { get; set; }

        public string? Referencia { get; set; }

        public int Pagina { get; set; } = 1;

        public int TamanhoPagina { get; set; } = 500;
    }
}