using AlterVision.Api.Models.Lojas;

namespace AlterVision.Api.Repositories.Interfaces
{
    public interface ILojaRepository
    {
        Task<IEnumerable<LojaResponse>> ListarAsync(LojaFiltroRequest filtro);
    }
}