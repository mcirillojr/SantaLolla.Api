using AlterVision.Api.Models.Estoques;

namespace AlterVision.Api.Repositories.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<IEnumerable<EstoqueResponse>> ListarAsync(EstoqueFiltroRequest filtro);

        Task<IEnumerable<EstoqueTotalAgrupadoResponse>> ListarTotalAgrupadoAsync(
            EstoqueTotalAgrupadoFiltroRequest filtro
        );
    }
}