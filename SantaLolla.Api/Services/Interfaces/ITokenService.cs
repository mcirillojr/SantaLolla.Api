using SantaLolla.Api.Models.Auth;

namespace SantaLolla.Api.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse?> GerarTokenAsync(TokenRequest request);
    }
}
