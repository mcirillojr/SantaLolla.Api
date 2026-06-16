using SantaLolla.Api.Models.Estoques;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<IEnumerable<EstoqueResponse>> ListarAsync(EstoqueFiltroRequest filtro);

        Task<IEnumerable<EstoqueTotalAgrupadoResponse>> ListarTotalAgrupadoAsync(
            EstoqueTotalAgrupadoFiltroRequest filtro
        );
    }
}
