using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Vendas;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaRepository _vendaRepository;

        public VendasController(IVendaRepository vendaRepository)
        {
            _vendaRepository = vendaRepository;
        }

        /// <summary>
        /// Lista as vendas.
        /// </summary>
        /// <remarks>
        /// Retorna vendas filtradas por data da venda, data de atualização,
        /// rede, loja, nota fiscal, observação e paginação.
        ///
        /// É recomendado informar pelo menos um período:
        /// dataInicio/dataFim ou lastUpdateInicio/lastUpdateFim.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta de vendas.</param>
        /// <returns>Resultado paginado contendo as vendas.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido ou período não informado.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<VendaResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar(
            [FromQuery] VendaFiltroRequest filtro)
        {
            var informouDataVenda =
                filtro.DataInicio.HasValue ||
                filtro.DataFim.HasValue;

            var informouLastUpdate =
                filtro.LastUpdateInicio.HasValue ||
                filtro.LastUpdateFim.HasValue;

            if (!informouDataVenda && !informouLastUpdate)
            {
                return BadRequest(new
                {
                    mensagem =
                        "Informe pelo menos um período: dataInicio/dataFim ou lastUpdateInicio/lastUpdateFim."
                });
            }

            if (filtro.DataInicio.HasValue &&
                filtro.DataFim.HasValue &&
                filtro.DataInicio.Value.Date > filtro.DataFim.Value.Date)
            {
                return BadRequest(new
                {
                    mensagem = "dataInicio não pode ser maior que dataFim."
                });
            }

            if (filtro.LastUpdateInicio.HasValue &&
                filtro.LastUpdateFim.HasValue &&
                filtro.LastUpdateInicio.Value > filtro.LastUpdateFim.Value)
            {
                return BadRequest(new
                {
                    mensagem =
                        "lastUpdateInicio não pode ser maior que lastUpdateFim."
                });
            }

            var vendas = await _vendaRepository.ListarAsync(filtro);

            return Ok(vendas);
        }
    }
}