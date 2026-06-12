using AlterVision.Api.Models.Auth;

namespace AlterVision.Api.Repositories.Interfaces
{
    public interface ITerceiroRepository
    {
        Task<TerceiroApi?> ObterPorClientIdAsync(string clientId);

        Task AtualizarUltimoAcessoAsync(long idTerceiro);
    }
}