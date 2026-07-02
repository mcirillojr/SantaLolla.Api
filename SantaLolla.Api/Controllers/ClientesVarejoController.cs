using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantaLolla.Api.Models.ClientesVarejo;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesVarejoController : ControllerBase
    {
        private readonly IClienteVarejoRepository _clienteVarejoRepository;

        public ClientesVarejoController(
            IClienteVarejoRepository clienteVarejoRepository
        )
        {
            _clienteVarejoRepository = clienteVarejoRepository;
        }

        /// <summary>
        /// Consulta clientes de varejo.
        /// </summary>
        /// <remarks>
        /// Permite filtros por rede, código do cliente, CPF/CNPJ,
        /// nome e data de atualização.
        ///
        /// O filtro de nome permite LIKE. Exemplo: %Marcio%.
        ///
        /// Paginação padrão: 500 registros por página.
        /// Limite máximo: 5000 registros por página.
        /// </remarks>
        /// <param name="filtro">Filtros para consulta de clientes de varejo.</param>
        /// <returns>Resultado paginado contendo os clientes.</returns>
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="400">Filtro inválido.</response>
        /// <response code="401">Token ausente, expirado ou inválido.</response>
        /// <response code="403">Acesso não autorizado ao endpoint.</response>
        [HttpGet]
        [ProducesResponseType(
            typeof(PagedResponse<ClienteVarejoResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Listar(
            [FromQuery] ClienteVarejoFiltroRequest filtro
        )
        {
            if (filtro.AtualizadoInicio.HasValue &&
                filtro.AtualizadoFim.HasValue &&
                filtro.AtualizadoInicio.Value > filtro.AtualizadoFim.Value)
            {
                return BadRequest(new
                {
                    mensagem =
                        "atualizadoInicio não pode ser maior que atualizadoFim."
                });
            }

            var clientes =
                await _clienteVarejoRepository.ListarAsync(filtro);

            return Ok(clientes);
        }
    }
}