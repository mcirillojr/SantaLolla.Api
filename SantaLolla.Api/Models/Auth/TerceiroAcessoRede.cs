namespace SantaLolla.Api.Models.Auth
{
    public class TerceiroAcessoRede
    {
        public bool AcessoTodasRedes { get; set; }

        public List<string> RedesPermitidas { get; set; } = new();
    }
}