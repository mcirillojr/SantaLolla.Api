using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.VendasProdutos;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IVendaProdutoRepository
    {
        Task<PagedResponse<VendaProdutoResponse>> ListarAsync(
            VendaProdutoFiltroRequest filtro
        );
    }
}