using AlterVision.Api.Models.Estoques;
using AlterVision.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EstoquesController : ControllerBase
    {
        private readonly IEstoqueRepository _estoqueRepository;

        public EstoquesController(IEstoqueRepository estoqueRepository)
        {
            _estoqueRepository = estoqueRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] EstoqueFiltroRequest filtro)
        {
            if (filtro.DataAtualizacaoInicio.HasValue &&
                filtro.DataAtualizacaoFim.HasValue &&
                filtro.DataAtualizacaoInicio.Value > filtro.DataAtualizacaoFim.Value)
            {
                return BadRequest(new
                {
                    mensagem = "dataAtualizacaoInicio não pode ser maior que dataAtualizacaoFim."
                });
            }

            var estoques = await _estoqueRepository.ListarAsync(filtro);

            return Ok(estoques);
        }
        [HttpGet("total-agrupado")]
        public async Task<IActionResult> ListarTotalAgrupado(
        [FromQuery] EstoqueTotalAgrupadoFiltroRequest filtro
    )
        {
            var estoques = await _estoqueRepository.ListarTotalAgrupadoAsync(filtro);

            return Ok(estoques);
        }
    }
}