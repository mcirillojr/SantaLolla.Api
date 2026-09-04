using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Produtos;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController(
            IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        /// <summary>
        /// Retorna os produtos cadastrados.
        /// </summary>
        /// <param name="filtro">
        /// Filtros utilizados na consulta dos produtos.
        /// </param>
        /// <returns>
        /// Lista paginada de produtos.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<ProdutoResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized
        )]
        [ProducesResponseType(
            StatusCodes.Status500InternalServerError
        )]
        public async Task<ActionResult<PagedResponse<ProdutoResponse>>> Listar(
            [FromQuery] ProdutoFiltroRequest filtro)
        {
            var resultado =
                await _produtoRepository.ListarAsync(
                    filtro
                );

            return Ok(resultado);
        }
    }
}