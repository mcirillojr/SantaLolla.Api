using AlterVision.Api.Models.Vendas;

namespace AlterVision.Api.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<IEnumerable<VendaResponse>> ListarAsync(VendaFiltroRequest filtro);
    }
}