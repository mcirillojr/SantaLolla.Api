namespace SantaLolla.Api.Models.Logs
{
    public class ApiLogAcesso
    {
        public long? IdTerceiro { get; set; }
        public string? ClientId { get; set; }
        public string? NomeTerceiro { get; set; }

        public string MetodoHttp { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string? QueryString { get; set; }

        public int? StatusCode { get; set; }
        public int? TempoRespostaMs { get; set; }

        public string? IpOrigem { get; set; }
        public string? UserAgent { get; set; }

        public string? MensagemErro { get; set; }
    }
}
