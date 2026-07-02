using SantaLolla.Api.Models.ClientesVarejo;
using SantaLolla.Api.Models.PagedResponse;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IClienteVarejoRepository
    {
        Task<PagedResponse<ClienteVarejoResponse>> ListarAsync(
            ClienteVarejoFiltroRequest filtro
        );
    }
}