namespace AlterVision.Api.Models.Estoques
{
    public class EstoqueResponse
    {
        public string Rede { get; set; } = string.Empty;

        public string CodigoLoja { get; set; } = string.Empty;

        public string? Cnpj { get; set; }

        public string? NomeLoja { get; set; }

        public string CodigoProduto { get; set; } = string.Empty;

        public string? DescricaoProduto { get; set; }

        public string? Tamanho { get; set; }

        public string? Cor { get; set; }

        public string? Grade { get; set; }

        public string? Referencia { get; set; }

        public string? Marca { get; set; }

        public string? Grupo { get; set; }

        public int Quantidade { get; set; }

        public decimal? Custo { get; set; }

        public decimal? Preco { get; set; }

        public decimal? Preco1 { get; set; }

        public decimal? Preco2 { get; set; }

        public DateTime? DataEstoque { get; set; }

        public DateTime? DataAtualizacao { get; set; }
    }
}