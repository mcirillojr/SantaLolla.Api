using AlterVision.Api.Models.Vendas;
using AlterVision.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlterVision.Api.Controllers
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

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] VendaFiltroRequest filtro)
        {
            var informouDataVenda = filtro.DataInicio.HasValue || filtro.DataFim.HasValue;
            var informouLastUpdate = filtro.LastUpdateInicio.HasValue || filtro.LastUpdateFim.HasValue;

            if (!informouDataVenda && !informouLastUpdate)
            {
                return BadRequest(new
                {
                    mensagem = "Informe pelo menos um período: dataInicio/dataFim ou lastUpdateInicio/lastUpdateFim."
                });
            }

            if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue &&
                filtro.DataInicio.Value.Date > filtro.DataFim.Value.Date)
            {
                return BadRequest(new
                {
                    mensagem = "dataInicio não pode ser maior que dataFim."
                });
            }

            if (filtro.LastUpdateInicio.HasValue && filtro.LastUpdateFim.HasValue &&
                filtro.LastUpdateInicio.Value > filtro.LastUpdateFim.Value)
            {
                return BadRequest(new
                {
                    mensagem = "lastUpdateInicio não pode ser maior que lastUpdateFim."
                });
            }

            var vendas = await _vendaRepository.ListarAsync(filtro);

            return Ok(vendas);
        }
    }
}