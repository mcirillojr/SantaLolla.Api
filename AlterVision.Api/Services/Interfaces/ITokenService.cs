using AlterVision.Api.Models.Auth;

namespace AlterVision.Api.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse?> GerarTokenAsync(TokenRequest request);
    }
}