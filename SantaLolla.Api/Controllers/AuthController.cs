using SantaLolla.Api.Models.Auth;
using SantaLolla.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SantaLolla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GerarToken([FromBody] TokenRequest request)
        {
            var token = await _tokenService.GerarTokenAsync(request);

            if (token == null)
            {
                return Unauthorized(new
                {
                    mensagem = "ClientId ou ClientSecret inválido."
                });
            }

            return Ok(token);
        }

        [HttpPost("token-form")]
        public async Task<IActionResult> GerarTokenForm([FromForm] TokenFormRequest request)
        {
            var tokenRequest = new TokenRequest
            {
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret
            };

            var token = await _tokenService.GerarTokenAsync(tokenRequest);

            if (token == null)
            {
                return Unauthorized(new
                {
                    mensagem = "ClientId ou ClientSecret inválido."
                });
            }

            return Ok(token);
        }
    }
}
