using SantaLolla.Api.Models.Lojas;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface ILojaRepository
    {
        Task<IEnumerable<LojaResponse>> ListarAsync(LojaFiltroRequest filtro);
    }
}
