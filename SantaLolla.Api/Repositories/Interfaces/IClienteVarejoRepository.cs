using SantaLolla.Api.Models.ClientesVarejo;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IClienteVarejoRepository
    {
        Task<IEnumerable<ClienteVarejoResponse>> ListarAsync(
            ClienteVarejoFiltroRequest filtro
        );
    }
}