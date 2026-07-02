using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Vendedores;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IVendedorRepository
    {
        Task<PagedResponse<VendedorResponse>> ListarAsync(
            VendedorFiltroRequest filtro
        );
    }
}