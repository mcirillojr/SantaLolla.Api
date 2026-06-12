using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
                sistema = "AlterVision.Api",
                mensagem = "API AlterVision em execução",
                dataHora = DateTime.Now
            });
        }
    }
}