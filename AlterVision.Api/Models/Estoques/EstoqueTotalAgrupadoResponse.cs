namespace AlterVision.Api.Models.Estoques
{
    public class EstoqueTotalAgrupadoResponse
    {
        public string? NomeLoja { get; set; }

        public string? Referencia { get; set; }

        public string? DescricaoProduto { get; set; }

        public string CodigoProduto { get; set; } = string.Empty;

        public string? Tamanho { get; set; }

        public string? Marca { get; set; }

        public int QuantidadeTotal { get; set; }
    }
}