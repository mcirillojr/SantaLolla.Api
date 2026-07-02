using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantaLolla.Api.Models.Estoques;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Controllers
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

        /// <summary>
        /// Lista o estoque atual detalhado.
        /// </summary>
        /// <remarks>
        /// Retorna o estoque atual por loja, produto, tamanho, cor e referência.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta do estoque detalhado.</param>
        /// <returns>Resultado paginado contendo o estoque detalhado.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<EstoqueResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar(
            [FromQuery] EstoqueFiltroRequest filtro)
        {
            if (filtro.DataAtualizacaoInicio.HasValue &&
                filtro.DataAtualizacaoFim.HasValue &&
                filtro.DataAtualizacaoInicio.Value >
                filtro.DataAtualizacaoFim.Value)
            {
                return BadRequest(new
                {
                    mensagem =
                        "dataAtualizacaoInicio não pode ser maior que dataAtualizacaoFim."
                });
            }

            var estoques =
                await _estoqueRepository.ListarAsync(filtro);

            return Ok(estoques);
        }

        /// <summary>
        /// Lista o estoque atual agrupado.
        /// </summary>
        /// <remarks>
        /// Retorna o estoque agrupado por rede, referência,
        /// descrição do produto, código do produto e marca.
        ///
        /// Permite pesquisar nome da loja, referência,
        /// coleção e linha usando LIKE.
        /// </remarks>
        /// <param name="filtro">
        /// Filtros para consulta do estoque total agrupado.
        /// </param>
        /// <returns>
        /// Resultado paginado contendo o estoque total agrupado.
        /// </returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        [HttpGet("total-agrupado")]
        [ProducesResponseType(
            typeof(PagedResponse<EstoqueTotalAgrupadoResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarTotalAgrupado(
            [FromQuery] EstoqueTotalAgrupadoFiltroRequest filtro)
        {
            var estoques =
                await _estoqueRepository
                    .ListarTotalAgrupadoAsync(filtro);

            return Ok(estoques);
        }
    }
}