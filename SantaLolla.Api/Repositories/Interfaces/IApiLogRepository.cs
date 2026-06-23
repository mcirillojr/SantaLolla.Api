using SantaLolla.Api.Models.Logs;

namespace SantaLolla.Api.Repositories.Interfaces
{
    public interface IApiLogRepository
    {
        Task GravarAsync(ApiLogAcesso log);
    }
}