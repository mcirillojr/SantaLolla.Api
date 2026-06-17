using SantaLolla.Api.Models.Vendas;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<IEnumerable<VendaResponse>> ListarAsync(VendaFiltroRequest filtro);
    }
}