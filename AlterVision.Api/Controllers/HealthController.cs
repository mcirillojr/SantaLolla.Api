using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "OK",
                sistema = "SantaLolla.Api",
                mensagem = "API Santa Lolla em execução",
                dataHora = DateTime.Now
            });
        }
    }
}