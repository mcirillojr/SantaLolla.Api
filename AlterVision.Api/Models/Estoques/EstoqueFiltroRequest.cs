namespace AlterVision.Api.Models.Estoques
{
    public class EstoqueFiltroRequest
    {
        public string? Rede { get; set; }

        public string? CodigoLoja { get; set; }

        public string? CodigoProduto { get; set; }

        public string? Referencia { get; set; }

        public string? Tamanho { get; set; }

        public string? Cor { get; set; }

        public DateTime? DataAtualizacaoInicio { get; set; }

        public DateTime? DataAtualizacaoFim { get; set; }

        public int Pagina { get; set; } = 1;

        public int TamanhoPagina { get; set; } = 500;
    }
}