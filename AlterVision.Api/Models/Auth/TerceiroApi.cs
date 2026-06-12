namespace AlterVision.Api.Models.Auth
{
    public class TerceiroApi
    {
        public long IdTerceiro { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecretHash { get; set; } = string.Empty;

        public bool Ativo { get; set; }
    }
}