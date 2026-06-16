using SantaLolla.Api.Models.Lojas;
using SantaLolla.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SantaLolla.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LojasController : ControllerBase
    {
        private readonly ILojaRepository _lojaRepository;

        public LojasController(ILojaRepository lojaRepository)
        {
            _lojaRepository = lojaRepository;
        }

        /// <summary>
        /// Lista as lojas cadastradas.
        /// </summary>
        /// <remarks>
        /// Permite filtrar lojas por rede, código da loja e período de atualização.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta de lojas.</param>
        /// <returns>Lista de lojas.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LojaResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar([FromQuery] LojaFiltroRequest filtro)
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

            var lojas = await _lojaRepository.ListarAsync(filtro);

            return Ok(lojas);
        }
    }
}
