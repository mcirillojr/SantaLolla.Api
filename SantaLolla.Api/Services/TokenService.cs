using SantaLolla.Api.Configurations;
using SantaLolla.Api.Models.Auth;
using SantaLolla.Api.Repositories.Interfaces;
using SantaLolla.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SantaLolla.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ITerceiroRepository _terceiroRepository;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            ITerceiroRepository terceiroRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _terceiroRepository = terceiroRepository;
        }

        public async Task<TokenResponse?> GerarTokenAsync(TokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientId) ||
                string.IsNullOrWhiteSpace(request.ClientSecret))
            {
                return null;
            }

            var terceiro = await _terceiroRepository.ObterPorClientIdAsync(request.ClientId);

            if (terceiro == null || !terceiro.Ativo)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(terceiro.ClientSecretHash))
            {
                return null;
            }

            bool secretValido;

            try
            {
                secretValido = BCrypt.Net.BCrypt.Verify(
                    request.ClientSecret,
                    terceiro.ClientSecretHash
                );
            }
            catch
            {
                return null;
            }

            if (!secretValido)
            {
                return null;
            }

            await _terceiroRepository.AtualizarUltimoAcessoAsync(terceiro.IdTerceiro);

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, terceiro.ClientId),
                new Claim("id_terceiro", terceiro.IdTerceiro.ToString()),
                new Claim("nome", terceiro.Nome),
                new Claim("client_id", terceiro.ClientId)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresIn = _jwtSettings.ExpirationMinutes * 60,
                ExpiresAt = expiresAt
            };
        }
    }
}
