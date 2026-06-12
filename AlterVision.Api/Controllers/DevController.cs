using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        [HttpGet("gerar-hash")]
        public IActionResult GerarHash([FromQuery] string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
            {
                return BadRequest(new
                {
                    mensagem = "Informe o parâmetro secret."
                });
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(secret);

            return Ok(new
            {
                secretOriginal = secret,
                hash = hash
            });
        }
    }
}