namespace AlterVision.Api.Models.Auth
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

        public int ExpiresIn { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}