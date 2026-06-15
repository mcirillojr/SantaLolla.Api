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

        /// <summary>
        /// Lista o estoque atual detalhado.
        /// </summary>
        /// <remarks>
        /// Retorna o estoque atual por loja, produto, tamanho, cor e referência.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta do estoque detalhado.</param>
        /// <returns>Lista de estoque detalhado.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstoqueResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    
        /// <summary>
        /// Lista o estoque atual agrupado.
        /// </summary>
        /// <remarks>
        /// Retorna o estoque agrupado por loja, referência, descrição do produto, código do produto, tamanho e marca.
        /// Permite pesquisar nome da loja e referência usando LIKE.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta do estoque total agrupado.</param>
        /// <returns>Lista de estoque total agrupado.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response> 



        [HttpGet("total-agrupado")]
        [ProducesResponseType(typeof(IEnumerable<EstoqueTotalAgrupadoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarTotalAgrupado(
            [FromQuery] EstoqueTotalAgrupadoFiltroRequest filtro
        )
        {
            var estoques = await _estoqueRepository.ListarTotalAgrupadoAsync(filtro);

            return Ok(estoques);
        }
    }
}