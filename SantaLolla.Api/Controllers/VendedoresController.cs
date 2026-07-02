using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Vendedores;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Controllers
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

        /// <summary>
        /// Lista os vendedores cadastrados.
        /// </summary>
        /// <remarks>
        /// Permite filtrar vendedores por rede, loja, código do vendedor
        /// e período de atualização.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta de vendedores.</param>
        /// <returns>Resultado paginado contendo os vendedores.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<VendedorResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar(
            [FromQuery] VendedorFiltroRequest filtro)
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

            var vendedores =
                await _vendedorRepository.ListarAsync(filtro);

            return Ok(vendedores);
        }
    }
}