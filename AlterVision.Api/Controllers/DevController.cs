using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public DevController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("gerar-hash")]
        public IActionResult GerarHash([FromQuery] string secret)
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

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