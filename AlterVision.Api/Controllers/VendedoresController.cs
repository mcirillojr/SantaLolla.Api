using AlterVision.Api.Models.Vendedores;
using AlterVision.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class VendedoresController : ControllerBase
    {
        private readonly IVendedorRepository _vendedorRepository;

        public VendedoresController(IVendedorRepository vendedorRepository)
        {
            _vendedorRepository = vendedorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] VendedorFiltroRequest filtro)
        {
            if (filtro.LastUpdateInicio.HasValue &&
                filtro.LastUpdateFim.HasValue &&
                filtro.LastUpdateInicio.Value > filtro.LastUpdateFim.Value)
            {
                return BadRequest(new
                {
                    mensagem = "lastUpdateInicio não pode ser maior que lastUpdateFim."
                });
            }

            var vendedores = await _vendedorRepository.ListarAsync(filtro);

            return Ok(vendedores);
        }
    }
}