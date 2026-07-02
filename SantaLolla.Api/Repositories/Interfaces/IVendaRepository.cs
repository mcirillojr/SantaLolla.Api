using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Vendas;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<PagedResponse<VendaResponse>> ListarAsync(
            VendaFiltroRequest filtro
        );
    }
}