namespace AlterVision.Api.Models.Lojas
{
    public class LojaResponse
    {
        public string Rede { get; set; } = string.Empty;

        public string CodigoLoja { get; set; } = string.Empty;

        public string NomeFantasia { get; set; } = string.Empty;

        public string Cnpj { get; set; } = string.Empty;

        public string? Cep { get; set; }

        public DateTime? DataAtualizacao { get; set; }
    }
}