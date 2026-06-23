using SantaLolla.Api.Models.VendasProdutos;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IVendaProdutoRepository
    {
        Task<IEnumerable<VendaProdutoResponse>> ListarAsync(
            VendaProdutoFiltroRequest filtro
        );
    }
}