using SantaLolla.Api.Models.Lojas;
using SantaLolla.Api.Models.PagedResponse;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface ILojaRepository
    {
        Task<PagedResponse<LojaResponse>> ListarAsync(
            LojaFiltroRequest filtro
        );
    }
}