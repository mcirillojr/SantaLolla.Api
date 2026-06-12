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
        public async Task<IActionResult> Listar()
        {
            var vendedores = await _vendedorRepository.ListarAsync();

            return Ok(vendedores);
        }
    }
}