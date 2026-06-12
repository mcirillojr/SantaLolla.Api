using AlterVision.Api.Models.Auth;
using AlterVision.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
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
    }
}