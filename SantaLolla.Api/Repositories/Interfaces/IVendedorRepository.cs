using SantaLolla.Api.Models.Vendedores;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IVendedorRepository
    {
        Task<IEnumerable<VendedorResponse>> ListarAsync(VendedorFiltroRequest filtro);
    }
}
