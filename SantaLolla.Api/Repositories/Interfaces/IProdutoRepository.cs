using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Models.Produtos;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<PagedResponse<ProdutoResponse>> ListarAsync(
            ProdutoFiltroRequest filtro
        );
    }
}