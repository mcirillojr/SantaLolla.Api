using AlterVision.Api.Models.Vendedores;

namespace AlterVision.Api.Repositories.Interfaces
{
    public interface IVendedorRepository
    {
        Task<IEnumerable<VendedorResponse>> ListarAsync(VendedorFiltroRequest filtro);
    }
}