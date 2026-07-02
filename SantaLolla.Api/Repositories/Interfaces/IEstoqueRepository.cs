using SantaLolla.Api.Models.Estoques;
using SantaLolla.Api.Models.PagedResponse;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<PagedResponse<EstoqueResponse>> ListarAsync(
            EstoqueFiltroRequest filtro
        );

        Task<PagedResponse<EstoqueTotalAgrupadoResponse>>
            ListarTotalAgrupadoAsync(
                EstoqueTotalAgrupadoFiltroRequest filtro
            );
    }
}