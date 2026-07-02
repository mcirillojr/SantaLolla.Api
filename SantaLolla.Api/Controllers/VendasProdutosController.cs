using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.VendasProdutos;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendasProdutosController : ControllerBase
    {
        private readonly IVendaProdutoRepository _vendaProdutoRepository;

        public VendasProdutosController(
            IVendaProdutoRepository vendaProdutoRepository
        )
        {
            _vendaProdutoRepository = vendaProdutoRepository;
        }

        /// <summary>
        /// Consulta os produtos das vendas.
        /// </summary>
        /// <remarks>
        /// Retorna os itens/produtos agrupados por venda.
        ///
        /// Permite filtros por venda, rede, vendedor, referência,
        /// loja, cliente, data da venda e data de atualização.
        ///
        /// A paginação é realizada por venda, e não por item.
        ///
        /// Paginação padrão: 500 vendas por página.
        /// Limite máximo: 5000 vendas por página.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta dos produtos das vendas.</param>
        /// <returns>Resultado paginado contendo as vendas e seus produtos.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        /// <response code="403">Acesso não autorizado ao endpoint.</response>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<VendaProdutoResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Listar(
            [FromQuery] VendaProdutoFiltroRequest filtro
        )
        {
            if (filtro.DataVendaInicio.HasValue &&
                filtro.DataVendaFim.HasValue &&
                filtro.DataVendaInicio.Value.Date >
                filtro.DataVendaFim.Value.Date)
            {
                return BadRequest(new
                {
                    mensagem =
                        "dataVendaInicio não pode ser maior que dataVendaFim."
                });
            }

            if (filtro.LastUpdateInicio.HasValue &&
                filtro.LastUpdateFim.HasValue &&
                filtro.LastUpdateInicio.Value >
                filtro.LastUpdateFim.Value)
            {
                return BadRequest(new
                {
                    mensagem =
                        "lastUpdateInicio não pode ser maior que lastUpdateFim."
                });
            }

            var vendasProdutos =
                await _vendaProdutoRepository.ListarAsync(filtro);

            return Ok(vendasProdutos);
        }
    }
}