using SantaLolla.Api.Models.Auth;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface ITerceiroRepository
    {
        Task<TerceiroApi?> ObterPorClientIdAsync(string clientId);

        Task AtualizarUltimoAcessoAsync(long idTerceiro);

        Task<bool> TerceiroTemPermissaoAsync(
            long idTerceiro,
            string endpoint,
            string metodoHttp
        );
    }
}