using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        /// Retorna os itens/produtos vendidos por venda.
        ///
        /// Permite filtros por venda, empresa, CNPJ, cliente, vendedor,
        /// referência, produto, código de barras, data da venda e lastupdate.
        ///
        /// Paginação padrão: 500 registros por página.
        /// Limite máximo: 5000 registros por página.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VendaProdutoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<VendaProdutoResponse>>> Listar(
            [FromQuery] VendaProdutoFiltroRequest filtro
        )
        {
            var vendasProdutos = await _vendaProdutoRepository.ListarAsync(filtro);

            return Ok(vendasProdutos);
        }
    }
}